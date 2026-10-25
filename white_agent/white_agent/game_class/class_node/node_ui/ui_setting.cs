using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Diagnostics;
namespace old_heart
{
    public class ui_setting : node
    {
        public run_data_manager run_data;
        public enum result { none, leave , reset_data};
        public result current_result = result.none;

        public Vector2 setting_top_left_position = new Vector2(230,80);
        public Vector2 setting_spacing = new Vector2(400, 50);
        public float setting_button_spacing_distance = 60;
        public Point setting_button_size = new Point(40,40);
        public Point setting_toggle_button_size = new Point(100, 40);

        public image black_background;
        public ui_button ui_return_button;
        public ui_text ui_return_text;

        public ui_button ui_reset_button;
        public ui_text ui_reset_text;

        public List<ui_text> ui_setting_text_list = new List<ui_text> { };
        public List<ui_text> ui_setting_value_text_list = new List<ui_text> { };
        public List<ui_button> ui_setting_decrease_button_list = new List<ui_button> { };
        public List<ui_button> ui_setting_increase_button_list = new List<ui_button> { };
        public List<ui_text> ui_setting_button_text_list = new List<ui_text> { }; // for - and +
        public List<ui_button> ui_setting_toggle_button_list = new List<ui_button> { };
        public List<ui_text> ui_setting_toggle_button_text_list = new List<ui_text> { }; // for true and false
        public ui_setting(ContentManager content  , game_manager game_manager, run_data_manager run_data)
        {
            this.run_data = run_data;
            active = false;

            Texture2D white_pixel = content.Load<Texture2D>("assets/image/other/white_pixel");
            black_background = new image(content, Vector2.Zero, texture: white_pixel);
            black_background.scale = global.render_size;
            black_background.color = Color.Black;
            black_background.visible = false;
            game_manager.add_ui(black_background);

            Texture2D texture_leave_button_unselect = content.Load<Texture2D>("assets/image/ui/ui_button_leave_unselect");
            Texture2D texture_leave_button_selected = content.Load<Texture2D>("assets/image/ui/ui_button_leave_selected");
            ui_return_button = new ui_button(content, new Rectangle(new Point(820, 475), texture_leave_button_unselect.Bounds.Size));
            ui_return_button.button_texture = texture_leave_button_unselect;
            ui_return_button.hover_texture = texture_leave_button_selected;
            ui_return_button.visible = false;
            ui_return_button.active = false;
            game_manager.add_ui(ui_return_button);

            ui_return_text = new ui_text("Return", Vector2.Zero, text_scale: new Vector2(0.3f, 0.3f), text_color: new Color(89, 89, 89));
            ui_return_text.position = ui_return_button.button_rectangle.Center.ToVector2() - ui_return_text.text_font.MeasureString(ui_return_text.text_string) * ui_return_text.text_scale / 2;
            ui_return_text.visible = false;
            game_manager.add_ui(ui_return_text);


            ui_reset_button = new ui_button(content, new Rectangle(20, 470, 150, 50));
            ui_reset_button.visible = false;
            game_manager.add_ui(ui_reset_button);

            ui_reset_text = new ui_text("reset data", ui_reset_button.button_rectangle.Center.ToVector2(), new Vector2(0.25f, 0.25f), Color.Black);
            ui_reset_text.origin = ui_reset_text.text_font.MeasureString(ui_reset_text.text_string) / 2;
            ui_reset_text.visible = false;
            game_manager.add_ui(ui_reset_text);

            make_ui_setting("Master volume", setting_top_left_position);
            make_ui_setting("Sound effect volume", setting_top_left_position + new Vector2(0,setting_spacing.Y));
            make_ui_setting("Music volume", setting_top_left_position + new Vector2(0, setting_spacing.Y * 2));
            make_ui_setting("Screen shake intensity", setting_top_left_position + new Vector2(0, setting_spacing.Y * 4));
            make_ui_setting("Full screen", setting_top_left_position + new Vector2(0, setting_spacing.Y * 6) , toggle_button:true);

            void make_ui_setting(string text ,Vector2 position , bool toggle_button = false)
            {
                if (!toggle_button)
                {
                    ui_button new_ui_setting_decrease_button = new ui_button(content, new Rectangle((position + new Vector2(setting_spacing.X - setting_button_spacing_distance,0)).ToPoint() - setting_button_size/2 , setting_button_size));
                    new_ui_setting_decrease_button.visible = false;
                    ui_setting_decrease_button_list.Add(new_ui_setting_decrease_button);
                    game_manager.add_ui(new_ui_setting_decrease_button);


                    ui_button new_ui_setting_increase_button = new ui_button(content, new Rectangle((position + new Vector2(setting_spacing.X + setting_button_spacing_distance, 0)).ToPoint() - setting_button_size / 2, setting_button_size));
                    new_ui_setting_increase_button.visible = false;
                    ui_setting_increase_button_list.Add(new_ui_setting_increase_button);
                    game_manager.add_ui(new_ui_setting_increase_button);

                    ui_text new_ui_setting_button_text = new ui_text("-", new_ui_setting_decrease_button.button_rectangle.Center.ToVector2() , new Vector2(0.5f, 0.5f), Color.Black);
                    new_ui_setting_button_text.origin = new_ui_setting_button_text.text_font.MeasureString("-") / 2;
                    new_ui_setting_button_text.visible = false;
                    ui_setting_button_text_list.Add(new_ui_setting_button_text);
                    game_manager.add_ui(new_ui_setting_button_text);

                    new_ui_setting_button_text = new ui_text("+", new_ui_setting_increase_button.button_rectangle.Center.ToVector2(), new Vector2(0.5f, 0.5f), Color.Black);
                    new_ui_setting_button_text.origin = new_ui_setting_button_text.text_font.MeasureString("-") / 2;
                    new_ui_setting_button_text.visible = false;
                    ui_setting_button_text_list.Add(new_ui_setting_button_text);
                    game_manager.add_ui(new_ui_setting_button_text);
                }
                else
                {
                    ui_button new_ui_setting_toggle_button = new ui_button(content, new Rectangle((position + new Vector2(setting_spacing.X, 0)).ToPoint() - setting_toggle_button_size / 2, setting_toggle_button_size));
                    new_ui_setting_toggle_button.visible = false;
                    ui_setting_toggle_button_list.Add(new_ui_setting_toggle_button);
                    game_manager.add_ui(new_ui_setting_toggle_button);

                    ui_text new_ui_setting_button_text = new ui_text(" ", new_ui_setting_toggle_button.button_rectangle.Center.ToVector2(), new Vector2(0.25f, 0.25f), Color.Black);
                    new_ui_setting_button_text.visible = false;
                    ui_setting_toggle_button_text_list.Add(new_ui_setting_button_text);
                    game_manager.add_ui(new_ui_setting_button_text);
                }

                ui_text new_ui_setting_text = new ui_text(text, position, new Vector2(0.25f, 0.25f), Color.White);
                new_ui_setting_text.origin = new_ui_setting_text.text_font.MeasureString("00") * new Vector2(0,0.5f);
                new_ui_setting_text.visible = false;
                ui_setting_text_list.Add(new_ui_setting_text);
                game_manager.add_ui(new_ui_setting_text);

                ui_text new_ui_setting_value_text = new ui_text(" ", position + new Vector2(setting_spacing.X,0), new Vector2(0.25f, 0.25f), Color.White);
                new_ui_setting_value_text.text_font = global.theme.default_font_1;
                new_ui_setting_value_text.origin = new_ui_setting_value_text.text_font.MeasureString("00") / 2;
                new_ui_setting_value_text.visible = false;
                ui_setting_value_text_list.Add(new_ui_setting_value_text);
                game_manager.add_ui(new_ui_setting_value_text);

            }
        }

