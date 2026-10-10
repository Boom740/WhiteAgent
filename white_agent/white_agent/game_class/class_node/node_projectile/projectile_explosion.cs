using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Collisions;
using System.Collections.Generic;
using System.Diagnostics;

namespace old_heart
{
    public class projectile_explosion : projectile
    {
        public int damage = 1;
        //public float knockback_speed = 250f; // ความแรงที่ enemy จะกระเด็น
        private HashSet<entity> hit_entity = new HashSet<entity>(); // กันโดนดาเมจซ้ำ

        public projectile_explosion(ContentManager content_set, Vector2 position,  entity owner = null, float hit_box_radius = 45)
            : base(content_set, position, time_left: 0.1f, owner: owner, hit_box_radius: hit_box_radius)
        {
            visible = false; // ล่องหน ไม่ต้องมี texture เลย
        }

        public override void on_hit_entity(entity target_entity)
        {
            if (target_entity is enemy target_enemy && target_enemy.alive && hit_entity.Contains(target_enemy) == false)
            {
                hit_entity.Add(target_enemy);


                target_enemy.take_damage(damage);


                //Vector2 target_direction = target_enemy.position - position;
                //Vector2 hit_direction = target_direction != Vector2.Zero ? Vector2.Normalize(target_direction) : Vector2.UnitY;
                //target_enemy.apply_knockback(hit_direction, knockback_speed); // ผลักตามทิศที่หมัดพุ่งเข้าใส่
                //target_enemy.enter_dizzy();
            }

            base.on_hit_entity(target_entity);
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