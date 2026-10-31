using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Collisions;

namespace old_heart
{
    public class projectile_player_magnet : projectile
    {
        public float magnet_acceleration = 2000f;

        public projectile_player_magnet(ContentManager content_set, Vector2 position,  entity owner = null, float hit_box_radius = 150)
            : base(content_set, position, time_left: 10 , owner: owner, hit_box_radius: hit_box_radius)
        {
            visible = false; // ล่องหน ไม่ต้องมี texture เลย
        }
        public override void Update(GameTime gameTime)
        {
            time_left = 10; // prevent projectile despawn
            base.Update(gameTime);
        }
        public override void on_collide_hit_box(projectile target_projectile)
        {
            base.on_collide_hit_box(target_projectile);

            if (target_projectile is item_base item)
            {
                item.acceleration = Vector2.Normalize(position - item.position) * magnet_acceleration;
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