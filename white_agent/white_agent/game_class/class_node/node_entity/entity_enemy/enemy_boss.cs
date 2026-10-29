using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace old_heart
{
    public class enemy_boss : enemy
    {
        public enum animation_name { idle , attack , tired , dizzy, died , intro , roar , angry , angry_idle }

        // --- summon ---
        public List<enemy> summoned_enemy_list = new List<enemy> { };
        public Vector2 summoned_enemy_spawn_offset = new Vector2 (0,67);
        public bool summoned_enemy = false;
        public float summon_time = 1f;
        public float summon_timer = 0f;

        //ranged attack
        public float attack_cooldown = 5f;
        private float attack_cooldown_timer = 0f;

        public int shots_burst_count = 0;  // number of shot shoot at same time
        public int shots_per_burst = 0;  // number of time enemy will shoot
        private int shots_fired_in_burst = 0;
        public float bullet_spread_angle = MathHelper.ToRadians(30);   // 10 degree

        public bool angry = false;
        public float idle_wait_time = 1f; // stand still before summon 
        public float idle_wait_timer = 0;

        public float roar_screen_shake_interval = 0.3f;  // roar
        public float roar_screen_shake_timer = 0;

        public float die_screen_shake_interval = 0.67f;  // die
        public float die_screen_shake_timer = 0;
        public float die_animation_timer = 0;   // for logic after die   time
        public enemy_boss(ContentManager content_set, Vector2 position) : base(content_set, position, max_hp: 3 , speed: 0, hit_box_radius: 67)
        {
            animation_player = new animation_player_boss(content_set);

            entity_push_enable = false;
            //animation_player_shield.visible = false;

            shadow_color = Color.Transparent;

            drop_count_min = 67;
            drop_count_max = 67;

            dizzy_timer = 2;
            attack_cooldown_timer = attack_cooldown;

            state = enemy_state.intro;
            animation_player.play(animation_player.data.data[animation_name.intro]);
        }
        public override void Update(GameTime gameTime)
        {
            float delta_time = (float)gameTime.ElapsedGameTime.TotalSeconds;

            summoned_enemy_list.RemoveAll(enemy => enemy.alive == false);     // clear dead minion

            switch (state)
            {
                case enemy_state.intro:
                    update_intro(delta_time);
                    break;
                case enemy_state.normal:
                    update_idle(delta_time);
                    break;
                case enemy_state.roar:
                    update_roar(delta_time);
                    break;
                case enemy_state.attack:
                    update_attack(delta_time);
                    break;
                case enemy_state.summoning:
                    update_summoning(delta_time);
                    break;
                case enemy_state.dizzy:
                    update_dizzy(delta_time);
                    break;
                case enemy_state.died:
                    update_died(delta_time);
                    break;
            }

            base.Update(gameTime); // ให้ entity จัดการ velocity/position/animation/collision ตามปกติ

            void update_intro(float delta_time)
            {
                if (animation_player.is_finished)
                {
                    enter_normal();
                }
            }
            void update_roar(float delta_time)
            {
                if (roar_screen_shake_timer > 0)
                {
                    roar_screen_shake_timer -= delta_time;
                }
                else
                {
                    global.signal.screen_shake(0.3f);
                    roar_screen_shake_timer = roar_screen_shake_interval;
                }

                if (animation_player.is_finished)
                {
                    state = enemy_state.tired;
                }
            }
            // ---------------- Normal ----------------
            void update_idle(float delta_time)
            {
                if (idle_wait_timer < idle_wait_time)
                {
                    idle_wait_timer += delta_time;
                    return;
                }
                if (summoned_enemy_list.Count <= 0)
                {
                    if (summoned_enemy == false)
                    {
                        start_summoning();
                    }
                    else
                    {
                        enter_roar();
                    }
                    return;
                }

                if (attack_cooldown_timer > 0)
                {
                    attack_cooldown_timer -= delta_time;
                }
                else
                {
                    enter_attack();
                }
            }
            void update_attack(float delta_time)
            {
                if (target == null) { enter_normal() ; return; }

                current_direction_vector = target.position - position;


                if (animation_player.is_finished)
                {
                    float bullet_spread_sum = bullet_spread_angle * (shots_burst_count-1);

                    for(int i = 0; i < shots_burst_count; i++)
                    {
                        Vector2 bullet_direction = current_direction_vector;
                        float rotate = (bullet_spread_sum / 2) - (i * bullet_spread_angle);
                        bullet_direction.Rotate(rotate);
                        fire_projectile(bullet_direction);
                    }
                    shots_fired_in_burst++;



                    if (shots_fired_in_burst >= shots_per_burst) // finish shooting
                    {
                        attack_cooldown_timer = attack_cooldown;
                        shots_fired_in_burst = 0;
                        enter_normal();
                        return;
                    }
                    else
                    {
                        animation_player.play(animation_player.data.data[animation_name.attack]);
                    }
                }


                void fire_projectile(Vector2? override_direction = null)
                {
                    Vector2 aim_direction = override_direction ?? (target.position - position);
                    if (aim_direction == Vector2.Zero) aim_direction = current_direction_vector;

                    projectile_enemy_bullet shot = new projectile_enemy_bullet(content, position, aim_direction, projectile_speed : 150f, damage:1, knockback_speed:30f , lifetime:3f);
                    shot.owner = this;
                    global.signal.spawn_projectile(shot);
                }
            }
            void update_summoning(float delta_time)
            {
                if (animation_player.is_finished == false)
                {
                    return;
                }

                int stage = 4 - hp;
                 
                shots_burst_count++;
                shots_per_burst++;

                if (stage == 1)
                {
                    spawn_enemy(new enemy_bacteria(content, position));
                }
                else if (stage == 2)
                {
                    spawn_enemy(new enemy_bacteria(content, position));
                    spawn_enemy(new enemy_bacteria(content, position));
                }
                else if (stage == 3)
                {
                    spawn_enemy(new enemy_bacteria(content, position)); 
                    spawn_enemy(new enemy_bacteria(content, position));
                    spawn_enemy(new enemy_bacteria(content, position));
                }

                enter_normal();
            }
            // ---------------- Dizzy ----------------

            void update_dizzy(float delta_time)
            {
                if (!shield) { return; }
                dizzy_timer_current -= delta_time;

                if (dizzy_timer_current <= 0)
                {
                    enter_normal();
                    attack_cooldown_timer = attack_cooldown;
                    summon_timer = summon_time;
                    summoned_enemy = false;
                }
            }

            void update_died(float delta_time)
            {
                if (die_screen_shake_timer > 0f)
                {
                    die_screen_shake_timer -= delta_time;
                }
                else
                {
                    global.signal.screen_shake(1.0f);
                    die_screen_shake_timer = die_screen_shake_interval;
                }

                die_animation_timer += delta_time;
                if (die_animation_timer > 3)
                {
                    die_screen_shake_interval = 0.3f;
                }
                if (animation_player.is_finished)
                {
                    global.signal.spawn_particle(particle_manager.particle_name.die_efx_green, position, high_layer: true);
                    global.signal.spawn_particle(particle_manager.particle_name.blood_on_ground_efx_green, position, high_layer: false);
                    global.sound.play_sound(global.sound.sound_name.enemy_die);
                    global.signal.screen_shake(1.0f);
                    drop_items(drop_speed: 670);
                    active = false;
                }
            }

            void enter_normal()
            {
                state = enemy_state.normal;
                idle_wait_timer = 0;
            }
            void enter_roar()
            {
                state = enemy_state.roar;
                animation_player.play(animation_player.data.data[animation_name.roar]);
                roar_screen_shake_timer = 0;
            }
            void enter_attack()
            {
                state = enemy_state.attack;
                animation_player.play(animation_player.data.data[animation_name.attack]);
                shots_fired_in_burst = 0;
            }
            void start_summoning()
            {
                summoned_enemy = true;
                state = enemy_state.summoning;

                animation_player.play(animation_player.data.data[animation_name.angry]);
                angry = true;
            }
        }

        public override void enter_dizzy(bool break_shield = true , float dizzy_time_set = 0)
        {
            if (state != enemy_state.tired) { return; }  // cant dizzy if not tired
            if (! break_shield) { return; } // dont dizzy if not break shield

            base.enter_dizzy(break_shield : true);
        }

        public override void apply_knockback(Vector2 direction, float speed)
        {
            // no knock back 
        }
        public override bool take_damage(int damage_taken, node damage_dealer = null)
        {
            bool took_damage = base.take_damage(1 , damage_dealer);
            if ( ! took_damage ) { return false; }
            if (! alive) { return true; }

            global.signal.screen_shake(1.0f);
            shield = true;
            angry = false;

            return took_damage; 
        }
        void spawn_enemy(enemy new_enemy)
        {
            new_enemy.drop_count_min = 0;
            new_enemy.drop_count_max = 0;
            new_enemy.position += summoned_enemy_spawn_offset;
            global.signal.spawn_entity(new_enemy);
            summoned_enemy_list.Add(new_enemy);
        }
        public override void die()
        {
            if (!alive) { return; }
            state = enemy_state.died;
            animation_player.play(animation_player.data.data[animation_name.died]);
            alive = false;
            shield = false;
        }

        public override void update_animation(float delta_time)
        {
            if (state == enemy_state.died)
            {
                //
            }
            else if (state == enemy_state.attack)
            {
                //
            }
            else if (state == enemy_state.intro)
            {
                //
            }
            else if (state == enemy_state.summoning)
            {
                //
            }
            else if (state == enemy_state.roar)
            {
                //
            }
            else if (state == enemy_state.tired)
            {
                animation_player.play(animation_player.data.data[animation_name.tired]);
            }
            else if(state == enemy_state.dizzy)
            {
                animation_player.play(animation_player.data.data[animation_name.dizzy]);
            }
            else
            {
                if (angry)
                {
                    animation_player.play(animation_player.data.data[animation_name.angry_idle]);
                }
                else
                {
                    animation_player.play(animation_player.default_animation);
                }
            }

            base.update_animation(delta_time);
        }



        public class animation_player_boss : animation_player_base
        {
            public static readonly animation_data animation_data = new animation_data();

            public animation_player_boss(ContentManager content) : base()
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
                Texture2D texture;

                texture = content.Load<Texture2D>("assets/image/enemy/sprite_boss_idle");
                animation idle_animation = new animation(texture, frame_per_sec: 12 ,sprite_size: new Point(256,256) , one_direction_sprite_format: true , one_diretion_sprite_format_frame_count:15);
                idle_animation.sprite_origin = new Vector2(128,201.5f);
                idle_animation.name = "boss idle";
                animation_data.data.Add(animation_name.idle, idle_animation);

                texture = content.Load<Texture2D>("assets/image/enemy/sprite_boss_angryidle");
                animation angry_idle_animation = new animation(texture, frame_per_sec: 12, sprite_size: new Point(256, 256), one_direction_sprite_format: true, one_diretion_sprite_format_frame_count: 15);
                angry_idle_animation.sprite_origin = new Vector2(128, 201.5f);
                angry_idle_animation.name = "boss angry_idle";
                animation_data.data.Add(animation_name.angry_idle, angry_idle_animation);

                texture = content.Load<Texture2D>("assets/image/enemy/sprite_boss_shoot");
                animation shoot_animation = new animation(texture, frame_per_sec: 10 ,sprite_size: new Point(256, 256), one_direction_sprite_format: true, one_diretion_sprite_format_frame_count: 4, loop: false);
                shoot_animation.sprite_origin = new Vector2(128,201.5f);
                shoot_animation.name = "boss attack";
                animation_data.data.Add(animation_name.attack, shoot_animation);

                texture = content.Load<Texture2D>("assets/image/enemy/sprite_boss_tired");
                animation tired_animation = new animation(texture, frame_per_sec: 10, sprite_size: new Point(256, 256), one_direction_sprite_format: true, one_diretion_sprite_format_frame_count: 8);
                tired_animation.sprite_origin = new Vector2(128,201.5f);
                tired_animation.name = "boss tired"; 
                animation_data.data.Add(animation_name.tired, tired_animation);

                texture = content.Load<Texture2D>("assets/image/enemy/sprite_boss_dizzy");
                animation dizzy_animation = new animation(texture, frame_per_sec: 12, sprite_size: new Point(256, 256), one_direction_sprite_format: true, one_diretion_sprite_format_frame_count: 8);
                dizzy_animation.sprite_origin = new Vector2(128,201.5f);
                dizzy_animation.name = "boss dizzy";
                animation_data.data.Add(animation_name.dizzy, dizzy_animation);

                texture = content.Load<Texture2D>("assets/image/enemy/sprite_boss_angry");
                animation angry_animation = new animation(texture, frame_per_sec: 12, sprite_size: new Point(256, 256), one_direction_sprite_format: true, one_diretion_sprite_format_frame_count: 12 , loop:false);
                angry_animation.sprite_origin = new Vector2(128,201.5f);
                angry_animation.name = "boss angry";
                animation_data.data.Add(animation_name.angry, angry_animation);

                texture = content.Load<Texture2D>("assets/image/enemy/sprite_boss_intro");
                animation intro_animation = new animation(texture, frame_per_sec: 10, sprite_size: new Point(256, 256), one_direction_sprite_format: true, one_diretion_sprite_format_frame_count: 15, loop: false);
                intro_animation.sprite_origin = new Vector2(128,201.5f);
                intro_animation.name = "boss intro";
                animation_data.data.Add(animation_name.intro, intro_animation);

                texture = content.Load<Texture2D>("assets/image/enemy/sprite_boss_die");
                animation die_animation = new animation(texture, frame_per_sec: 12, sprite_size: new Point(256, 256), one_direction_sprite_format: true, one_diretion_sprite_format_frame_count: 43, loop: false);
                die_animation.sprite_origin = new Vector2(128, 201.5f);
                die_animation.name = "boss die";
                animation_data.data.Add(animation_name.died, die_animation);

                texture = content.Load<Texture2D>("assets/image/enemy/sprite_boss_roar");
                animation roar_animation = new animation(texture, frame_per_sec: 12, sprite_size: new Point(256, 256), one_direction_sprite_format: true, one_diretion_sprite_format_frame_count: 30, loop: false);
                roar_animation.sprite_origin = new Vector2(128, 201.5f);
                roar_animation.name = "boss roar";
                animation_data.data.Add(animation_name.roar, roar_animation);
            }
        }
    }
}