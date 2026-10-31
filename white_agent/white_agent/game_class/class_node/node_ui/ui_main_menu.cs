using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace old_heart
{
    public class ui_main_menu : node
    {
        public run_data_manager run_data;

        public enum result { none, new_game , resume_game , exit , setting};
        public result current_result = result.none;

        public image ui_game_logo;
        public ui_text ui_version_text;

        public Point first_button_position = new Point(60, 270); 
        public Point button_offset = new Point(0, 60);
        public Point button_size = new Point(300, 50);
        float button_text_alpha = 0.6f;  // when not hover
        
        public ui_button ui_new_game_button;
        public ui_text ui_new_game_text;

        public ui_button ui_resume_button;
        public ui_text ui_resume_text;

        public ui_button ui_setting_button;
        public ui_text ui_setting_text;

        public ui_button ui_exit_button;
        public ui_text ui_exit_text;
        public ui_main_menu(ContentManager content,  game_manager game_manager , run_data_manager run_data)
        {
            this.run_data = run_data;
            Vector2 render_size = global.render_size;
            Point button_center_offset = new Point(0, button_size.Y / 2);

            ui_game_logo = new image(content, new Vector2(40, 30), file_path: "assets/image/ui/ui_gametitle");
            game_manager.add_ui(ui_game_logo);

            ui_version_text = new ui_text("version : 0.1", render_size - new Vector2(10,10));
            ui_version_text.origin = ui_version_text.text_font.MeasureString(ui_version_text.text_string);
            ui_version_text.text_scale = new Vector2(0.25f, 0.25f);
            game_manager.add_ui(ui_version_text);

            ui_new_game_button = new ui_button(content, new Rectangle(first_button_position - button_center_offset, button_size));
            ui_new_game_button.visible = false;
            game_manager.add_ui(ui_new_game_button);

            ui_new_game_text = new ui_text("NEW GAME",  new Vector2 (ui_new_game_button.button_rectangle.Left + 10, ui_new_game_button.button_rectangle.Center.Y), new Vector2(0.5f, 0.5f), Color.White);
            ui_new_game_text.origin = ui_new_game_text.text_font.MeasureString(ui_new_game_text.text_string)/ 2 * new Vector2(0, 1);
            game_manager.add_ui(ui_new_game_text);

            ui_resume_button = new ui_button(content, new Rectangle(first_button_position + button_offset - button_center_offset, button_size));
            ui_resume_button.visible = false;
            ui_resume_button.active = false;
            game_manager.add_ui(ui_resume_button);

            ui_resume_text = new ui_text("RESUME GAME", new Vector2(ui_resume_button.button_rectangle.Left + 10, ui_resume_button.button_rectangle.Center.Y), new Vector2(0.5f, 0.5f), Color.White);
            ui_resume_text.origin = ui_resume_text.text_font.MeasureString(ui_resume_text.text_string) / 2 * new Vector2 (0,1);
            ui_resume_text.visible = false;
            game_manager.add_ui(ui_resume_text);

            ui_setting_button = new ui_button(content, new Rectangle(first_button_position + button_offset * 2 - button_center_offset, button_size));
            ui_setting_button.visible = false;
            game_manager.add_ui(ui_setting_button);

            ui_setting_text = new ui_text("SETTING", new Vector2(ui_setting_button.button_rectangle.Left + 10, ui_setting_button.button_rectangle.Center.Y), new Vector2(0.5f, 0.5f), Color.White);
            ui_setting_text.origin = ui_setting_text.text_font.MeasureString(ui_setting_text.text_string) / 2 * new Vector2(0, 1);
            game_manager.add_ui(ui_setting_text);

            ui_exit_button = new ui_button(content, new Rectangle(first_button_position + button_offset * 3 - button_center_offset, button_size));
            ui_exit_button.visible = false;
            game_manager.add_ui(ui_exit_button);

            ui_exit_text = new ui_text("EXIT GAME", new Vector2(ui_exit_button.button_rectangle.Left + 10, ui_exit_button.button_rectangle.Center.Y), new Vector2(0.5f, 0.5f), Color.White);
            ui_exit_text.origin = ui_exit_text.text_font.MeasureString(ui_exit_text.text_string) / 2 * new Vector2(0, 1);
            game_manager.add_ui(ui_exit_text);
        }
        public override void Update(GameTime gameTime)
        {
            if (run_data == null) { return; }
            float delta_time = (float)gameTime.ElapsedGameTime.TotalSeconds;

            ui_new_game_text.alpha = button_text_alpha;
            ui_resume_text.alpha = button_text_alpha;
            ui_setting_text.alpha = button_text_alpha;
            ui_exit_text.alpha = button_text_alpha;

            if (ui_new_game_button.mouse_hover)
            {
                ui_new_game_text.alpha = 1;
            }
            else if (ui_resume_button.mouse_hover)
            {
                ui_resume_text.alpha = 1;
            }
            else if (ui_setting_button.mouse_hover)
            {
                ui_setting_text.alpha = 1;
            }
            else if (ui_exit_button.mouse_hover)
            {
                ui_exit_text.alpha = 1;
            }

            if (current_result != result.none) { return; }

            if (run_data.cleared_level != 0)
            {
                ui_resume_text.visible = true;
                ui_resume_button.active = true;
            }
            else
            {
                ui_resume_text.visible = false;
                ui_resume_button.active = false;
            }

            if (ui_new_game_button.clicked)
            {
                current_result = result.new_game;
            }
            else if (ui_resume_button.clicked)
            {
                if (run_data.cleared_level < run_data.level_list.Count)  // still has next level
                {
                    current_result = result.resume_game;
                }
            }
            else if (ui_setting_button.clicked)
            {
                current_result = result.setting;
            }
            else if (ui_exit_button.clicked)
            {
                current_result = result.exit;
            }



        }
        public override void Draw(SpriteBatch sprite_batch)
        {
        }
    }
}
