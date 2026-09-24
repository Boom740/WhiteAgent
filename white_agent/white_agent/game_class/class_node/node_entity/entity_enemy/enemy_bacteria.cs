using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection.PortableExecutable;

namespace old_heart
{
    public class enemy_bacteria : enemy
    {
        public enum animation_name { idle, walk, dizzy, died, attack }


        // --- radii ---
        public float dangerous_radius = 300f;
        public float attack_radius = 160f;

        // --- patrol (square) ---
        private float patrol_time_wait = 1f;
        private float patrol_time_walk = 0.5f;
        private float current_patrol_time = 0f;

        public float attack_duration = 1.3f;
        public float attack_timer = 0f;

        public float attack_cooldown = 1.5f;
        public float attack_cooldown_timer = 0f;

        public float dash_timer = 0f;
        public int dash_animation_frame_index = 5;
        private HashSet<entity> hit_entity = new HashSet<entity>(); // กันโดนดาเมจซ้ำจาก swing เดียวกัน
        public melee_data melee_data = new melee_data(damage: 1, lunge_speed: 1500f, range: 150f, hitbox_lifetime: 0.5f, knockback_speed: 500f);

        public enemy_bacteria(ContentManager content_set, Vector2 position) : base(content_set, position, max_hp: 8 , hit_box_radius: 15)
        {
            animation_player = new animation_player_bacteria(content_set);
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

                case enemy_state.dash:
                    update_dash();
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

                if (animation_player.current_animation == animation_player_bacteria.animation_data.data[animation_name.attack] && animation_player.current_frame_index == dash_animation_frame_index)
                {
                    start_dash();
                }
                
                if (attack_timer <= 0f)
                {
                    state = enemy_state.normal;
                    attack_cooldown_timer = attack_cooldown;
                }
            }
            
            void update_dash()
            {
                dash_timer -= delta_time;

                velocity = melee_data.lunge_speed * current_direction_vector;

                if (dash_timer <= 0f)
                {
                    state = enemy_state.attack;
                    velocity = Vector2.Zero;
                    animation_player.pause = false;
                    animation_player.current_frame_index = dash_animation_frame_index + 1;
                }
            }

            void start_dash()
            {
                state = enemy_state.dash;
                animation_player.pause = true;
                animation_player.current_frame_index = dash_animation_frame_index;
                dash_timer = melee_data.hitbox_lifetime;
                hit_entity.Clear();

                acceleration = Vector2.Zero;
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

                Vector2 aim_direction = Vector2.Normalize(target.position - position) * melee_data.lunge_speed;

                current_direction_vector = aim_direction;  // update direction to aim

                attack_timer = attack_duration;


                animation_player.play(animation_player.data.data[animation_name.attack]);
            }

        }
        public override void collide_entity(entity entity)
        {
            if (alive == false) return;
            if (state != enemy_state.dash) return; // ตอนติด dizzyไม่ระเบิดใส่ผู้เล่นตอนโดนชน

            if (entity is player && entity.alive && hit_entity.Contains(entity) == false)
            {
                hit_entity.Add(entity);

                bool deal_damage = entity.take_damage(melee_data.damage, damage_dealer: this);
                if (deal_damage == false) { return; }
                entity.apply_knockback(velocity,melee_data.knockback_speed);
            }
        }

        public override void enter_dizzy()
        {
            if (state == enemy_state.dash) { return; }
            base.enter_dizzy();
        }
        public override void die()
        {
            global.signal.spawn_particle(particle_manager.particle_name.die_efx_green, position, high_layer: true);
            global.signal.spawn_particle(particle_manager.particle_name.blood_on_ground_efx_green, position, high_layer: false);
            base.die();
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
            else if (state == enemy_state.dash)
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

        public class animation_player_bacteria : animation_player_base
        {
            public static readonly animation_data animation_data = new animation_data();

            public animation_player_bacteria(ContentManager content) : base()
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
                Texture2D idle_texture = content.Load<Texture2D>("assets/image/enemy/sprite_bacteria_idle"); //ใช้รูป player ไปก่อนเพราะ Beast ยังใส่ไม่ได้
                Texture2D walk_texture = content.Load<Texture2D>("assets/image/enemy/sprite_bacteria_walk"); //ใช้รูป player ไปก่อนเพราะ Beast ยังใส่ไม่ได้
                Texture2D dizzy_texture = content.Load<Texture2D>("assets/image/enemy/sprite_bacteria_dizzy");

                animation idle_animation = new animation(idle_texture, frame_per_sec: 8 ,sprite_size: new Point(80,80)); // (ใส่ , sprite_size: placeholder_sprite_size ไว้หลัง frame per sec  ถ้าใส่ beast ได้แล้ว)
                idle_animation.sprite_scale = new Vector2(1,1);
                idle_animation.sprite_origin = new Vector2(40, 64);
                idle_animation.name = "bacteria idle";
                animation_data.data.Add(animation_name.idle, idle_animation);

                animation walk_animation = new animation(walk_texture, frame_per_sec: 8, sprite_size: new Point(80, 80));
                walk_animation.sprite_scale = new Vector2(1, 1);
                walk_animation.sprite_origin = new Vector2(40, 64);
                walk_animation.name = "bacteria walk";
                animation_data.data.Add(animation_name.walk, walk_animation);

                animation dizzy_animation = new animation(dizzy_texture, frame_per_sec: 2, sprite_size: new Point(80, 80));
                dizzy_animation.sprite_scale = new Vector2(1, 1);
                dizzy_animation.sprite_origin = new Vector2(40, 64);
                dizzy_animation.name = "bacteria dizzy";
                animation_data.data.Add(animation_name.dizzy, dizzy_animation);

                Texture2D punch_texture = content.Load<Texture2D>("assets/image/enemy/sprite_bacteria_attack");
                animation punch_animation = new animation(punch_texture, loop: false, frame_per_sec: 12, sprite_size: new Point(80, 80));

                punch_animation.sprite_origin = new Vector2(40, 64);
                punch_animation.name = "bacteria attack";
                animation_data.data.Add(animation_name.attack, punch_animation);
            }
        }
    }
}