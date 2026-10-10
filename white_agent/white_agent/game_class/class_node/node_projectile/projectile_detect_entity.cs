using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Collisions;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace old_heart
{
    public class projectile_detect_entity : projectile
    {
        private HashSet<entity> detected_entity = new HashSet<entity>(); // every entity detected
        public Action<HashSet<entity>> return_funciton;
        public projectile_detect_entity(ContentManager content_set, Vector2 position , entity owner = null , float hit_box_radius = 200 , Action<HashSet<entity>> return_function = null)
            : base(content_set, position, time_left:0.02f , owner: owner, hit_box_radius: hit_box_radius)
        {
            visible = false; // ล่องหน ไม่ต้องมี texture เลย
            this.return_funciton = return_function;
        }

        public override void on_hit_entity(entity target_entity)
        {
            if (target_entity is enemy target_enemy && target_enemy.alive && detected_entity.Contains(target_enemy) == false)
            {
                detected_entity.Add(target_enemy);
            }
        }

        public override void time_out()
        {
            base.time_out();


            return_funciton(detected_entity);
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