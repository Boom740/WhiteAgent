using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended.Screens;

namespace old_heart
{
    public class scene_main_menu : base_screen
    {
        public ui_button start_button;
        public ui_text test_text;
        public ui_text test_text_2;
        public ui_button reset_button;
        public scene_main_menu(Game1 game) : base(game)
        {
        }
        public override void LoadContent()
        {
            base.LoadContent();

            test_text = new ui_text("Chess Battle Advanced\nclick play to play\nEsc to quit\nV test thing scene\nB level editor\nF full screen",  new Vector2(10, 5));
            game_manager.add_ui(test_text);

            test_text_2 = new ui_text("replace in update ", new Vector2(10, 500));
            test_text_2.text_scale = new Vector2(0.25f, 0.25f);
            game_manager.add_ui(test_text_2);

            start_button = new ui_button(Content, new Rectangle(100, 300, 200, 100));
            game_manager.add_ui(start_button);

            ui_text test_text_local;

            test_text_local = new ui_text("PLAY", new Vector2(130, 330));
            test_text_local.text_color = Color.Black;
            game_manager.add_ui(test_text_local);

            reset_button = new ui_button(Content, new Rectangle(700, 400, 150, 50));
            game_manager.add_ui(reset_button);

            test_text_local = new ui_text("reset data", new Vector2(720, 415));
            test_text_local.text_color = Color.Black;
            test_text_local.text_scale = new Vector2(0.25f, 0.25f);
            game_manager.add_ui(test_text_local);

        }

        public override void Update(GameTime gameTime)
        {
            if (global.input.keyboard_state.WasKeyPressed(Keys.Escape)){
                game.Exit();
            }
            else if (global.input.keyboard_state.WasKeyPressed(Keys.V))
            {
                ScreenManager.ReplaceScreen(new scene_test_anything(game), fade_transition);
            }
            else if (start_button.clicked)
            {
                if (game.run_data_manager.cleared_level < game.run_data_manager.level_list.Count)  // still has next level
                {
                    ScreenManager.ReplaceScreen(new scene_gameplay(game, game.run_data_manager.level_list[game.run_data_manager.cleared_level]), fade_transition);
                }
            }
            else if (reset_button.clicked)
            {
                game.run_data_manager = new run_data_manager(game);
            }
            else if (global.input.keyboard_state.WasKeyPressed(Keys.B))
            {
                ScreenManager.ReplaceScreen(new scene_level_editor(game), fade_transition);
            }
            else if (global.input.keyboard_state.WasKeyPressed(Keys.F))
            {
                game._graphics.ToggleFullScreen();
            }

            test_text_2.text_string = $"level cleared : {game.run_data_manager.cleared_level} / {game.run_data_manager.level_list.Count}" +
            $"\nlife left : {game.run_data_manager.respawn_left}" +
            $"";
                

            update_all(gameTime);
        }
        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);
        }
    }
}