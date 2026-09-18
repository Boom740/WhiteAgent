using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;
using MonoGame.Extended.Input;
using MonoGame.Extended.ViewportAdapters;
using System;
using System.Diagnostics;

namespace old_heart
{
    public class camera_manager
    {
        public OrthographicCamera ui_camera;  // not move
        public OrthographicCamera camera;     // move

        public BoxingViewportAdapter viewport_adapter;

        private float truama = 0;
        private float trauma_decay = 2f;
        private float max_Offset = 10f;
        private Random rng_shake = new Random();

        public bool following_player = false;
        public float follow_mouse_ratio = 0.05f; // 0-1  
        public float tween_smooting = 0.5f;  // 0-1    

        public camera_manager(GameWindow window, GraphicsDevice graphics_device)
        {
            viewport_adapter = new BoxingViewportAdapter(window,graphics_device,(int)global.render_size.X, (int)global.render_size.Y);

            ui_camera = new OrthographicCamera(viewport_adapter);
            camera = new OrthographicCamera(viewport_adapter);
        }
        public void shake_screen(float amount = 0.4f) // chess battle advanced
        {
            truama = MathHelper.Max(truama, amount);
        }
        public void update(GameTime gameTime ,player player)
        {
            update_global_mouse_position();

            if (player != null)
            {
                if (following_player == false)
                {
                    following_player = true;
                    camera.LookAt(player.position);
                }

                Vector2 target_position = (player.position*(1-follow_mouse_ratio) + global.input.scaled_mouse_world_position * (follow_mouse_ratio));
                Vector2 currentCenter = camera.Position + camera.Origin;

                camera.LookAt(Vector2.Lerp(currentCenter, target_position, tween_smooting));

            }

            if(truama > 0f)
            {
                float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;
                float shake = truama * truama;

                float offset_x = max_Offset * shake * (float)(rng_shake.NextDouble() * 2 - 1);
                float offset_y = max_Offset * shake * (float)(rng_shake.NextDouble() * 2 - 1);
                //float angle = max_angle * shake * (float)(rng_shake.NextDouble() * 2 - 1);

                camera.Position += new Vector2(offset_x, offset_y);

                truama = MathHelper.Clamp(truama - trauma_decay * dt, 0f, 1f);
            }

            
            KeyboardStateExtended keyboard_state = global.input.keyboard_state;
            if (keyboard_state.WasKeyPressed(Keys.E)) //camera shake Test
            {
                //global.signal.screen_shake(0.5f);
            }
        }

        public void update_global_mouse_position()
        {
            global.input.update_scaled_mouse(viewport_adapter, camera); // update mouse position base on viewport scale
        }
    }
}