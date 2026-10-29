using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Collisions;

namespace old_heart
{
    public class projectile_merchant : projectile
    {
        public bool spawned = false;

        public animation_player_base animation_player;
        public projectile_merchant(ContentManager content_set, Vector2 position, entity owner = null)
            : base(content_set, position, time_left: 1, owner: owner, hit_box_radius: 50)
        {
            animation_player = new animation_player_merchant(content);

            shadow_scale = new Vector2(1.5f, 1.5f); 
            animation_player.play(animation_player.data.data[animation_player_merchant.animation_name.wave]);
        }

        public override void Update(GameTime gameTime)
        {
            float delta_time = (float)gameTime.ElapsedGameTime.TotalSeconds;

            animation_player.update(delta_time, "down");
            if (animation_player.current_animation == animation_player.data.data[animation_player_merchant.animation_name.wave] && animation_player.is_finished)
            {
                animation_player.play(animation_player.data.data[animation_player_merchant.animation_name.idle]);
            }
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
        public override void Draw(SpriteBatch sprite_batch)
        {
            animation_player.draw(sprite_batch, position);

            sprite_batch.Draw(shadow_texture, position, null, Color.White * new Color(1, 1, 1, 0.4f) , 0, shadow_origin, shadow_scale, SpriteEffects.None, 0);
        }
    }

    public class animation_player_merchant : animation_player_base       // custom animation for this class only
    {
        public enum animation_name { idle , wave }

        public static readonly animation_data animation_data = new animation_data();
        public animation_player_merchant(ContentManager content) : base()
        {
            if (animation_data.data.Count == 0)
            {
                load(content);
            }

            base.data = animation_data;

            default_animation = animation_data.data[animation_name.idle];
            current_animation = default_animation;
        }
        public void load(ContentManager content)
        {
            Texture2D idle_texture = content.Load<Texture2D>("assets/image/npc/sprite_merchant_idle");
            animation idle_animation = new animation(idle_texture, frame_per_sec: 8, loop: true, one_direction_sprite_format: true, one_diretion_sprite_format_frame_count: 6, sprite_size: new Point(64,64));
            idle_animation.sprite_origin = (idle_animation.sprite_size).ToVector2() * new Vector2(0.5f,0.9f);
            idle_animation.name = "merchant idle";
            animation_data.data.Add(animation_name.idle, idle_animation);

            Texture2D wave_texture = content.Load<Texture2D>("assets/image/npc/sprite_merchant_wave");
            animation wave_animation = new animation(wave_texture, frame_per_sec: 8, loop: false, one_direction_sprite_format: true, one_diretion_sprite_format_frame_count: 17, sprite_size: new Point(64, 64));
            wave_animation.sprite_origin = (wave_animation.sprite_size).ToVector2() * new Vector2(0.5f, 0.9f);
            wave_animation.name = "merchant wave";
            animation_data.data.Add(animation_name.wave, wave_animation);
        }
    }
}