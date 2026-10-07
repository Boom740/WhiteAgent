using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace old_heart
{
    public class enemy_mole : enemy
    {
        public enum animation_name { idle, walk, dizzy, died }

        // radius
        public float sight_radius = 420f;   // ระยะการมองเห็น
        public float attack_radius = 320f;  // ระยะยิง
        public float dangerous_rad = 160f;   // ระยะหนี
        public float safe_rad = 240f;       // ระยะหยุดหนี

        //ranged attack
        public float attack_cooldown = 1.5f;
        private float attack_cooldown_timer = 0f;

        public int shots_per_burst = 3;
        public float shot_interval = 0.3f;
        private int shots_fired_in_burst = 0;
        private float shot_timer = 0f;

        public float projectile_speed = 250f;
        public int projectile_damage = 1;
        public float projectile_knockback_speed = 150f;

        //frightened 
        public float frightened_exit_timer = 0f;
        private const float frightened_exit_delay = 0.5f;

        //patrol 
        private float patrol_time_wait = 1f;
        private float patrol_time_walk = 0.5f;
        private float current_patrol_time = 0f;

        public enemy_mole(ContentManager content_set, Vector2 position) : base(content_set, position, max_hp: 4, speed: 1200, hit_box_radius: 15)
        {
            animation_player = new animation_player_mole(content_set);
        }

        public override void Update(GameTime gameTime)
        {
            if (alive == false) return;
            float delta_time = (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (attack_cooldown_timer > 0f)
            {
                attack_cooldown_timer -= delta_time;
            }

            // shield regen 
            if (shield == false && state != enemy_state.died)
            {
                shield_timer_current -= delta_time;
                if (shield_timer_current <= 0f)
                {
                    shield = true;
                }
            }

            switch (state)
            {
                case enemy_state.normal:
                    update_patrol();
                    check_sight_distance();
                    break;

                case enemy_state.chase:
                    update_chase();
                    break;

                case enemy_state.attack:
                    update_attack(delta_time);
                    break;

                case enemy_state.frightened:
                    update_frightened(delta_time);
                    break;

                case enemy_state.dizzy:
                    update_dizzy(delta_time);
                    break;

                case enemy_state.died:
                    break;
            }

            base.Update(gameTime);

            // Normal 

            void update_patrol()
            {
                if (current_patrol_time <= 0)
                {
                    int action_number = random.Next(1, 101);
                    if (action_number < 10)
                    {
                        current_patrol_time = patrol_time_walk;
                        acceleration = Vector2.Rotate(Vector2.One, ((float)random.NextDouble() * (float)Math.PI * 2)) * speed;
                        current_direction_vector = acceleration;
                    }
                    else
                    {
                        current_patrol_time = patrol_time_wait;
                        acceleration = Vector2.Zero;
                    }
                }
                else
                {
                    current_patrol_time -= delta_time;
                }
            }

            void check_sight_distance()
            {
                if (target == null) return;
                float distance = Vector2.Distance(position, target.position);
                if (distance <= sight_radius)
                {
                    state = enemy_state.chase;
                }
            }

            // Chase

            void update_chase()
            {
                if (target == null) { state = enemy_state.normal; return; }
                float distance = Vector2.Distance(position, target.position);

                if (distance <= dangerous_rad)
                {
                    enter_frightened();
                    return;
                }
                if (distance <= attack_radius)
                {
                    enter_attack();
                    return;
                }
                if (distance > sight_radius)
                {
                    state = enemy_state.normal;
                    acceleration = Vector2.Zero;
                    return;
                }

                acceleration = Vector2.Normalize(target.position - position) * speed;
                current_direction_vector = acceleration;
            }

            //Attack 

            void enter_attack()
            {
                state = enemy_state.attack;
                shots_fired_in_burst = 0;
                shot_timer = 0f;
                acceleration = Vector2.Zero;
            }

            void update_attack(float dt)
            {
                if (target == null) { state = enemy_state.normal; return; }
                float distance = Vector2.Distance(position, target.position);

                if (distance <= dangerous_rad)
                {
                    enter_frightened();
                    return;
                }
                if (distance > attack_radius)
                {
                    state = enemy_state.chase;
                    return;
                }

                current_direction_vector = target.position - position;
                acceleration = Vector2.Zero;

                if (shots_fired_in_burst >= shots_per_burst)
                {
                    attack_cooldown_timer = attack_cooldown;
                    shots_fired_in_burst = 0;
                }

                if (attack_cooldown_timer > 0f) return;

                shot_timer -= dt;
                if (shot_timer <= 0f)
                {
                    fire_projectile();
                    shots_fired_in_burst++;
                    shot_timer = shot_interval;
                }
            }

            //Frightened

            void enter_frightened()
            {
                state = enemy_state.frightened;
                frightened_exit_timer = 0f;
            }

            void update_frightened(float delta_time)
            {
                if (target == null) return;

                Vector2 away_direction = position - target.position;
                float distance = away_direction.Length();
                away_direction = distance > 0.001f ? Vector2.Normalize(away_direction) : Vector2.UnitY;

                if (distance >= safe_rad)
                {
                    frightened_exit_timer += delta_time;
                    if (frightened_exit_timer >= frightened_exit_delay)
                    {
                        state = enemy_state.chase; 
                        frightened_exit_timer = 0f;
                        acceleration = Vector2.Zero;
                    }
                }
                else
                {
                    frightened_exit_timer = 0f;
                    acceleration = away_direction * speed;
                    current_direction_vector = away_direction; // หันหน้าตามทิศวิ่งหนี... จะ override ด้วยทิศเล็งด้านล่างแทน
                }

                // ยิงต่อระหว่างหนี โดยไม่สนใจ cooldown burst ของ attack state (ใช้ cooldown เดียวกันแต่ไม่ต้องยืนนิ่ง)
                if (shots_fired_in_burst >= shots_per_burst)
                {
                    attack_cooldown_timer = attack_cooldown;
                    shots_fired_in_burst = 0;
                }

                if (attack_cooldown_timer > 0f) return;

                shot_timer -= delta_time;
                if (shot_timer <= 0f)
                {
                    Vector2 aim_direction = target.position - position;
                    fire_projectile(aim_direction);
                    shots_fired_in_burst++;
                    shot_timer = shot_interval;
                }
            }

            // Shared fire logic 

            void fire_projectile(Vector2? override_direction = null)
            {
                Vector2 aim_direction = override_direction ?? (target.position - position);
                if (aim_direction == Vector2.Zero) aim_direction = current_direction_vector;

                projectile_leukemia_bullet shot = new projectile_leukemia_bullet(content, position, aim_direction, projectile_speed, projectile_damage, projectile_knockback_speed);
                shot.owner = this;
                global.signal.spawn_projectile(shot);
            }

            // Dizzy

            void update_dizzy(float delta_time)
            {
                acceleration = Vector2.Zero;

                dizzy_timer_current -= delta_time;
                if (dizzy_timer_current <= 0f)
                {
                    state = enemy_state.normal;
                }
            }
        }

        public override void die()
        {
            global.signal.spawn_particle(particle_manager.particle_name.die_efx_purple, position, high_layer: true);
            global.signal.spawn_particle(particle_manager.particle_name.blood_on_ground_efx_purple, position, high_layer: false);
            base.die();
        }

        public override void update_animation(float delta_time)
        {
            if (state == enemy_state.dizzy)
            {
                animation_player.play(animation_player.data.data[animation_name.dizzy]);
            }
            else if (velocity.Length() > 10f)
            {
                animation_player.play(animation_player.data.data[animation_name.walk]);
            }
            else
            {
                animation_player.play(animation_player.default_animation);
            }

            base.update_animation(delta_time);
        }

        public class animation_player_mole : animation_player_base
        {
            public static readonly animation_data animation_data = new animation_data();

            public animation_player_mole(ContentManager content) : base()
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

                texture = content.Load<Texture2D>("assets/image/enemy/sprite_leukemia_idle");
                animation idle_animation = new animation(texture, frame_per_sec: 8, sprite_size: new Point(64, 64));
                idle_animation.sprite_scale = new Vector2(1, 1);
                idle_animation.sprite_origin = new Vector2(32, 57);
                idle_animation.name = "mole idle";
                animation_data.data.Add(animation_name.idle, idle_animation);

                texture = content.Load<Texture2D>("assets/image/enemy/sprite_leukemia_Walk");
                animation walk_animation = new animation(texture, frame_per_sec: 8, sprite_size: new Point(64, 64));
                walk_animation.sprite_scale = new Vector2(1, 1);
                walk_animation.sprite_origin = new Vector2(32, 57);
                walk_animation.name = "mole walk";
                animation_data.data.Add(animation_name.walk, walk_animation);

                texture = content.Load<Texture2D>("assets/image/enemy/sprite_leukemia_dizzy");
                animation dizzy_animation = new animation(texture, frame_per_sec: 12, sprite_size: new Point(64, 64));
                dizzy_animation.sprite_scale = new Vector2(1, 1);
                dizzy_animation.sprite_origin = new Vector2(32, 57);
                dizzy_animation.name = "mole dizzy";
                animation_data.data.Add(animation_name.dizzy, dizzy_animation);
            }
        }
    }
}