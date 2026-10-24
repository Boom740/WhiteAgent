using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace old_heart
{
    public class ui_main_menu : node
    {
        public run_data_manager run_data;

        public enum result { none, start , exit , setting};
        public result current_result = result.none;

        public ui_text test_text;
        public ui_text test_text_2;

        public ui_button ui_start_button;
        public ui_text ui_start_text;

        public ui_button ui_setting_button;
        public ui_text ui_setting_text;

        public ui_button ui_exit_button;
        public ui_text ui_exit_text;
        public ui_main_menu(ContentManager content,  game_manager game_manager , run_data_manager run_data)
        {
            this.run_data = run_data;

            test_text = new ui_text("White Agent\n\nV test thing scene\nB level editor", new Vector2(10, 5));
            game_manager.add_ui(test_text);

            test_text_2 = new ui_text("replace in update ", new Vector2(10, 500));
            test_text_2.text_scale = new Vector2(0.25f, 0.25f);
            game_manager.add_ui(test_text_2);

            ui_start_button = new ui_button(content, new Rectangle(50, 250, 200, 100));
            game_manager.add_ui(ui_start_button);

            ui_start_text = new ui_text("PLAY", ui_start_button.button_rectangle.Center.ToVector2(), new Vector2(0.5f, 0.5f), Color.Black);
            ui_start_text.origin = ui_start_text.text_font.MeasureString(ui_start_text.text_string) / 2;
            game_manager.add_ui(ui_start_text);

            ui_setting_button = new ui_button(content, new Rectangle(50, 360, 150, 40));
            game_manager.add_ui(ui_setting_button);

            ui_setting_text = new ui_text("Setting", ui_setting_button.button_rectangle.Center.ToVector2(), new Vector2(0.25f, 0.25f), Color.Black);
            ui_setting_text.origin = ui_setting_text.text_font.MeasureString(ui_setting_text.text_string) / 2;
            game_manager.add_ui(ui_setting_text);

            ui_exit_button = new ui_button(content, new Rectangle(50, 410, 150, 40));
            game_manager.add_ui(ui_exit_button);

            ui_exit_text = new ui_text("Exit", ui_exit_button.button_rectangle.Center.ToVector2(), new Vector2(0.25f, 0.25f), Color.Black);
            ui_exit_text.origin = ui_exit_text.text_font.MeasureString(ui_exit_text.text_string) / 2;
            game_manager.add_ui(ui_exit_text);
        }
        public override void Update(GameTime gameTime)
        {
            if (run_data == null) { return; }
            float delta_time = (float)gameTime.ElapsedGameTime.TotalSeconds;


            if (current_result != result.none) { return; }

            else if (ui_start_button.clicked)
            {
                if (run_data.cleared_level < run_data.level_list.Count)  // still has next level
                {
                    current_result = result.start;
                }
            }
            else if(ui_setting_button.clicked)
            {
                current_result = result.setting ;
            }
            else if (ui_exit_button.clicked)
            {
                current_result = result.exit;
            }

            test_text_2.text_string = $"level cleared : {run_data.cleared_level} / {run_data.level_list.Count}" +
            $"\nlife left : {run_data.respawn_left}" +
            $"";


        }
        public override void Draw(SpriteBatch sprite_batch)
        {
        }
    }
}
