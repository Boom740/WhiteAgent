using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Collisions;
using System.Collections.Generic;
using System.Diagnostics;

namespace old_heart
{
    public class projectile_shock_wave : projectile
    {
        run_data_manager run_data;

        public int damage = 0;
        public float knockback_speed = 250f; // ความแรงที่ enemy จะกระเด็น
        private HashSet<enemy> hit_enemies = new HashSet<enemy>(); // กันโดนดาเมจซ้ำจาก swing เดียวกัน

        public projectile_shock_wave(ContentManager content_set, Vector2 position , run_data_manager run_data , entity owner = null)
            : base(content_set, position, time_left:0.1f , owner: owner)
        {
            this.run_data = run_data;
            visible = false; // ล่องหน ไม่ต้องมี texture เลย

            hit_box_radius = run_data.shock_wave_radius;
        }

        public override void on_hit_entity(entity target_entity)
        {
            if (target_entity is enemy target_enemy && target_enemy.alive && hit_enemies.Contains(target_enemy) == false)
            {
                hit_enemies.Add(target_enemy);
                Vector2 target_direction = target_enemy.position - position;
                Vector2 hit_direction = target_direction != Vector2.Zero ? Vector2.Normalize(target_direction) : Vector2.UnitY;
                target_enemy.apply_knockback(hit_direction, knockback_speed); // ผลักตามทิศที่หมัดพุ่งเข้าใส่
                // target_enemy.take_damage(damage);   // no damage
                target_enemy.enter_dizzy();
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