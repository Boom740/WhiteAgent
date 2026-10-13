using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Diagnostics;
using static System.Net.Mime.MediaTypeNames;

namespace old_heart
{
    public class ui_game_over : node
    {
        run_data_manager run_data;
        public enum result { none , respawn , return_to_main_menu};
        public result current_result = result.none;

        public image black_screen;
        public float black_screen_transition_time = 2;  // both in and out   // show other at halve trasition timer time
        public float black_screen_transition_timer = 0;

        public image black_background;
        public ui_text game_over_text;


        public Point respawn_button_size = new Point(300, 100);

        public ui_button respawn_button;
        public ui_text respawn_text;
        public ui_game_over(ContentManager content, game_manager game_manager , run_data_manager run_data)
        {
            this.run_data = run_data;
            active = false;

            Texture2D white_pixel = content.Load<Texture2D>("assets/image/other/white_pixel");
            black_background = new image(content, Vector2.Zero, texture: white_pixel);
            black_background.scale = global.render_size;
            black_background.color = Color.Black;
            black_background.visible = false;
            game_manager.add_ui(black_background);

            game_over_text = new ui_text("GAME OVER", Vector2.Zero , text_scale: new Vector2(1f, 1f));
            game_over_text.position = global.render_size * new Vector2(0.5f, 0.2f) - game_over_text.text_font.MeasureString(game_over_text.text_string) * game_over_text.text_scale / 2;
            game_over_text.visible = false;
            game_manager.add_ui(game_over_text);

            respawn_button = new ui_button(content, new Rectangle((global.render_size * new Vector2(0.5f, 0.8f)).ToPoint() - respawn_button_size/2, respawn_button_size));
            respawn_button.visible = false;
            respawn_button.active = false;
            game_manager.add_ui(respawn_button);

            respawn_text = new ui_text("respawn", Vector2.Zero, text_scale: new Vector2(0.5f, 0.5f) , text_color: Color.Black);
            respawn_text.position = global.render_size * new Vector2(0.5f, 0.8f) - game_over_text.text_font.MeasureString(respawn_text.text_string) * respawn_text.text_scale / 2;
            respawn_text.visible = false;
            game_manager.add_ui(respawn_text);

            black_screen = new image(content, Vector2.Zero, texture: white_pixel);
            black_screen.scale = global.render_size;
            black_screen.color = Color.Transparent;
            game_manager.add_ui(black_screen);

        }
        public override void Update(GameTime gameTime)
        {
            if (!active) return;

            float delta_time = (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (black_screen_transition_timer < black_screen_transition_time)  // transitioning
            {
                black_screen_transition_timer += delta_time;

                float alpha_ration = (1f - (Math.Abs((black_screen_transition_time / 2f) - black_screen_transition_timer) / (black_screen_transition_time / 2f)));   // 0 -> 1 -> 0    black screen alpha
                alpha_ration = alpha_ration * alpha_ration * alpha_ration * alpha_ration;  // ease in quart
                black_screen.color = Color.Black * alpha_ration;
            }
            if (black_screen_transition_timer > black_screen_transition_time/2f &&  black_background.visible == false)   // show ui
            {
                black_background.visible = true;
                game_over_text.visible = true;

                if (run_data.respawn_left > 0)
                {
                    respawn_text.visible = true;
                    respawn_button.active = true;
                    respawn_button.visible = true;
                }
                
            }

            if (current_result != result.none) { return; }

            if (respawn_button.clicked)
            {
                run_data.respawn_left--;
                current_result = result.respawn;
            }
            else if (run_data.respawn_left <=  0 && black_screen_transition_timer >= black_screen_transition_time)
            {
                current_result = result.return_to_main_menu;
                run_data.respawn_left--;
            }
        }
        public override void Draw(SpriteBatch sprite_batch)
        {
        }
    }
}
