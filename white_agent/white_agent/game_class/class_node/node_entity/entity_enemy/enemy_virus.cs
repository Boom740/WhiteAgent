using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Diagnostics;

namespace old_heart
{
    public class enemy_virus : enemy
    {
        public enum animation_name { idle, walk, dizzy, died, attack }


        // --- radii ---
        public float dangerous_radius = 200f;
        public float attack_radius = 40f;

        // --- patrol (square) ---
        private float patrol_time_wait = 1f;
        private float patrol_time_walk = 0.5f;
        private float current_patrol_time = 0f;

        public float attack_duration = 1f; // ~10 frame ที่ 60fps เป็น placeholder ไปก่อน
        public float attack_timer = 0f;
        public float hitbox_spawn_time = 0.5f;
        public float hitbox_spawn_timer = -1f;

        public float attack_cooldown = 2f;
        public float attack_cooldown_timer = 0f;
        public int attack_hit_frame = 8; // frame 9 แบบ 1-indexed = index 8 แบบ 0-indexed
        private bool hitbox_spawned_this_attack = false;

        public melee_data melee_data = new melee_data(damage: 1, lunge_speed: 100f, range: 40f, hitbox_lifetime: 0.3f, knockback_speed: 100f);

        public enemy_virus(ContentManager content_set, Vector2 position) : base(content_set, position, max_hp: 5 ,hit_box_radius: 15)
        {
            animation_player = new animation_player_virus(content_set);
            can_be_dizzied = false;
        }

        public override void Update(GameTime gameTime)
        {
            if (alive == false) return;
            float delta_time = (float)gameTime.ElapsedGameTime.TotalSeconds;

            update_cooldown();

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
                    break;

                case enemy_state.chase:
                    update_chase();
                    break;

                case enemy_state.attack:
                    update_attack();
                    break;

                case enemy_state.dizzy:
                    update_dizzy();
                    break;

                case enemy_state.died:
                    break;
            }

            base.Update(gameTime); // ให้ entity จัดการ velocity/position/animation/collision ตามปกติ

            void update_cooldown()
            {
                if (attack_cooldown_timer > 0f)
                {
                    attack_cooldown_timer -= delta_time;
                }
            }

            // ---------------- Normal ----------------

            void update_patrol()
            {
                if (current_patrol_time <= 0)
                {
                    int action_number = random.Next(1, 101);  // random 1 - 100     
                    if (action_number < 10) //  % to walk
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


                if (target == null) return;
                float distance = Vector2.Distance(position, target.position);
                if (distance <= dangerous_radius)
                {
                    state = enemy_state.chase;
                }
            }

            // ----------------- chase -------------------
            void update_chase()
            {
                if (target == null) return;
                float distance = Vector2.Distance(position, target.position);

                if (distance > dangerous_radius)
                {
                    state = enemy_state.normal;
                    return;
                }
                else if (distance <= attack_radius)
                {
                    if (attack_cooldown_timer <= 0)
                    {
                        start_attack();
                    }
                    acceleration = Vector2.Zero;
                }
                else
                {
                    acceleration = Vector2.Normalize(target.position - position) * speed;
                }

                current_direction_vector = target.position - position;  // loot at target
            }

            // ----------------- attack -------------------

            void update_attack()
            {
                attack_timer -= delta_time;

                if (hitbox_spawned_this_attack == false && animation_player.current_frame_index >= attack_hit_frame)
                {
                    hitbox_spawned_this_attack = true;
                    acceleration = Vector2.Zero;
                    melee_data.spawn_melee_projectile(content, this, position, current_direction_vector);
                }
               

                if (attack_timer <= 0f)
                {
                    state = enemy_state.normal;
                    attack_cooldown_timer = attack_cooldown;
                }
            }

            // ---------------- Dizzy ----------------

            void update_dizzy()
            {
                acceleration = Vector2.Zero;

                dizzy_timer_current -= delta_time;
                if (dizzy_timer_current <= 0f)
                {
                    state = enemy_state.normal;
                }
            }


            void start_attack()
            {
                state = enemy_state.attack;
                hitbox_spawned_this_attack = false;

                Vector2 aim_direction = Vector2.Normalize(target.position - position) * melee_data.lunge_speed;

                current_direction_vector = aim_direction;  // update direction to aim

                attack_timer = attack_duration;
                //hitbox_spawn_timer = hitbox_spawn_time;


                animation_player.play(animation_player.data.data[animation_name.attack]);
            }

        }
        
        public override void apply_knockback(Vector2 direction, float speed)
        {
            // ทำให้ virus ไม่โดน knockback 
        }
        public override void update_animation(float delta_time)
        {
            if (state == enemy_state.dizzy)
            {
                animation_player.play(animation_player.data.data[animation_name.dizzy]);
            }
            else if (state == enemy_state.attack)
            {

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
        public override void die()
        {
            global.signal.spawn_particle(particle_manager.particle_name.die_efx_red, position, high_layer: true);
            global.signal.spawn_particle(particle_manager.particle_name.blood_on_ground_efx_red, position, high_layer: false);
            base.die();
        }
        public class animation_player_virus : animation_player_base
        {
            public static readonly animation_data animation_data = new animation_data();

            public animation_player_virus(ContentManager content) : base()
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
                //Point placeholder_sprite_size = new Point(16, 16); // sprite sheet ของ Beast.png คือ 4x4 ช่อง ช่องละ 16x16

                // TODO: Beast.png เป็น placeholder แทน Leukemia ไปก่อน เปลี่ยน path เมื่อมีภาพจริงของ bacteria
                Texture2D placeholder_texture = content.Load<Texture2D>("assets/image/enemy/sprite_virus_idle"); //ใช้รูป player ไปก่อนเพราะ Beast ยังใส่ไม่ได้
                Texture2D placeholder_texture_2 = content.Load<Texture2D>("assets/image/enemy/sprite_virus_walk"); //ใช้รูป player ไปก่อนเพราะ Beast ยังใส่ไม่ได้

                animation idle_animation = new animation(placeholder_texture, frame_per_sec: 2, sprite_size: new Point(128, 128)); // (ใส่ , sprite_size: placeholder_sprite_size ไว้หลัง frame per sec  ถ้าใส่ beast ได้แล้ว)
                idle_animation.sprite_scale = new Vector2(1, 1);
                idle_animation.name = "bacteria idle";
                animation_data.data.Add(animation_name.idle, idle_animation);

                animation walk_animation = new animation(placeholder_texture_2, frame_per_sec: 8, sprite_size: new Point(128, 128));
                walk_animation.sprite_scale = new Vector2(1, 1);
                walk_animation.name = "bacteria walk";
                animation_data.data.Add(animation_name.walk, walk_animation);

                animation dizzy_animation = new animation(placeholder_texture, frame_per_sec: 2, sprite_size: new Point(128, 128));
                dizzy_animation.sprite_scale = new Vector2(1, 1);
                dizzy_animation.name = "bacteria dizzy";
                animation_data.data.Add(animation_name.dizzy, dizzy_animation);

                Texture2D punch_texture = content.Load<Texture2D>("assets/image/enemy/sprite_virus_attack");
                animation punch_animation = new animation(punch_texture, loop: false, frame_per_sec: 12, sprite_size: new Point(128, 128));
                punch_animation.name = "bacteria attack";
                animation_data.data.Add(animation_name.attack, punch_animation);
            }
        }
    }
}