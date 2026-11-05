using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;

namespace old_heart
{
    public class ui_cut_scene : node
    {
        public run_data_manager run_data;
        public enum result { none , finished };
        public result current_result = result.none;

        public image black_screen;

        public image black_background;
        ui_button ui_next_button;

        public ui_text ui_ending_text_1;
        public ui_text ui_ending_text_2;

        public float cut_scene_animation_time = 2f;
        public float cut_scene_animation_timer = 0;

        public int current_cut_scene_step = 0;

        public List<image> ui_intro_image_list = new List<image> { };
        public List<image> ui_ending_image_list = new List<image> { };

        public ui_text ui_click_text;

        public ui_cut_scene(ContentManager content, game_manager game_manager, run_data_manager run_data)
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

            ui_next_button = new ui_button(content, new Rectangle(Point.Zero, render_size.ToPoint()));
            ui_next_button.hover_sound_enable = false;
            ui_next_button.active = false;
            ui_next_button.visible = false;
            game_manager.add_ui(ui_next_button);

            ui_ending_text_1 = new ui_text("To be continue...", Vector2.Zero, text_scale: new Vector2(0.5f, 0.5f), text_color: Color.DarkGray);
            ui_ending_text_1.position = render_size * new Vector2(0.5f, 0.5f) - ui_ending_text_1.text_font.MeasureString(ui_ending_text_1.text_string) * ui_ending_text_1.text_scale / 2;
            ui_ending_text_1.visible = false;
            game_manager.add_ui(ui_ending_text_1);

            ui_ending_text_2 = new ui_text("Thanks for playing", Vector2.Zero, text_scale: new Vector2(1f, 1f), text_color: Color.Yellow);
            ui_ending_text_2.position = render_size * new Vector2(0.5f, 0.67f) - ui_ending_text_2.text_font.MeasureString(ui_ending_text_2.text_string) * ui_ending_text_2.text_scale / 2;
            ui_ending_text_2.visible = false;
            game_manager.add_ui(ui_ending_text_2);

            Vector2 image_spacing = render_size * new Vector2(0.3f, 0f); // position of center of image spacing
            Vector2 image_spacing_row_offset = render_size * new Vector2(0f, 0.4f); // next row offset
            for (int i = 0; i < 6; i++)
            {
                Vector2 image_position = (render_size * new Vector2(0.5f, 0.2f) + ((i - 1) * image_spacing)); // center of image
                if (i >= 3)
                {
                    image_position += (-3 * image_spacing) + image_spacing_row_offset;
                }
                image new_image = new image(content, image_position, file_path: "assets/image/cutscenes/cutscene_start_0" + (i+1));
                new_image.origin = new_image.texture.Bounds.Center.ToVector2();
                new_image.visible = false;
                ui_intro_image_list.Add(new_image);
                game_manager.add_ui(new_image);
            }
            for (int i = 0; i < 3; i++)
            {
                Vector2 image_position = (render_size * new Vector2(0.5f, 0.2f) + ((i - 1) * image_spacing)); // center of image
                image new_image = new image(content, image_position, file_path: "assets/image/cutscenes/cutscene_end_0" + (i + 1));
                new_image.origin = new_image.texture.Bounds.Center.ToVector2();
                new_image.visible = false;
                ui_ending_image_list.Add(new_image);
                game_manager.add_ui(new_image);
            }

            ui_click_text = new ui_text("Click to proceed", render_size * new Vector2(0.5f, 0.9f) , text_scale: new Vector2(0.5f, 0.5f), text_color: new Color (50,50,50));
            ui_click_text.origin = ui_ending_text_1.text_font.MeasureString(ui_ending_text_1.text_string)/ 2;
            ui_click_text.visible = false;
            game_manager.add_ui(ui_click_text);

        }
        public override void Update(GameTime gameTime)
        {
            if (!active) return;

            float delta_time = (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (black_background.visible == false)
            {
                cut_scene_animation_timer = 0;
                current_cut_scene_step = 0;
                black_background.visible = true;
                ui_next_button.active = true;
                ui_click_text.visible = true;
            }

            if (current_result != result.none) { return; }


            cut_scene_animation_timer += delta_time;

            if (ui_next_button.clicked)
            {
                proceed_cutscene();
            }
            else if (cut_scene_animation_timer > cut_scene_animation_time)
            {
                if (run_data.cleared_level == 0 && current_cut_scene_step > 5)  // almost finish intro cutscene
                {
                    // do nothing
                }
                else if (run_data.cleared_level != 0 && current_cut_scene_step > 5)// almost finish ending cutscene
                {
                    // do nothing
                }
                else
                {
                    proceed_cutscene();
                }
            }

            if (run_data.cleared_level == 0 && current_cut_scene_step > 6)  // finish intro cutscene
            {
                current_result = result.finished;
                active = false;
            }
            else if (run_data.cleared_level != 0 && current_cut_scene_step > 6)// finish ending cutscene
            {
                current_result = result.finished;
                active = false;
                set_ui_visible(false);
            }


            void proceed_cutscene()
            {
                cut_scene_animation_timer = 0;
                current_cut_scene_step++;
                if (run_data.cleared_level == 0)
                {
                    for (int i = 0; i < ui_intro_image_list.Count; i++)
                    {
                        if (i >= current_cut_scene_step)
                        {
                            return;
                        }
                        image image = ui_intro_image_list[i];
                        image.visible = true;
                    }
                }
                else if (run_data.cleared_level != 0)
                {
                    for (int i = 0; i < ui_ending_image_list.Count; i++)
                    {
                        if (i >= current_cut_scene_step)
                        {
                            return;
                        }
                        image image = ui_ending_image_list[i];
                        image.visible = true;
                    }
                    if (current_cut_scene_step >= 4)
                    {
                        ui_ending_text_1.visible = true;
                    }
                    if (current_cut_scene_step >= 5)
                    {
                        ui_ending_text_2.visible = true;
                    }
                }
            }

            void set_ui_visible(bool visible)
            {
                black_background.visible = visible;
                ui_next_button.active = visible;
                ui_click_text.visible = visible;
                foreach (image image in ui_intro_image_list)
                {
                    image.visible = false;
                }
                foreach (image image in ui_ending_image_list)
                {
                    image.visible = false;
                }
                ui_ending_text_1.visible = visible;
                ui_ending_text_2.visible = visible;
            }
        }
        public override void Draw(SpriteBatch sprite_batch)
        {
        }
    }
}
