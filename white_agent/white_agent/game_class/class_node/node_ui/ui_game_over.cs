using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Diagnostics;

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
        public ui_text ui_game_over_text;

        public ui_text ui_respawn_text;
        public ui_text ui_life_text;
        public image ui_life_image;

        public ui_text ui_life_change_text;
        public Vector2 life_change_text_travel_vector = new Vector2(20, 0);
        public Vector2 life_change_text_start_position;
        public float life_change_text_time = 0.5f;
        public float life_change_text_timer = 0;

        public float game_over_animation_time = 2f;
        public float game_over_animation_timer = 0;

        public float heart_beat_amination_grow_time = 0.2f;
        public float heart_beat_amination_shrink_time = 0.3f;

        public Vector2 heart_beat_scale_shrink = new Vector2(1, 1);
        public Vector2 heart_beat_scale_default = new Vector2(1.5f ,1.5f);
        public Vector2 heart_beat_scale_grow = new Vector2(2, 2);
        public ui_game_over(ContentManager content, game_manager game_manager , run_data_manager run_data)
        {
            this.run_data = run_data;
            active = false;
            Vector2 render_size = global.render_size;

            Texture2D white_pixel = content.Load<Texture2D>("assets/image/other/white_pixel");
            black_background = new image(content, Vector2.Zero, texture: white_pixel);
            black_background.scale = render_size;
            black_background.color = Color.Black;
            black_background.visible = false;
            game_manager.add_ui(black_background);

            ui_game_over_text = new ui_text("GAME OVER", Vector2.Zero , text_scale: new Vector2(1f, 1f));
            ui_game_over_text.position = render_size * new Vector2(0.5f, 0.2f) - ui_game_over_text.text_font.MeasureString(ui_game_over_text.text_string) * ui_game_over_text.text_scale / 2;
            ui_game_over_text.visible = false;
            game_manager.add_ui(ui_game_over_text);

            ui_respawn_text = new ui_text("Remaining Life", Vector2.Zero, text_scale: new Vector2(0.5f, 0.5f) , text_color: Color.White);
            ui_respawn_text.position = render_size * new Vector2(0.5f, 0.4f) - ui_game_over_text.text_font.MeasureString(ui_respawn_text.text_string) * ui_respawn_text.text_scale / 2;
            ui_respawn_text.visible = false;
            game_manager.add_ui(ui_respawn_text);

            ui_life_text = new ui_text("x " + run_data.respawn_left, Vector2.Zero, text_scale: new Vector2(0.5f, 0.5f), text_color: Color.White);
            ui_life_text.position = render_size * new Vector2(0.55f, 0.55f) - ui_life_text.text_font.MeasureString(ui_life_text.text_string) * ui_life_text.text_scale / 2;
            ui_life_text.visible = false;
            game_manager.add_ui(ui_life_text);

            life_change_text_timer = life_change_text_time;
            life_change_text_start_position = ui_life_text.position + (ui_life_text.text_font.MeasureString(ui_life_text.text_string) * ui_life_text.text_scale * new Vector2(1, 0f)) + new Vector2(10, 0);
            ui_life_change_text = new ui_text("-1", life_change_text_start_position , text_scale: new Vector2(0.3f, 0.3f) , text_color: new Color(255, 50, 50));
            ui_life_change_text.visible = false;
            game_manager.add_ui(ui_life_change_text);


            ui_life_image = new image(content, Vector2.Zero, file_path: "assets/image/ui/ui_loselife");
            ui_life_image.scale = heart_beat_scale_default;
            ui_life_image.position = render_size * new Vector2(0.5f, 0.62f) - (ui_life_image.texture.Bounds.Size.ToVector2() * ui_life_image.scale / 2) ;
            ui_life_image.origin = ui_life_image.texture.Bounds.Size.ToVector2() / 2;
            ui_life_image.visible = false;
            game_manager.add_ui(ui_life_image);

            black_screen = new image(content, Vector2.Zero, texture: white_pixel);
            black_screen.scale = render_size;
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
            else if (game_over_animation_timer < game_over_animation_time) // done transition   start animation
            {
                game_over_animation_timer += delta_time;

                if (run_data.respawn_left > 1)
                {
                    update_life_change_animation();
                }
            }

            if (black_screen_transition_timer > black_screen_transition_time / 2f && black_background.visible == false)   // show ui
            {
                black_background.visible = true;

                if (run_data.respawn_left > 1)
                {
                    ui_respawn_text.visible = true;
                    ui_life_text.visible = true;
                    ui_life_image.visible = true;
                }
                else
                {
                    ui_game_over_text.visible = true;
                }

            }

            if (current_result != result.none) { return; }

            if (run_data.respawn_left > 1 && game_over_animation_timer >= game_over_animation_time)
            {
                run_data.respawn_left--;
                current_result = result.respawn;
            }
            else if (run_data.respawn_left <=  1 && game_over_animation_timer >= game_over_animation_time)
            {
                run_data.respawn_left--;
                current_result = result.return_to_main_menu;
            }

            void update_life_change_animation()
            {
                ui_life_change_text.visible = true;
                ui_life_text.text_string = "x " + (run_data.respawn_left - 1);

                if (life_change_text_timer > 0)
                {
                    life_change_text_timer -= delta_time;

                    float move_ratio = life_change_text_timer / life_change_text_time;  // start a 1   -> 0
                    float color_ratio = 1f - move_ratio;  // start a 0  -> 1

                    move_ratio *= move_ratio * move_ratio * move_ratio * move_ratio;
                    color_ratio *= color_ratio * color_ratio * color_ratio * color_ratio;

                    move_ratio = 1f - move_ratio;   // start a 0 -> 1
                    color_ratio = 1f - color_ratio; // start a 1 -> 0

                    ui_life_change_text.position = life_change_text_start_position + (life_change_text_travel_vector * move_ratio);
                    ui_life_change_text.alpha = color_ratio;
                }
                else
                {
                    ui_life_change_text.visible = false;
                }

                if (game_over_animation_timer < heart_beat_amination_grow_time)
                {
                    float ratio = game_over_animation_timer / heart_beat_amination_grow_time;

                    ratio = 1 - ratio;
                    ratio *= ratio * ratio * ratio;
                    ratio = 1 - ratio;

                    ui_life_image.scale = (heart_beat_scale_shrink * (1f - ratio)) + (heart_beat_scale_grow * ratio);
                }
                else if (game_over_animation_timer - heart_beat_amination_grow_time < heart_beat_amination_shrink_time)
                {
                    float ratio = (game_over_animation_timer - heart_beat_amination_grow_time) / heart_beat_amination_shrink_time;

                    ratio *= ratio * ratio * ratio;

                    ui_life_image.scale = (heart_beat_scale_grow * (1f - ratio)) + (heart_beat_scale_default * ratio);
                }
                else
                {
                    //ui_life_image.scale = heart_beat_scale_default;
                }
            }
        }
        public override void Draw(SpriteBatch sprite_batch)
        {
        }
    }
}
