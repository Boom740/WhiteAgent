using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Collisions;
using System.Collections.Generic;
using System.Diagnostics;

namespace old_heart
{
    // hitbox ของท่าโจมตีปกติ: ล่องหน วิ่งออกไปตามทิศ cursor ระยะสั้นๆ แล้วหายไป
    public class projectile_melee : projectile
    {
        public int damage;
        public float knockback_speed = 150f;
        private HashSet<entity> hit_entity = new HashSet<entity>(); // กันโดนดาเมจซ้ำจาก swing เดียวกัน

        public projectile_melee(ContentManager content_set, Vector2 position, Vector2 aim_direction, float travel_distance = 50, float travel_time = 0.3f, int damage = 1 , int hit_box_dadius = 15)
            : base(content_set,  position , time_left: travel_time)
        {
            this.damage = damage;
            this.velocity = Vector2.Normalize(aim_direction) * (travel_distance / travel_time); // วิ่งให้ได้ระยะ travel_distance พอดีตอน time_left หมด
            this.hit_box_radius = hit_box_dadius;

            visible = false; // ล่องหน ไม่ต้องมี texture เลย
        }

        public override void on_hit_entity(entity target_entity)
        {
            if (owner is player)
            {
                if (target_entity is enemy target_enemy && target_enemy.alive && hit_entity.Contains(target_enemy) == false)
                {
                    hit_entity.Add(target_enemy);

                    Vector2 hit_direction = velocity != Vector2.Zero ? Vector2.Normalize(velocity) : Vector2.UnitY;

                    target_enemy.apply_knockback(hit_direction, knockback_speed); // ผลักเสมอ ไม่ว่าจะมี shield กันดาเมจอยู่หรือไม่
                    target_enemy.take_damage(damage, damage_dealer: this); // ดาเมจยังถูก shield บล็อกตามปกติถ้ามี shield อยู่

                    /* bool deal_damage = target_enemy.take_damage(damage,damage_dealer: this);
                     if (deal_damage)
                     {
                         target_enemy.apply_knockback(hit_direction, knockback_speed); // ผลักตามทิศที่หมัดพุ่งเข้าใส่
                     }*/
                }
            }
            else if (owner is enemy)
            {
                if (target_entity is player target_player && target_player.alive && hit_entity.Contains(target_player) == false)
                {
                    hit_entity.Add(target_player);

                    Vector2 hit_direction = velocity != Vector2.Zero ? Vector2.Normalize(velocity) : Vector2.UnitY;

                    bool deal_damage = target_player.take_damage(damage, damage_dealer: this);
                    if (deal_damage)
                    {
                        target_player.apply_knockback(hit_direction, knockback_speed); // ผลักตามทิศที่หมัดพุ่งเข้าใส่
                    }
                }
            }
            else
            {
                Debug.WriteLine("projectile_melee owner is not player or enemy");
            }
        }
        public override void collide_wall(CollisionPair2D pair, float delta_time)
        {
            // not disapear when hit wall
        }
        public override void Draw(SpriteBatch sprite_batch)
        {
            // ล่องหน ไม่วาดอะไรเลย (ไม่เรียก base.Draw เพราะไม่มี texture โหลดไว้)
        }
    }
}