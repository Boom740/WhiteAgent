using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Collisions;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace old_heart
{
    public class projectile_next_level_portal : projectile
    {
        public bool spawned = false;

        public animation_player_base animation_player;
        public projectile_next_level_portal(ContentManager content_set, Vector2 position, entity owner = null)
            : base(content_set, position, time_left: 1, owner: owner, hit_box_radius: 30)
        {
            animation_player = new animation_player_next_level_portal(content);
        }

        public override void Update(GameTime gameTime)
        {
            float delta_time = (float)gameTime.ElapsedGameTime.TotalSeconds;

            animation_player.update(delta_time, "down");
        }

        public override void spawn(projectile_manager projectile_manager, collision_manager collision_manager, debug_manager debug_manager)
        {
            base.spawn(projectile_manager, collision_manager, debug_manager);

            global.signal.spawn_particle(particle_manager.particle_name.die_efx_red, position);
        }
        public override void on_hit_entity(entity target_entity)
        {
            if (target_entity is player == false) return;

            player player = target_entity as player;

            if (player.current_buffer_input == player.bufferable_input.interact)
            {
                player.enter_next_level(position);
            }
        }

        public override void collide_wall(CollisionPair2D pair, float delta_time)
        {
            // not disapear when hit wall
        }
        public override void Draw(SpriteBatch sprite_batch)
        {
            animation_player.draw(sprite_batch, position);
        }
    }
    public class animation_player_next_level_portal : animation_player_base       // custom animation for this class only
    {
        public enum animation_name { idle }

        public static readonly animation_data animation_data = new animation_data();
        public animation_player_next_level_portal(ContentManager content) : base()
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
            Texture2D idle_texture = content.Load<Texture2D>("assets/image/player/sprite_player_die");
            animation idle_animation = new animation(idle_texture, frame_per_sec: 16, loop: true, one_direction_sprite_format: true, one_diretion_sprite_format_frame_count: 13);
            idle_animation.layer_depth_offset = -1f;
            idle_animation.name = "next_level_portal idle";
            animation_data.data.Add(animation_name.idle, idle_animation);
        }
    }
}