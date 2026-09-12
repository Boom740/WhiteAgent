using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended.Input;
using System;
using System.Collections.Generic;
using System.Diagnostics; // Required for Keyboard input

namespace old_heart
{
    public class player : entity
    {
        public Vector2 input_direction = Vector2.Zero;
        public enum state { idle, walk }
        public enum combat_state { none, attack, aim , dash}
        public state current_state = state.idle;
        public combat_state current_combat_state = combat_state.none;

        // --- combat: melee ---
        public float attack_duration = 20f /60f; // ~10 frame ที่ 60fps เป็น placeholder ไปก่อน
        public float attack_timer = 0f;

        public List<melee_combo_hit_data> combo_hits = new List<melee_combo_hit_data>
         {
          new melee_combo_hit_data(damage: 1, lunge_speed: 150f, range: 40f, hitbox_lifetime: 0.07f, knockback_speed: 150f), // hit 1
          new melee_combo_hit_data(damage: 1, lunge_speed: 150f, range: 40f, hitbox_lifetime: 0.07f, knockback_speed: 150f), // hit 2
          new melee_combo_hit_data(damage: 1, lunge_speed: 150f, range: 40f, hitbox_lifetime: 0.07f, knockback_speed: 150f), // hit 3
          new melee_combo_hit_data(damage: 2, lunge_speed: 850f, range: 55f, hitbox_lifetime: 0.08f, knockback_speed: 300f), // hit 4 (finisher)
         };
        // --- combat: melee combo ---
        public int combo_count = 0;
        public int max_combo = 4;
        public float combo_reset_window = 1f;   // เว้นช่วงกดเกินเท่านี้ = คอมโบหลุด
        public float combo_reset_timer = 0f;
        public float combo_cooldown_duration = 0.5f; // คูลดาวน์หลังคอมโบครบ 4
        public float melee_cooldown_timer = 0f;

        public float attack_input_delay = 0.15f; // ดีเลย์ขั้นต่ำระหว่างแต่ละ hit กันคลิกรัวเกินจังหวะ
        private float next_attack_timer = 0f;

        // --- combat: head throw ---
        public bool has_head = true;
        public float head_throw_speed = 1200f;
        public float pickup_radius = 24f;
        private head_projectile thrown_head;

        // --- aim  ---
        public float aim_speed_multiplier = 0.2f;

        // --- dash (Space) ---
        public float dash_speed = 1600f;
        public float dash_timeout = 2f; // ยกเลิก dash ถ้าไปไม่ถึงภายในเวลานี้
        private float dash_timer = 0f;

        private float default_max_velocity;
        // --- headless wobble ---
        public float headless_wobble_max_degrees = 25f;
        private Random rng = new Random();
        public player(ContentManager content, Vector2 position) : base(content, max_hp: 4, position, speed: 5000)
        {
            animation_player = new animation_player_player(content);
            ground_friction = 10f;
            max_velocity = 300;
            default_max_velocity = max_velocity;


        }
        public override void Update(GameTime gameTime)
        {
            if (alive == false) return;
            float delta_time = (float)gameTime.ElapsedGameTime.TotalSeconds;

            KeyboardStateExtended keyboard_state = global.input.keyboard_state;
            MouseStateExtended mouse_state = global.input.mouse_state; // TODO: เช็คว่าชื่อ property ตรงกับของจริงในโปรเจกต์ไหม
            input_direction = Vector2.Zero;

            update_cooldown();


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

            }

            //  local functions: state handlers (เรียกจาก switch ด้านบน) 
            void update_free_state()
            {
                if (mouse_state.WasButtonPressed(MouseButton.Left))     // attack
                {
                    if (melee_cooldown_timer <= 0 && next_attack_timer <= 0f)
                    {
                        start_attack();
                    }
                }
                else if (mouse_state.IsButtonDown(MouseButton.Right) && has_head)
                {
                    current_combat_state = combat_state.aim;
                }// --- space: dash เข้าหาหัว ---
                else if (keyboard_state.WasKeyPressed(Keys.Space) && has_head == false && thrown_head != null)
                {
                    current_combat_state = combat_state.dash;
                    max_velocity = MathF.Max(default_max_velocity, dash_speed); // เปิดเพดานความเร็วให้สูงพอสำหรับ dash

                }
                update_movement_input();
                update_walk_idle_state();
            }
            void update_attack_state()
            {
                attack_timer -= delta_time;
                if (attack_timer <= 0f)
                {
                    current_combat_state = combat_state.none;
                }
            }
            void update_aim_state()
            {

                Vector2 to_cursor = global.input.scaled_mouse_world_position - position;
                current_direction = get_cardinal_direction(to_cursor);
                direction_locked = true;

                if ((mouse_state.IsButtonDown(MouseButton.Right) && has_head) == false)
                {
                    current_combat_state = combat_state.none;
                    direction_locked = false;
                }
                else if (mouse_state.WasButtonPressed(MouseButton.Left))
                {
                    throw_head();
                }
                
                update_movement_input();
                update_walk_idle_state();
            }
            void update_dash_state()
            {
                update_dash();
            }


