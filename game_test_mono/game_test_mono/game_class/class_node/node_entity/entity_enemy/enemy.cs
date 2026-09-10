using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using System;

namespace old_heart
{
    public class enemy : entity
    {
        // shared animation key ใช้ร่วมกันทุก enemy type เพื่อให้ base class เลือก animation ถูก
        public enum animation_name { idle, walk, dizzy, died }

        public enum enemy_state { normal, frightened, dizzy, died }
        public enemy_state state = enemy_state.normal;

        // --- target (player) ---
        public entity target;

        // --- radii ---
        public float dangerous_rad = 150f;
        public float safe_rad = 250f;

        // --- shield ---
        public bool shield = true;
        public float shield_timer = 3f; // ตั้งเท่ากับ dizzy_timer ไว้ก่อน ปรับแยกได้ทีหลัง
        private float shield_timer_current = 0f;

        // --- dizzy ---
        public float dizzy_timer = 3f;
        private float dizzy_timer_current = 0f;

        // --- clone ---
        public float clone_timer = 5f; // TODO: ปรับค่าตามความยากง่ายที่ต้องการ
        private float clone_timer_current;

        // --- frightened ---
        public float frightened_speed_multiplier = 2f;
        private float frightened_exit_timer = 0f;
        private const float frightened_exit_delay = 1f;

        // --- patrol (square) ---
        private float patrol_time_wait = 1f;
        private float patrol_time_walk = 0.5f;
        private float current_patrol_time = 0f;

        private Random random = new Random();

        public enemy(ContentManager content_set, int max_hp, Vector2 position, float speed) : base(content_set, max_hp, position, speed)
        {
            clone_timer_current = clone_timer;
        }

        public override void Update(GameTime gameTime)
        {
            if (alive == false) return;
            float delta_time = (float)gameTime.ElapsedGameTime.TotalSeconds;

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
                    }
                }
                else
                {
                    frightened_exit_timer = 0f; // ยังไม่พ้น safe_rad ให้รีเซ็ต delay

                    acceleration = away_direction * speed * frightened_speed_multiplier;  // เดินหนี แค่ตอนอยู่ในระยะ
                }
            }

            // ---------------- Dizzy ----------------

            void update_dizzy(float delta_time)
            {
                velocity = Vector2.Zero;
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
                clone_timer_current -= delta_time;
                if (clone_timer_current <= 0f)
                {
                    clone_timer_current = clone_timer;
                    global.signal.spawn_entity(new enemy_leukemia(content, position + new Vector2(0, 0)));
                }
            }
        }

        // ---------------- Dizzy ----------------
        private void enter_dizzy()
        {
            state = enemy_state.dizzy;
            shield = false;
            dizzy_timer_current = dizzy_timer;
            shield_timer_current = shield_timer;
            velocity = Vector2.Zero;
            acceleration = Vector2.Zero;
        }

        public void on_hit_by_projectile(projectile proj) // เรียกจาก collision manager ตอน projectile ชน enemy
        {
            if (alive == false) return;
            if (shield && (state == enemy_state.normal || state == enemy_state.frightened))
            {
                enter_dizzy();
            }
        }
        // ---------------- Damage / Death ----------------
        public override void take_damage(int damage_taken)
        {
            if (alive == false) return;
            if (shield) return; // มี shield อยู่ โจมตีธรรมดาไม่เข้า

            base.take_damage(damage_taken);
            dizzy_timer_current += 0.3f;
            shield_timer_current += 0.3f;
            if (alive == false)
            {
                state = enemy_state.died;
            }
        }

        public override void die()
        {
            base.die();
            //global.collision_manager.remove(collision); // กัน collision ค้างในโลกหลัง enemy ตาย      // มันลบอยู่แล้วตอนหาย ใน game_manager update (clear_inactive_node())
        }

        // ---------------- Animation ----------------

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
    }
}