        public override void Update(GameTime gameTime)
        {
            if (!active) return; // make it not work when it is inactive

            if (black_background.visible == false)  // open setting menu
            {
                set_ui_visible(true);
            }

            if (ui_return_button.mouse_hover)
            {
                ui_return_text.text_color = Color.White;
            }
            else
            {
                ui_return_text.text_color = new Color(89, 89, 89);
            }

            for (int i = 0; i < ui_setting_text_list.Count; i++)
            {
                int toggle_index = i - (ui_setting_decrease_button_list.Count);
                ui_text ui_setting_value_text = ui_setting_value_text_list[i];
                float setting_value_change = 0f;

                if (i < ui_setting_decrease_button_list.Count)  // not toggle button
                {
                    ui_button ui_setting_decrease_button = ui_setting_decrease_button_list[i];
                    ui_button ui_setting_increase_button = ui_setting_increase_button_list[i];

                    if (ui_setting_decrease_button.clicked)
                    {
                        setting_value_change = -0.1f;
                    }
                    else if (ui_setting_increase_button.clicked)
                    {
                        setting_value_change = 0.1f;
                    }
                }
                else // toggle button
                {
                    ui_button ui_setting_toggle_button = ui_setting_toggle_button_list[toggle_index];

                    if (ui_setting_toggle_button.clicked)
                    {
                        setting_value_change = 1f;
                    }
                }


                if (i == 0)
                {
                    global.setting.volume_master = Math.Clamp(global.setting.volume_master + setting_value_change, 0f, 1f);
                    ui_setting_value_text.text_string = Math.Round(global.setting.volume_master * 100).ToString() + "%";
                    ui_setting_value_text.origin = ui_setting_value_text.text_font.MeasureString(ui_setting_value_text.text_string) / 2;
                    if (setting_value_change != 0f)
                    {
                        global.sound.play_sound(global.sound.sound_name.enemy_gethit);
                        global.setting.apply_song_volume_setting();
                    }
                }
                else if (i == 1)
                {
                    global.setting.volume_sound_effect = Math.Clamp(global.setting.volume_sound_effect + setting_value_change, 0f, 1f);
                    ui_setting_value_text.text_string = Math.Round(global.setting.volume_sound_effect * 100).ToString() + "%";
                    ui_setting_value_text.origin = ui_setting_value_text.text_font.MeasureString(ui_setting_value_text.text_string) / 2;
                    if (setting_value_change != 0f)
                    {
                        global.sound.play_sound(global.sound.sound_name.enemy_gethit);
                    }
                }
                else if (i == 2)
                {
                    global.setting.volume_song = Math.Clamp(global.setting.volume_song + setting_value_change, 0f, 1f);
                    ui_setting_value_text.text_string = Math.Round(global.setting.volume_song * 100).ToString() + "%";
                    ui_setting_value_text.origin = ui_setting_value_text.text_font.MeasureString(ui_setting_value_text.text_string) / 2;
                    if (setting_value_change != 0f)
                    {
                        global.setting.apply_song_volume_setting();
                    }
                }
                else if (i == 3)
                {
                    global.setting.screen_shake_intensity = Math.Clamp(global.setting.screen_shake_intensity + setting_value_change, 0f, 3f);
                    ui_setting_value_text.text_string = Math.Round(global.setting.screen_shake_intensity * 100).ToString() + "%";
                    ui_setting_value_text.origin = ui_setting_value_text.text_font.MeasureString(ui_setting_value_text.text_string) / 2;
                }
                else if (i == 4)
                {
                    if (setting_value_change == 1f)
                    {
                        if (global.setting.full_screen == true)
                        {
                            global.setting.full_screen = false;
                        }
                        else
                        {
                            global.setting.full_screen = true;
                        }
                    }
                    ui_text ui_setting_toggle_button_text = ui_setting_toggle_button_text_list[toggle_index];
                    ui_setting_toggle_button_text.text_string = global.setting.full_screen.ToString();
                    ui_setting_toggle_button_text.origin = ui_setting_toggle_button_text.text_font.MeasureString(ui_setting_toggle_button_text.text_string) / 2;
                }


            }

            if (current_result != result.none) { return; }

            if (ui_reset_button.clicked)
            {
                current_result = result.reset_data;
            }
            else if (ui_return_button.clicked) // close setting
            {
                ui_return_button.clicked = false;
                set_ui_visible(false);
                current_result = result.leave;
                return;
            }


            void set_ui_visible(bool visible)
            {
                black_background.visible = visible;
                ui_return_button.visible = visible;
                ui_return_button.active = visible;
                ui_return_text.visible = visible;
                ui_reset_button.visible = visible;
                ui_reset_text.visible = visible;
                foreach (ui_text ui_text in ui_setting_text_list)
                {
                    ui_text.visible = visible;
                }
                foreach (ui_text ui_text in ui_setting_value_text_list)
                {
                    ui_text.visible = visible;
                }
                foreach (ui_button ui_Button in ui_setting_decrease_button_list)
                {
                    ui_Button.visible = visible;
                }
                foreach (ui_button ui_Button in ui_setting_increase_button_list)
                {
                    ui_Button.visible = visible;
                }
                foreach (ui_button ui_Button in ui_setting_toggle_button_list)
                {
                    ui_Button.visible = visible;
                }
                foreach (ui_text ui_text in ui_setting_toggle_button_text_list)
                {
                    ui_text.visible = visible;
                }
                foreach (ui_text ui_text in ui_setting_button_text_list)
                {
                    ui_text.visible = visible;
                }
            }
        }

        public override void Draw(SpriteBatch sprite_batch)
        {
            if (!visible) return; // make it not work when it is invisible
        }

    }
}
