using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using MonoGame.Extended.Collisions;
using System;
using System.Buffers;
using System.Diagnostics;

namespace old_heart
{
    // หัวที่ผู้เล่นขว้างออกไป: บินไปตาม velocity แล้วค่อยๆ ช้าลงด้วย drag จนหยุด (ไม่ time_out หายไปเอง รอผู้เล่นมาเก็บ)
    public class projectile_head : projectile
    {
        public run_data_manager run_data;

        public bool is_resting = false;
        public float drag = 3f;                    // ยิ่งมากยิ่งหยุดเร็ว
        private const float stop_velocity_threshold = 15f;

        public float bounce_power = 50f;   // 0-1 ยิ่งมากยิ่งกระเด้งกลับแรง
        public bool has_bounced = false; // กันโดนกระแทกซ้ำหลายเฟรมจาก enemy ตัวเดิม
        public float sprite_height = 25;
        public float initial_speed = 1000;

        public bool shock_wave_enable = true;

        public float pickup_lock_timer = 0f; // ห้ามเก็บหัว
        public float blink_interval = 0.1f;
        public float blink_min_alpha = 0.2f;
        public projectile_head(ContentManager content_set, Vector2 position , run_data_manager run_data , entity owner = null)
            : base(content_set,  position : position , owner : owner) // time_left ไม่ได้ใช้จริงเพราะ override Update ทั้งหมด
        {
            this.run_data = run_data;
            texture = content.Load<Texture2D>("assets/image/weapons/sprite_weapon_head");
            sprite_origin = new Vector2(texture.Width / 2, texture.Height * (3f/4f) + sprite_height); // position คือกึ่งกลาง X, 3/4 Y

            hit_box_radius = 14;
        }

        public override void Update(GameTime gameTime)
        {
            if (alive == false) return;
            float delta_time = (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (pickup_lock_timer > 0f)
            {
                pickup_lock_timer -= delta_time; // ต้องอยู่ก่อน is_resting check ไม่งั้นค้างไม่นับตอนหัวหยุดนิ่งแล้ว
            }
            if (is_resting) return;
            initial_speed = Math.Max(velocity.Length(), initial_speed);

            velocity -= velocity * drag * delta_time;
            position += velocity * delta_time;
            collision.Shape = new CollisionShape2D(new BoundingCircle2D(position, hit_box_radius));

            float speed_till_stop = velocity.Length() - stop_velocity_threshold;
            float height_ratio = Math.Min(1 , speed_till_stop / (initial_speed - stop_velocity_threshold));
            height_ratio = 1f - height_ratio;
            height_ratio = height_ratio * height_ratio;
            height_ratio = 1f - height_ratio;
            sprite_origin = new Vector2(texture.Width / 2, texture.Height * (4f/5f) + (height_ratio * sprite_height) );

            if (velocity.Length() < stop_velocity_threshold)
            {
                resting();
            }

        }
        public void resting()
        {
            velocity = Vector2.Zero;
            is_resting = true;
            sprite_origin = new Vector2(texture.Width / 2, texture.Height * (4f / 5f)); // position คือกึ่งกลาง X, 3/4 Y
        }
        public void spawn_shock_wave()
        {
            if (shock_wave_enable == false) { return;}
            shock_wave_enable = false;

            projectile_shock_wave shock_wave = new projectile_shock_wave(content, position, run_data , owner: this.owner);
            global.signal.spawn_projectile(shock_wave);
        }
        public void bounce_back(Vector2 bounce_vector)
        {
            velocity = bounce_vector * bounce_power;

            if (has_bounced == false)
            {
                has_bounced = true;
                initial_speed = velocity.Length();
            }
        }
        public override void on_hit_entity(entity target_entity)
        {

            if (has_bounced) return;
            if (is_resting) return;

            if (target_entity is enemy target_enemy)
            {
                spawn_shock_wave();

                Vector2 target_direction = position - target_enemy.position;
                Vector2 hit_direction = target_direction != Vector2.Zero ? Vector2.Normalize(target_direction) : Vector2.UnitY;
                bounce_back(hit_direction);
            }
        }
        
        public override void collide_wall(CollisionPair2D pair, float delta_time)
        {
            if (is_resting) return;

            bounce_back(Vector2.Normalize(pair.FirstResult.MinimumTranslationVector));
            spawn_shock_wave();
        }

        public override void Draw(SpriteBatch sprite_batch)
        {
            float alpha = 1f;
            if (pickup_lock_timer > 0f)
            {
                bool visible_phase = ((int)(pickup_lock_timer / blink_interval)) % 2 == 0;
                alpha = visible_phase ? 1f : blink_min_alpha;
            }

            float layer_depth = (position.Y + 50000f) / 100000f;
            Color color = Color.White * alpha;
            sprite_batch.Draw(texture, position, null, color, rotation, sprite_origin, sprite_scale, SpriteEffects.None, layer_depth);
        }
    }
}