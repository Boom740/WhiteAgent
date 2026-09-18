using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace old_heart
{
    public class enemy_leukemia : enemy
    {
        public enum animation_name { idle, walk, dizzy, died }


        // --- radii ---
        public float dangerous_rad = 150f;
        public float safe_rad = 250f;


        // --- clone ---
        public List<enemy_leukemia_minion> minion_list = new List<enemy_leukemia_minion> { };

        public float clone_timer = 5f; // TODO: ปรับค่าตามความยากง่ายที่ต้องการ
        private float current_clone_timer;
        public int minion_limit = 2;

        // --- frightened ---
        public float frightened_exit_timer = 0f;
        private const float frightened_exit_delay = 0.5f;

        // --- patrol (square) ---
        private float patrol_time_wait = 1f;
        private float patrol_time_walk = 0.5f;
        private float current_patrol_time = 0f;

        public enemy_leukemia(ContentManager content_set, Vector2 position) : base(content_set, position, max_hp: 5 , speed: 1200)
        {
            animation_player = new animation_player_leukemia(content_set);
            current_clone_timer = clone_timer;
        }
        public override void Update(GameTime gameTime)
        {
            if (alive == false) return;
            float delta_time = (float)gameTime.ElapsedGameTime.TotalSeconds;

            minion_list.RemoveAll(enemy => enemy.alive == false);     // clear dead minion 

            // shield regen ทำงานอิสระจาก state ตามที่ต้องการ
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
                    update_clone_timer(delta_time);
                    update_patrol();
                    check_player_distance();
                    break;

                case enemy_state.frightened:
                    update_clone_timer(delta_time);
                    update_frightened(delta_time);
                    break;

                case enemy_state.dizzy:
                    update_dizzy(delta_time);
                    break;

                case enemy_state.died:
                    break;
            }

            base.Update(gameTime); // ให้ entity จัดการ velocity/position/animation/collision ตามปกติ

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

            }

            void check_player_distance()
            {
                if (target == null) return;
                float distance = Vector2.Distance(position, target.position);
                if (distance <= dangerous_rad)
                {
                    enter_frightened();
                }
            }

            // ---------------- Frightened ----------------

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
                        state = enemy_state.normal;
                        frightened_exit_timer = 0f;
                        acceleration = Vector2.Zero;   // stop when return to normal state
                    }
                }
                else
                {
                    frightened_exit_timer = 0f; // ยังไม่พ้น safe_rad ให้รีเซ็ต delay

                    acceleration = away_direction * speed ;  // เดินหนี แค่ตอนอยู่ในระยะ
                    current_direction_vector = acceleration;
                }
            }

            // ---------------- Dizzy ----------------

            void update_dizzy(float delta_time)
            {
                acceleration = Vector2.Zero;

                dizzy_timer_current -= delta_time;
                if (dizzy_timer_current <= 0f)
                {
                    state = enemy_state.normal;
                }
            }

            // ---------------- Clone ----------------

            void update_clone_timer(float delta_time)
            {
                if (minion_list.Count >= minion_limit)
                {
                    current_clone_timer = clone_timer;  // timer dont move
                    return;
                }

                current_clone_timer -= delta_time;
                if (current_clone_timer <= 0f)
                {
                    current_clone_timer = clone_timer;

                    spawn_minion(new Vector2(-1, 0));

                    if (minion_list.Count < minion_limit)
                    {
                        spawn_minion(new Vector2(1, 0));
                    }
                }

                void spawn_minion(Vector2 offset)
                {
                    enemy_leukemia_minion new_minion = new enemy_leukemia_minion(content, position + offset);
                    global.signal.spawn_entity(new_minion);
                    minion_list.Add(new_minion);
                }
            }
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



        public class animation_player_leukemia : animation_player_base
        {
            public static readonly animation_data animation_data = new animation_data();

            public animation_player_leukemia(ContentManager content) : base()
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

                // TODO: Beast.png เป็น placeholder แทน Leukemia ไปก่อน เปลี่ยน path เมื่อมีภาพจริงของ leukemia
                Texture2D placeholder_texture = content.Load<Texture2D>("Placeholder/Player/Idle"); //ใช้รูป player ไปก่อนเพราะ Beast ยังใส่ไม่ได้
                Texture2D placeholder_texture_2 = content.Load<Texture2D>("Placeholder/Player/Walk"); //ใช้รูป player ไปก่อนเพราะ Beast ยังใส่ไม่ได้

                animation idle_animation = new animation(placeholder_texture, frame_per_sec: 2 ,sprite_size: new Point(32,32)); // (ใส่ , sprite_size: placeholder_sprite_size ไว้หลัง frame per sec  ถ้าใส่ beast ได้แล้ว)
                idle_animation.sprite_scale = new Vector2(2,2);
                idle_animation.name = "leukemia idle";
                animation_data.data.Add(animation_name.idle, idle_animation);

                animation walk_animation = new animation(placeholder_texture_2, frame_per_sec: 8, sprite_size: new Point(32, 32));
                walk_animation.sprite_scale = new Vector2(2, 2);
                walk_animation.name = "leukemia walk";
                animation_data.data.Add(animation_name.walk, walk_animation);

                animation dizzy_animation = new animation(placeholder_texture, frame_per_sec: 2, sprite_size: new Point(32, 32));
                dizzy_animation.sprite_scale = new Vector2(2, 2);
                dizzy_animation.name = "leukemia dizzy";
                animation_data.data.Add(animation_name.dizzy, dizzy_animation);
            }
        }
    }
}