            void update_movement_input()
            {

                if (keyboard_state.IsKeyDown(Keys.D)) input_direction += new Vector2(1, 0);
                if (keyboard_state.IsKeyDown(Keys.A)) input_direction += new Vector2(-1, 0);
                if (keyboard_state.IsKeyDown(Keys.S)) input_direction += new Vector2(0, 1);
                if (keyboard_state.IsKeyDown(Keys.W)) input_direction += new Vector2(0, -1);

                if (input_direction != Vector2.Zero)
                {
                    float effective_speed = current_combat_state == combat_state.aim ? speed * aim_speed_multiplier : speed;

                    if (has_head == false)
                    {
                        float wobble_angle = MathHelper.ToRadians((float)(rng.NextDouble() * 2 - 1) * headless_wobble_max_degrees);
                        input_direction = Vector2.Transform(input_direction, Matrix.CreateRotationZ(wobble_angle));
                    }

                    input_direction = Vector2.Normalize(input_direction) * effective_speed;
                }

            }

            void update_walk_idle_state()
            {
                current_state = velocity.Length() > 10f ? state.walk : state.idle;
            }

            // ---------------- Melee ----------------

            void start_attack()
            {
                current_combat_state = combat_state.attack;
                attack_timer = attack_duration;
                velocity = Vector2.Zero; // หยุดนิ่งทันทีตอนเริ่มโจมตี

                combo_count++;
                combo_reset_timer = combo_reset_window;
                next_attack_timer = attack_input_delay; // เริ่มนับดีเลย์ทันทีที่ออกหมัด

                int hit_index = MathHelper.Clamp(combo_count - 1, 0, combo_hits.Count - 1); // กันเผื่อ max_combo กับ combo_hits.Count ไม่ตรงกัน
                melee_combo_hit_data hit_data = combo_hits[hit_index];

                Vector2 to_cursor = global.input.scaled_mouse_world_position - position;
                Vector2 aim_direction = to_cursor != Vector2.Zero ? Vector2.Normalize(to_cursor) : Vector2.UnitY;

                velocity = aim_direction * hit_data.lunge_speed;

                current_direction = get_cardinal_direction(aim_direction); // ยังใช้ตัวนี้แค่สำหรับเลือก animation/sprite ทิศทาง ไม่เกี่ยวกับ hit detection แล้ว

                melee_projectile punch = new melee_projectile(content, position, aim_direction, hit_data.range, hit_data.hitbox_lifetime, hit_data.damage);
                punch.owner = this;
                punch.knockback_speed = hit_data.knockback_speed; // set หลังสร้าง เพราะ constructor เดิมไม่รับ knockback_speed
                global.signal.spawn_projectile(punch);
                if (combo_count >= max_combo)
                {
                    melee_cooldown_timer = combo_cooldown_duration;
                    combo_count = 0; // เริ่มคอมโบใหม่ตั้งแต่ตอนนี้ ระหว่างนี้ cooldown จะบล็อกการโจมตีอยู่แล้ว
                }
            }

            direction get_cardinal_direction(Vector2 v)
            {
                float abs_x = MathF.Abs(v.X);
                float abs_y = MathF.Abs(v.Y);
                if (abs_x > abs_y)
                    return v.X > 0 ? direction.right : direction.left;
                else
                    return v.Y > 0 ? direction.down : direction.up;
            }

            // ---------------- Head throw / dash / pickup ----------------

            void throw_head()
            {
                has_head = false;

                head_projectile head = new head_projectile(content, position);
                head.owner = this;
                Vector2 to_cursor = global.input.scaled_mouse_world_position - position;
                head.velocity = Vector2.Normalize(to_cursor) * head_throw_speed;

                global.signal.spawn_projectile(head);
                thrown_head = head;
            }

