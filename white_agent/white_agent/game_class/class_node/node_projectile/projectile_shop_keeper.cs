using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Collisions;
using System.Diagnostics;

namespace old_heart
{
    public class projectile_shop_keeper : projectile
    {
        public bool spawned = false;

        public projectile_shop_keeper(ContentManager content_set, Vector2 position, entity owner = null)
            : base(content_set, position, time_left: 1, owner: owner, hit_box_radius: 50)
        {
            texture = content.Load<Texture2D>("assets/image/ui/crosshair_002");
            sprite_origin = texture.Bounds.Size.ToVector2() * new Vector2(0.5f,0.8f);

            shadow_scale = new Vector2(1.5f, 1.5f);
            Debug.WriteLine(shadow_scale.ToString());   
        }

        public override void Update(GameTime gameTime)
        {
            // do nothing
        }
        public override void on_hit_entity(entity target_entity)
        {
            if (target_entity is player == false) return;

            player player = target_entity as player;
            player.in_interactable_zone = true;

            if (player.current_buffer_input == player.bufferable_input.interact)
            {
                player.current_combat_state = player.combat_state.in_shop;
            }
        }

        public override void collide_wall(CollisionPair2D pair, float delta_time)
        {
            // not disapear when hit wall
        }
    }
}