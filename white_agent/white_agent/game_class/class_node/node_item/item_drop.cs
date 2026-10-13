using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using MonoGame.Extended.Collisions;
using old_heart;
using System;

namespace white_agent.game_class.class_node.node_item
{
    public abstract class item_drop : projectile
    {
       /* public ContentManager content;
        public Texture2D texture;    
        public Texture2D shadow_texture;*/
        public Color placeholder_color = Color.White;
        public int placeholder_size = 8;

       // public Vector2 position;
       // public Vector2 velocity = Vector2.Zero; 
        public float friction = 6f;

        // --- pickup ---
       // public float pickup_radius = 3f;        // เก็บได้เมื่อห่าง player น้อยกว่า pickup_radius + hit_box_radius ของ player
        public float magnet_radius = 30f;       // เข้าใกล้กว่านี้แล้วไอเทมเริ่มลอยเข้าหา player
        public float magnet_acceleration = 1800f;
        public float magnet_max_speed = 700f;
        public float pickup_delay = 0.5f;        // กันเก็บทันทีตอนเพิ่งเด้งออกมา
        public bool collected = false;
        private float age = 0f;

        // --- lifetime ---
       // public float lifetime = 30f;
        public float blink_time = 3f;            // กระพริบช่วงท้ายก่อนหาย
        public float blink_interval = 0.1f;

        // --- visual ---
        public float float_height = 8f;          // ลอยเหนือพื้นกี่ px
        public float bob_height = 2f;
        public float bob_speed = 6f;

        protected item_drop(ContentManager content_set, Vector2 position)
            : base(content_set, position, time_left: 30f, owner: null, hit_box_radius: 15f)
        {
           
        }

        public override void Update(GameTime gameTime)
        {
            if (collected || active == false) return;
            float delta_time = (float)gameTime.ElapsedGameTime.TotalSeconds;

            age += delta_time;

            if (velocity != Vector2.Zero)
            {
                velocity -= velocity * friction * delta_time;
                //position += velocity * delta_time;
                if (velocity.LengthSquared() < 4f)
                {
                    velocity = Vector2.Zero;
                }
            }

            /*lifetime -= delta_time;
            if (lifetime <= 0f)
            {
                active = false; // หมดอายุ ให้ game_manager ลบทิ้ง
            }*/

            base.Update(gameTime);
        }

        // เรียกจาก item_manager ทุกเฟรมหลัง Update
        public void update_pickup(player target, float delta_time)
        {
            if (collected || !alive) return;
            if (target == null || target.alive == false) return;
            if (age < pickup_delay) return;

            Vector2 to_target = target.position - position;
            float distance = to_target.Length();

            if (distance <= hit_box_radius + target.hit_box_radius)
            {
                collected = true;
                on_picked_up(target);
                time_out(); 
                return;
            }

            if (distance <= magnet_radius && distance > 0.001f)
            {
                velocity += to_target / distance * magnet_acceleration * delta_time;
                if (velocity.Length() > magnet_max_speed)
                {
                    velocity = Vector2.Normalize(velocity) * magnet_max_speed;
                }
            }
        }

        // ชนิดไอเทมแต่ละตัวเขียนเองว่าเก็บแล้วเกิดอะไร
        protected abstract void on_picked_up(player target);

        public override void collide_wall(CollisionPair2D pair, float delta_time)
        {
            velocity = Vector2.Zero;
        }
        public override void Draw(SpriteBatch sprite_batch)
        {
            if (collected) return;

            float alpha = 1f;
            if (time_left <= blink_time) 
            {
                bool visible_phase = (int)(time_left / blink_interval) % 2 == 0;
                alpha = visible_phase ? 1f : 0.3f;
            }

            float bob = MathF.Sin(age * bob_speed) * bob_height;
            Vector2 draw_position = position + new Vector2(0, -float_height - bob);
            float layer_depth = (position.Y + 50000f) / 100000f - 0.0001f; // อยู่หลัง entity ที่ยืน Y เดียวกันเล็กน้อย

            float shadow_size = hit_box_radius * 2f / shadow_texture.Width; // ใช้ hit_box_radius คลาสแม่
            Vector2 shadow_origin = new Vector2(shadow_texture.Width, shadow_texture.Height) / 2f;
            sprite_batch.Draw(shadow_texture, position, null, Color.White * (0.4f * alpha), 0, shadow_origin, new Vector2(shadow_size), SpriteEffects.None, 0);

            // วาดไอเทม
            if (texture != null)
            {
                Vector2 origin = new Vector2(texture.Width / 2f, texture.Height / 2f);
                sprite_batch.Draw(texture, draw_position, null, Color.White * alpha, 0, origin, Vector2.One, SpriteEffects.None, layer_depth);
            }
            else
            {
                int half = placeholder_size / 2;
                Rectangle outer = new Rectangle((int)draw_position.X - half - 1, (int)draw_position.Y - half - 1, placeholder_size + 2, placeholder_size + 2);
                Rectangle inner = new Rectangle((int)draw_position.X - half, (int)draw_position.Y - half, placeholder_size, placeholder_size);
                sprite_batch.FillRectangle(outer, Color.Black * alpha, layer_depth);
                sprite_batch.FillRectangle(inner, placeholder_color * alpha, layer_depth + 0.00001f);
            }   
        }
    }
}