            void update_dash()
            {
                dash_timer += delta_time;

                if (thrown_head == null ||   dash_timer >= dash_timeout)
                {
                    cancel_dash();
                }

                Vector2 to_head = thrown_head.position - position;

                if (to_head.Length() <= pickup_radius)   // head in pickup_radius
                {
                    reattach_head();
                }
                else    // head NOT in pickup_radius
                {
                    velocity = Vector2.Normalize(to_head) * dash_speed; // ความเร็วคงที่พุ่งตรงเข้าหาหัว
                    acceleration = Vector2.Zero;
                }


                void cancel_dash()
                {
                    current_combat_state = combat_state.none;
                    max_velocity = default_max_velocity; // คืนเพดานความเร็วปกติ
                    velocity = Vector2.Zero; // หยุดนิ่งทันทีตอนยกเลิก กันพุ่งเลยไปแรงๆ ก่อนกลับสู่ physics ปกติ
                    dash_timer = 0f;
                }

                void reattach_head()
                {
                    if (thrown_head != null)
                    {
                        thrown_head.time_out(); // ลบตัวเองออกจาก scene และ collision world
                        thrown_head = null;
                    }
                    else
                    {
                        Debug.WriteLine("player reattach_head function    ERROR    reattach head but there is no thrown head");
                    }

                    has_head = true;
                    cancel_dash();
                }
            }

        }
        public override void update_animation(float delta_time)
        {
            switch (current_combat_state)
            {
                case combat_state.attack:
                    animation_player.play(has_head
                        ? animation_player.data.data[animation_player_player.animation_name.punch]
                        : animation_player.data.data[animation_player_player.animation_name.no_head_punch]);
                    break;

                default: // none / aim ใช้ตรรกะเดียวกัน: เลือกตาม current_state (walk/idle)
                    switch (current_state)
                    {
                        case state.walk:
                            animation_player.play(has_head
                                ? animation_player.data.data[animation_player_player.animation_name.walk]
                                : animation_player.data.data[animation_player_player.animation_name.no_head_walk]);
                            break;

                        case state.idle:
                            animation_player.play(has_head
                                ? animation_player.default_animation
                                : animation_player.data.data[animation_player_player.animation_name.no_head_idle]);
                            break;
                    }
                    break;
            }
            base.update_animation(delta_time);
        }
        public override void Draw(SpriteBatch sprite_batch)
        {
            base.Draw(sprite_batch);
        }
        public class animation_player_player : animation_player_base       // custom animation for this class only
        {
            public enum animation_name { idle, walk, no_head_idle, no_head_walk, punch, no_head_punch }

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
                animation idle_animation = new animation(idle_texture, frame_per_sec: 12);
                idle_animation.name = "player idle";
                animation_data.data.Add(animation_name.idle, idle_animation);

                Texture2D walk_texture = content.Load<Texture2D>("assets/image/player/sprite_player_walk");
                animation walk_animation = new animation(walk_texture, frame_per_sec: 15);
                walk_animation.name = "player walk";
                animation_data.data.Add(animation_name.walk, walk_animation);

                Texture2D no_head_idle_texture = content.Load<Texture2D>("assets/image/player/sprite_player_nohead_idle");
                animation no_head_idle_animation = new animation(no_head_idle_texture, frame_per_sec: 12);
                no_head_idle_animation.name = "player no_head_walk";
                animation_data.data.Add(animation_name.no_head_idle, no_head_idle_animation);

                Texture2D no_head_walk_texture = content.Load<Texture2D>("assets/image/player/sprite_player_nohead_walk");
                animation no_head_walk_animation = new animation(no_head_walk_texture, frame_per_sec: 15);
                no_head_walk_animation.name = "player no_head_walk";
                animation_data.data.Add(animation_name.no_head_walk, no_head_walk_animation);

                Texture2D punch_texture = content.Load<Texture2D>("assets/image/player/sprite_player_punchattack");
                animation punch_animation = new animation(punch_texture, loop: false, frame_per_sec: 12); 
                punch_animation.name = "player punch";
                animation_data.data.Add(animation_name.punch, punch_animation);

                Texture2D no_head_punch_texture = content.Load<Texture2D>("assets/image/player/sprite_player_noheadbutarm_punchattack");
                animation no_head_punch_animation = new animation(no_head_punch_texture, loop: false, frame_per_sec: 12);
                no_head_punch_animation.name = "player no_head_punch";
                animation_data.data.Add(animation_name.no_head_punch, no_head_punch_animation);
            }
        }
    }

}
