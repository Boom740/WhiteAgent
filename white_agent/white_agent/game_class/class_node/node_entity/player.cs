using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;
using MonoGame.Extended.Input;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace old_heart
{
    public class player : entity
    {
        public run_data_manager run_data;

        public Vector2 input_direction = Vector2.Zero;
        public enum state { idle, walk }
        public enum combat_state { none, attack, aim , dash , die , change_scene}
        public enum bufferable_input { none , attack , dash }
        public state current_state = state.idle;
        public combat_state current_combat_state = combat_state.none;
        public bufferable_input current_buffer_input = bufferable_input.none;

        public float input_buffer_time_limit = 0.3f;
        public float current_input_beffer_time = 0f;

        public float i_frame_time = 0f;  // currnetly use when change scene only
        // --- combat: melee ---
        public float attack_duration = 20f /60f; // ~10 frame ที่ 60fps เป็น placeholder ไปก่อน
        public float attack_timer = 0f;
        // --- I-frame---
        public float i_frame_duration = 3f; // ระยะเวลา i-frame ทั้งหมด ใช้ทั้งคุม i_frame_time และ pickup_lock_timer ของหัว ให้ sync กัน
        public float blink_interval = 0.1f;
        public float blink_min_alpha = 0.2f;

        public List<melee_data> combo_hits = new List<melee_data>
         {
          new melee_data(damage: 1, lunge_speed: 200f, range: 70f, hitbox_lifetime: 0.07f, knockback_speed: 200f , hitbox_radius: 35), // hit 1
          new melee_data(damage: 1, lunge_speed: 200f, range: 70f, hitbox_lifetime: 0.07f, knockback_speed: 200f, hitbox_radius: 35), // hit 2
          new melee_data(damage: 2, lunge_speed: 400f, range: 70f, hitbox_lifetime: 0.07f, knockback_speed: 400f, hitbox_radius: 35), // hit 3 (finisher)
         };
        // --- combat: melee combo ---
        public int combo_count = 0;
        public int max_combo = 3;
        public float combo_reset_window = 1f;   // เว้นช่วงกดเกินเท่านี้ = คอมโบหลุด
        public float combo_reset_timer = 0f;
        public float combo_cooldown_duration = 0.5f; // คูลดาวน์หลังคอมโบครบ 4
        public float melee_cooldown_timer = 0f;

        public float attack_input_delay = 0.15f; // ดีเลย์ขั้นต่ำระหว่างแต่ละ hit กันคลิกรัวเกินจังหวะ
        private float next_attack_timer = 0f;

        // --- combat: head throw ---
        private projectile_head head_projectile;
        public bool has_head = true;
        public float head_throw_speed = 1200f;
        public float pickup_radius = 24f;
        public float throw_head_i_frame_duration = 1f;

        public float head_drop_speed = 300;
        // --- aim  ---
        public float aim_speed_multiplier = 0.2f;
        public animation_player_player animation_player_2;

        // --- dash (Space) ---
        public float dash_speed = 1000f;
        public float dash_duration = 0.1f; // ยกเลิก dash ถ้าไปไม่ถึงภายในเวลานี้
        public float dash_i_frame_duration = 0.6f;
        public float fade_i_frame_visual_timer = 0f; // player has i_frame but not blink
        public float dash_timer = 0f;
        public float dash_cooldown = 1.5f;
        public float dash_cooldown_timer = 0f;

        private float default_max_velocity;

        public player(ContentManager content, Vector2 position , run_data_manager run_data) : base(content, position , speed: 5000)
        {
            animation_player = new animation_player_player(content);
            animation_player_2 = new animation_player_player(content);
            ground_friction = 10f;
            max_velocity = 300;
            default_max_velocity = max_velocity;

            this.run_data = run_data;
            hp = run_data.hp_left;
            max_hp = run_data.max_hp;
        }
        public override void Update(GameTime gameTime)
        {
            float delta_time = (float)gameTime.ElapsedGameTime.TotalSeconds;

            KeyboardStateExtended keyboard_state = global.input.keyboard_state;
            MouseStateExtended mouse_state = global.input.mouse_state; // TODO: เช็คว่าชื่อ property ตรงกับของจริงในโปรเจกต์ไหม
            input_direction = Vector2.Zero;

            update_cooldown();
            update_input_buffer();

            switch (current_combat_state)
            {
                case combat_state.attack:
                    update_attack_state();
                    break;

                case combat_state.aim:
                    update_aim_state();
                    break;

                case combat_state.dash:
                    update_dash_state(); 
                    break;

                case combat_state.none:
                    update_free_state();
                    break;

                case combat_state.die:
                    update_die_state();
                    break;

                case combat_state.change_scene:
                    update_change_scene_state();
                    break;
            }

            if (keyboard_state.WasKeyPressed(Keys.F))       // debug 
            {
                take_damage(1);
                Debug.WriteLine("hp left " + hp + " / " + max_hp);
            }          // debug
            if (keyboard_state.WasKeyPressed(Keys.T))
            {
                projectile_test projectile_test = new projectile_test(content, 3, position);
                projectile_test.owner = this;
                projectile_test.velocity = Vector2.Normalize(global.input.scaled_mouse_world_position - position) * 300;
                global.signal.spawn_projectile(projectile_test);
            }          // debug



            acceleration = input_direction;
            base.Update(gameTime);



            void update_input_buffer()
            {
                if (keyboard_state.WasKeyPressed(Keys.Space))
                {
                    buffer_input(bufferable_input.dash);
                }
                else if (mouse_state.WasButtonPressed(MouseButton.Left) && current_combat_state != combat_state.aim && global.input.mouse_in_screen)
                {
                    buffer_input(bufferable_input.attack);
                }

                if (current_buffer_input == bufferable_input.none) { return; }
                if (current_input_beffer_time < input_buffer_time_limit)
                {
                    current_input_beffer_time += delta_time;
                }
                else
                {
                    current_buffer_input = bufferable_input.none;
                }

                void buffer_input(bufferable_input buffer_input)
                {
                    current_buffer_input = buffer_input;
                    current_input_beffer_time = 0;
                }
            }

            void update_cooldown()
            {
                // --- combo cooldown countdown ---
                if (melee_cooldown_timer > 0f)
                {
                    melee_cooldown_timer -= delta_time;
                }
                // --- combo reset countdown (เฉพาะตอนไม่ได้ cooldown อยู่) ---
                else if (combo_count > 0)
                {
                    combo_reset_timer -= delta_time;
                    if (combo_reset_timer <= 0f)
                    {
                        combo_count = 0; // เว้นช่วงนานเกินไป คอมโบหลุดกลับไปนับ 1 ใหม่
                    }
                }

                // --- attack input delay countdown ---
                if (next_attack_timer > 0f)
                {
                    next_attack_timer -= delta_time;
                }

                if (dash_cooldown_timer > 0)
                {
                    dash_cooldown_timer -= delta_time;
                }

                if (i_frame_time > 0f)  // i frame
                {
                    i_frame_time -= delta_time;
                }
                if (fade_i_frame_visual_timer > 0f)  //  // player has i_frame but not blink
                {
                    fade_i_frame_visual_timer -= delta_time;
                }
            }

            //  local functions: state handlers (เรียกจาก switch ด้านบน) 
            void update_free_state()
            {
                check_head_pickup();
                update_movement_input();

                if (current_buffer_input == bufferable_input.attack)     // attack
                {
                    if (melee_cooldown_timer <= 0 && next_attack_timer <= 0f)
                    {
                        start_attack();
                        return;
                    }
                }
                else if (mouse_state.IsButtonDown(MouseButton.Right) && has_head && global.input.mouse_in_screen)
                {
                    current_combat_state = combat_state.aim;
                    animation_player_2.play(animation_player_2.data.data[animation_player_player.animation_name.takeoff_head]);
                    return;
                }
                else if (current_buffer_input == bufferable_input.dash && dash_cooldown_timer <= 0)
                {
                    current_buffer_input = bufferable_input.none;
                    start_dash();
                    return;
                }

                update_walk_idle_state();
            }
            void update_attack_state()
            {
                attack_timer -= delta_time;
                if (attack_timer <= 0f)
                {
                    current_combat_state = combat_state.none;
                }

                if (current_buffer_input == bufferable_input.dash && dash_cooldown_timer <= 0)
                {
                    update_movement_input();
                    current_buffer_input = bufferable_input.none;
                    start_dash();
                    return;
                }
            }
            void update_aim_state()
            {

                Vector2 to_cursor = global.input.scaled_mouse_world_position - position;

                current_direction_vector = to_cursor;

                if ((mouse_state.IsButtonDown(MouseButton.Right) && has_head) == false)
                {
                    current_combat_state = combat_state.none;
                }
                else if (mouse_state.WasButtonPressed(MouseButton.Left) && global.input.mouse_in_screen)
                {
                    throw_head();
                }
                
                update_movement_input();
                update_walk_idle_state();
            }
            void update_dash_state()
            {
                update_dash();
                update_walk_idle_state();
            }
            void update_die_state()
            {
                if (animation_player.is_finished)
                {
                    active = false;
                    global.signal.spawn_particle(particle_manager.particle_name.die_efx_white, position, high_layer: true);
                    global.signal.spawn_particle(particle_manager.particle_name.blood_on_ground_efx_white, position, high_layer: false);
                }
            }
            void update_change_scene_state()
            {
                i_frame_time = 1; // set i frame to 1 every time when change scene
            }

            void update_movement_input()
            {
                if (keyboard_state.IsKeyDown(Keys.D))
                {
                    input_direction += new Vector2(1, 0);
                }
                if (keyboard_state.IsKeyDown(Keys.A)) 
                {
                    input_direction += new Vector2(-1, 0);
                }
                if (keyboard_state.IsKeyDown(Keys.S)) 
                { 
                    input_direction += new Vector2(0, 1); 
                }
                if (keyboard_state.IsKeyDown(Keys.W)) 
                { 
                    input_direction += new Vector2(0, -1);
                }

                if (input_direction != Vector2.Zero)
                {
                    float effective_speed = current_combat_state == combat_state.aim ? speed * aim_speed_multiplier : speed;

                    input_direction = Vector2.Normalize(input_direction) * effective_speed;
                }

                if (current_combat_state != combat_state.aim  &&  input_direction != Vector2.Zero)
                {
                    current_direction_vector = input_direction;
                }
            }

            void update_walk_idle_state()
            {
                current_state = velocity.Length() > 10f ? state.walk : state.idle;
            }

            // ---------------- Melee ----------------

            void start_attack()
            {
                current_buffer_input = bufferable_input.none;
                current_combat_state = combat_state.attack;
                attack_timer = attack_duration;

                global.sound.play_sound(global.sound.sound_name.slash);

                combo_count++;
                combo_reset_timer = combo_reset_window;
                next_attack_timer = attack_input_delay; // เริ่มนับดีเลย์ทันทีที่ออกหมัด

                int hit_index = MathHelper.Clamp(combo_count - 1, 0, combo_hits.Count - 1); // กันเผื่อ max_combo กับ combo_hits.Count ไม่ตรงกัน
                melee_data hit_data = combo_hits[hit_index];

                Vector2 to_cursor = global.input.scaled_mouse_world_position - position;
                Vector2 aim_direction = to_cursor != Vector2.Zero ? Vector2.Normalize(to_cursor) : Vector2.UnitY;

                current_direction_vector = aim_direction; // ยังใช้ตัวนี้แค่สำหรับเลือก animation/sprite ทิศทาง ไม่เกี่ยวกับ hit detection แล้ว

                hit_data.spawn_melee_projectile(content, this, position, aim_direction);

                if (combo_count >= max_combo)
                {
                    melee_cooldown_timer = combo_cooldown_duration;
                    combo_count = 0; // เริ่มคอมโบใหม่ตั้งแต่ตอนนี้ ระหว่างนี้ cooldown จะบล็อกการโจมตีอยู่แล้ว
                }

                animation_player.play(has_head
                   ? animation_player.data.data[animation_player_player.animation_name.punch]
                   : animation_player.data.data[animation_player_player.animation_name.no_head_punch]);
            }

            

            // ---------------- Head throw / dash / pickup ----------------

            void throw_head()
            {
                has_head = false;

                projectile_head head = new projectile_head(content, position,run_data ,owner: this);
                Vector2 to_cursor = global.input.scaled_mouse_world_position - position;
                head.velocity = Vector2.Normalize(to_cursor) * head_throw_speed;

                global.sound.play_sound(global.sound.sound_name.throw_head);
                global.signal.spawn_projectile(head);
                head_projectile = head;

                i_frame_time = throw_head_i_frame_duration;
            }
            
            void update_dash()
            {
                dash_timer -= delta_time;
                if (dash_timer <= 0)  // dash end
                {
                    end_dash();
                }

                Vector2 dash_direction = current_direction_vector;
                if (dash_direction == Vector2.Zero)
                {
                    dash_direction = new  Vector2(0, 1);
                    Debug.WriteLine("error player update dash function dash_direction = vector 0,0  ");
                }
                velocity = Vector2.Normalize(dash_direction) * dash_speed; // ความเร็วคงที่พุ่งตรงเข้าหาหัว
                acceleration = Vector2.Zero;


                check_head_pickup();
            }
            void start_dash()
            {
                current_combat_state = combat_state.dash;
                dash_timer = dash_duration;
                max_velocity = MathF.Max(default_max_velocity, dash_speed); // เปิดเพดานความเร็วให้สูงพอสำหรับ dash

                i_frame_time += dash_i_frame_duration;
                fade_i_frame_visual_timer += dash_i_frame_duration;
            }
            void end_dash()
            {
                current_combat_state = combat_state.none;
                max_velocity = default_max_velocity; // คืนเพดานความเร็วปกติ
                velocity = Vector2.Zero; // หยุดนิ่งทันทีตอนยกเลิก กันพุ่งเลยไปแรงๆ ก่อนกลับสู่ physics ปกติ
                dash_cooldown_timer = dash_cooldown;
            }

            void reattach_head()
            {
                if (head_projectile != null)
                {
                    head_projectile.time_out(); // ลบตัวเองออกจาก scene และ collision world
                    head_projectile = null;
                }
                else
                {
                    Debug.WriteLine("player reattach_head function    ERROR    reattach head but there is no thrown head");
                }

                has_head = true;
            }

            void check_head_pickup()
            {
                if (head_projectile == null)  // not head projectile yet
                {
                    return;
                }
                if (head_projectile.pickup_lock_timer > 0f)  // ยังอยู่ในช่วงล็อก เก็บไม่ได้
                {
                    return;
                }

                Vector2 to_head = head_projectile.position - position;
                if (to_head.Length() <= pickup_radius  && head_projectile.is_resting)   // head in pickup_radius
                {
                    bool pickup_via_dash = current_combat_state == combat_state.dash;

                    reattach_head();
                    global.sound.play_sound(global.sound.sound_name.pick_up_head);

                    if (pickup_via_dash)
                    {
                        global.signal.screen_shake(0.6f);
                    }
                }
            }
        }
        public override void update_animation(float delta_time)
        {
            switch (current_combat_state)
            {
                case combat_state.die:
                    //  already play animaiton in die function   dont play any other animation while dying  Chess Battle Advanced
                    break;
                case combat_state.attack:
                    //  already play animaiton in punch function   dont play any other animation while attacking
                    break;
                case combat_state.dash:
                    //  already play animaiton in punch function   dont play any other animation while attacking
                    break;
                case combat_state.aim:   // body layer: เล่นท่า headless ตลอดช่วง aim
                    switch (current_state)
                    {
                        case state.walk:
                            animation_player.play(animation_player.data.data[animation_player_player.animation_name.no_head_walk]);
                            break;
                        case state.idle:
                            animation_player.play(animation_player.data.data[animation_player_player.animation_name.no_head_idle]);
                            break;
                    }

                    // overlay layer: takeoff_head เล่นซ้อนทับครั้งเดียวจบ
                    animation_player_2.update(delta_time, current_direction.ToString());
                    break;

                default: // none / aim ใช้ตรรกะเดียวกัน: เลือกตาม current_state (walk/idle)
                    switch (current_state)
                    {
                        case state.walk:
                            animation_player.play(has_head
                                ? animation_player.data.data[animation_player_player.animation_name.walk]
                                : animation_player.data.data[animation_player_player.animation_name.no_head_arm_walk]);
                            break;

                        case state.idle:
                            animation_player.play(has_head
                                ? animation_player.default_animation
                                : animation_player.data.data[animation_player_player.animation_name.no_head_arm_idle]);
                            break;
                    }
                    break;
            }
            base.update_animation(delta_time);
        }
        public override bool take_damage(int damage_taken, node damage_dealer = null)
        {
            if (i_frame_time > 0) { return false; }

            if (has_head)
            {
                Vector2 head_drop_direction = Vector2.Zero;
                if (damage_dealer is projectile projectile)
                {
                    head_drop_direction = position - projectile.position;
                }
                else if (damage_dealer is entity entity)
                {
                    head_drop_direction = position - entity.position;
                }

                if (head_drop_direction == Vector2.Zero)
                {
                    head_drop_direction = new Vector2(0, -200);
                }

                drop_head(head_drop_direction);

                i_frame_time = i_frame_duration; // เริ่ม i-frame ทันทีที่หัวหลุด
                global.signal.screen_shake(0.6f);
                return true;
            }

            if (base.take_damage(damage_taken) == false) { return false; }

            i_frame_time = i_frame_duration; // เริ่ม i-frame ตอนโดนดาเมจจริง
            global.signal.screen_shake(0.6f);
            run_data.hp_left = hp;

           
            


            return true;

            void drop_head(Vector2 head_drop_direction)  // when take damage
            {
                if (head_projectile != null) { return; }
                has_head = false;

                projectile_head head = new projectile_head(content, position, run_data, owner: this);
                head.velocity = Vector2.Normalize(head_drop_direction) * head_drop_speed ;
                head.has_bounced = true;
                head.shock_wave_enable = false;
                head.pickup_lock_timer = i_frame_duration; // ห้ามเก็บตลอดช่วง i-frame

                global.signal.spawn_projectile(head);
                head_projectile = head;
            }
        }
        public override void die()
        {
            if (! alive) { return; }
            alive = false;
            current_combat_state = combat_state.die;
            animation_player.play(animation_player.data.data[animation_player_player.animation_name.die]);

            run_data.hp_left = max_hp; // reset run_data hp to max
        }

        public override void Draw(SpriteBatch sprite_batch)
        {
            float blink_alpha = 1f;
            if (i_frame_time > 0f)
            {
                bool visible_phase = ((int)(i_frame_time / blink_interval)) % 2 == 0;
                blink_alpha = visible_phase ? 1f : blink_min_alpha;
            }

            if (current_combat_state == combat_state.die || current_combat_state == combat_state.change_scene)
            {
                base.Draw(sprite_batch, 1);   // not blinking when die or change scene
            }
            else if (fade_i_frame_visual_timer > 0 )  // still has i_frame from dash
            {
                base.Draw(sprite_batch, blink_min_alpha);
            }
            else
            {
                base.Draw(sprite_batch, blink_alpha);
            }
            
            if (current_combat_state == combat_state.aim)
            {
                animation_player_2.draw(sprite_batch, position, blink_alpha);
                
            }
            if (dash_cooldown_timer > 0)
            {
                int dash_cooldown_offset_height = -50;
                Point dash_cooldown_rectangle_size = new Point(60,5);
                float rectangle_ratio = (dash_cooldown_timer / dash_cooldown);

                rectangle_ratio = rectangle_ratio * rectangle_ratio * rectangle_ratio * rectangle_ratio;     // ease out quad  

                Point rectagle_top_left = new Point((int)(position.X - (dash_cooldown_rectangle_size.X / 2 * rectangle_ratio)), (int)(position.Y + dash_cooldown_offset_height));
                Rectangle dash_cooldown_rectangle = new Rectangle(rectagle_top_left,new Point((int)(dash_cooldown_rectangle_size.X * rectangle_ratio), dash_cooldown_rectangle_size.Y));

                sprite_batch.FillRectangle(dash_cooldown_rectangle, Color.White, 1);
            }
        }
        public class animation_player_player : animation_player_base       // custom animation for this class only
        {
            public enum animation_name { idle, walk, no_head_idle, no_head_walk, punch, no_head_punch, takeoff_head, no_head_arm_idle, no_head_arm_walk , die}

            public static readonly animation_data animation_data = new animation_data();
            public animation_player_player(ContentManager content) : base()
            {
                if (animation_data.data.Count == 0)
                {
                    load(content);
                }

                base.data = animation_data;

                default_animation = animation_data.data[animation_name.idle];
                current_animation = default_animation;
            }
            public void load(ContentManager content)
            {
                Texture2D idle_texture = content.Load<Texture2D>("assets/image/player/sprite_player_idle");
                animation idle_animation = new animation(idle_texture, frame_per_sec: 8);
                idle_animation.name = "player idle";
                animation_data.data.Add(animation_name.idle, idle_animation);

                Texture2D walk_texture = content.Load<Texture2D>("assets/image/player/sprite_player_walk");
                animation walk_animation = new animation(walk_texture, frame_per_sec: 15);
                walk_animation.name = "player walk";
                animation_data.data.Add(animation_name.walk, walk_animation);

                Texture2D no_head_idle_texture = content.Load<Texture2D>("assets/image/player/sprite_player_nohead_idle");
                animation no_head_idle_animation = new animation(no_head_idle_texture, frame_per_sec: 8);
                no_head_idle_animation.name = "player no_head_walk";
                animation_data.data.Add(animation_name.no_head_idle, no_head_idle_animation);

                Texture2D no_head_walk_texture = content.Load<Texture2D>("assets/image/player/sprite_player_nohead_walk");
                animation no_head_walk_animation = new animation(no_head_walk_texture, frame_per_sec: 15);
                no_head_walk_animation.name = "player no_head_walk";
                animation_data.data.Add(animation_name.no_head_walk, no_head_walk_animation);

                Texture2D no_head_arm_idle_texture = content.Load<Texture2D>("assets/image/player/sprite_player_noheadbutarm_Idle");
                animation no_head_arm_idle_animation = new animation(no_head_arm_idle_texture, frame_per_sec: 8);
                no_head_arm_idle_animation.name = "player no_head_arm_walk";
                animation_data.data.Add(animation_name.no_head_arm_idle, no_head_arm_idle_animation);

                Texture2D no_head_arm_walk_texture = content.Load<Texture2D>("assets/image/player/sprite_player_noheadbutarm_walk");
                animation no_head_arm_walk_animation = new animation(no_head_arm_walk_texture, frame_per_sec: 15);
                no_head_arm_walk_animation.name = "player no_head_arm_walk";
                animation_data.data.Add(animation_name.no_head_arm_walk, no_head_arm_walk_animation);

                Texture2D punch_texture = content.Load<Texture2D>("assets/image/player/sprite_player_punchattack");
                animation punch_animation = new animation(punch_texture, loop: false, frame_per_sec: 12); 
                punch_animation.name = "player punch";
                animation_data.data.Add(animation_name.punch, punch_animation);

                Texture2D no_head_punch_texture = content.Load<Texture2D>("assets/image/player/sprite_player_noheadbutarm_punchattack");
                animation no_head_punch_animation = new animation(no_head_punch_texture, loop: false, frame_per_sec: 12);
                no_head_punch_animation.name = "player no_head_punch";
                animation_data.data.Add(animation_name.no_head_punch, no_head_punch_animation);

                Texture2D takeoff_head_texture = content.Load<Texture2D>("assets/image/player/sprite_player_takeoffhead"); 
                animation takeoff_head_animation = new animation(takeoff_head_texture, frame_per_sec: 12 , loop: false);
                takeoff_head_animation.name = "player takeoff_head";
                animation_data.data.Add(animation_name.takeoff_head, takeoff_head_animation);

                Texture2D die_texture = content.Load<Texture2D>("assets/image/player/sprite_player_die");
                animation die_animation = new animation(die_texture, frame_per_sec: 12, loop: false , one_direction_sprite_format: true , one_diretion_sprite_format_frame_count: 13);
                die_animation.name = "player die";
                animation_data.data.Add(animation_name.die, die_animation);
            }
        }
    }

}
