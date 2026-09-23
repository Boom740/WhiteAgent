using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace old_heart
{
    // projectile ระยะไกลที่ leukemia ยิงใส่ player — โดนแล้วเหมือนโดนโจมตีทุกประการ (take_damage + knockback แบบเดียวกับ melee)
    public class projectile_leukemia_bullet : projectile
    {
        public int damage;
        public float knockback_speed;

        public projectile_leukemia_bullet(ContentManager content_set, Vector2 position, Vector2 aim_direction, float projectile_speed, int damage, float knockback_speed, float lifetime = 2f)
            : base(content_set, position, time_left: lifetime)
        {
            this.damage = damage;
            this.knockback_speed = knockback_speed;
            velocity = (aim_direction != Vector2.Zero ? Vector2.Normalize(aim_direction) : Vector2.UnitY) * projectile_speed;

            // TODO: ใช้ placeholder ไปก่อน เปลี่ยน path เมื่อมี sprite projectile จริงของ leukemia
            texture = content.Load<Texture2D>("assets/image/weapons/sprite_weapon_head");
            sprite_origin = new Vector2(texture.Width / 2f, texture.Height / 2f);
            sprite_scale = new Vector2(0.6f, 0.6f); // ย่อขนาดกว่าตัวหัวจริง เพราะยืม sprite มาใช้ชั่วคราว

            hit_box_radius = 10f;
        }

        public override void on_hit_entity(entity target_entity)
        {
            if (target_entity is player target_player && target_player.alive)
            {
                Vector2 hit_direction = velocity != Vector2.Zero ? Vector2.Normalize(velocity) : Vector2.UnitY;

                target_player.apply_knockback(hit_direction, knockback_speed); // knockback เสมอ เหมือนกับ melee
                target_player.take_damage(damage, damage_dealer: this); // ทำงานเหมือนโดนโจมตีทุกอย่าง (หัวหลุด, i-frame ฯลฯ)

                time_out(); // โดนแล้วหายทันที
            }
        }
    }
}