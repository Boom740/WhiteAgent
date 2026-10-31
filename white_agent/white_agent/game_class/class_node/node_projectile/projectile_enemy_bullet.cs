using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace old_heart
{
    // projectile ระยะไกลที่ leukemia ยิงใส่ player — โดนแล้วเหมือนโดนโจมตีทุกประการ (take_damage + knockback แบบเดียวกับ melee)
    public class projectile_enemy_bullet : projectile
    {
        public int damage;
        public float knockback_speed;

        private trail_effect trail;
        public projectile_enemy_bullet(ContentManager content_set, Vector2 position, Vector2 aim_direction, float projectile_speed, int damage, float knockback_speed, float lifetime = 2f,
     string texture_path = "assets/image/weapons/sprite_bullet_boss", float scale = 0.8f)
     : base(content_set, position, time_left: lifetime, hit_box_radius: 6)
        {
            this.damage = damage;
            this.knockback_speed = knockback_speed;
            velocity = (aim_direction != Vector2.Zero ? Vector2.Normalize(aim_direction) : Vector2.UnitY) * projectile_speed;

            texture = content.Load<Texture2D>(texture_path);
            sprite_origin = new Vector2(texture.Width / 2f, texture.Height * 3 / 2f);
            sprite_scale = new Vector2(scale);

            deflectable = true;

            trail = new trail_effect(max_points: 8, max_width: texture.Width * sprite_scale.X, min_point_distance: 2f, color: Color.Gray);
        }
        public override void Update(GameTime gameTime)
        {
            // ให้คลาสแม่จัดการเรื่องเคลื่อนที่
            base.Update(gameTime);

            if (alive)
            {
                float offset_x = 0f;
                float offset_y = -14f;
                trail.add_point(position + new Vector2(offset_x, offset_y));
            }
        }

        public override void on_hit_entity(entity target_entity)
        {
            if (alive == false) { return; }
            if (target_entity is player target_player && target_player.alive)
            {
                bool deal_damage = target_player.take_damage(damage, damage_dealer: this); // ทำงานเหมือนโดนโจมตีทุกอย่าง (หัวหลุด, i-frame ฯลฯ)
                if (!deal_damage) { return; }

                Vector2 hit_direction = velocity != Vector2.Zero ? Vector2.Normalize(velocity) : Vector2.UnitY;

                target_player.apply_knockback(hit_direction, knockback_speed); // knockback เสมอ เหมือนกับ melee
                target_player.take_damage(damage, damage_dealer: this); // ทำงานเหมือนโดนโจมตีทุกอย่าง (หัวหลุด, i-frame ฯลฯ)

                time_out(); // โดนแล้วหายทันที
            }

            base.on_hit_entity (target_entity);
        }
        public override void time_out()
        {
            trail.clear();
            base.time_out();
        }

        public override void collide_wall(MonoGame.Extended.Collisions.CollisionPair2D pair, float delta_time)
        {
        
            trail.clear();
            base.collide_wall(pair, delta_time);
        }

        public override void Draw(SpriteBatch sprite_batch, float alpha)
        {
            float layer_depth = Math.Clamp((position.Y + 50000f) / 100000f, 0, 1) - 0.0001f;
            trail.draw(sprite_batch, alpha, layer_depth);

            base.Draw(sprite_batch, alpha);
        }
    }
}