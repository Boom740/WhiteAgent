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
        public scene_main_menu(Game1 game) : base(game)
        {
            game_ref = game;
        }
        public override void LoadContent()
        {
            base.LoadContent();

            test_text = new ui_text("Chess Battle Advanced\nclick play to play\nEsc to quit\nV test thing scene\nB level editor",  new Vector2(10, 5));
            game_manager.add_ui(test_text);
            start_button = new ui_button(Content, new Rectangle(100, 400, 200, 100));
            game_manager.add_ui(start_button);
            test_text = new ui_text("PLAY", new Vector2(130, 420));
            game_manager.add_ui(test_text);


        }

        public override void Update(GameTime gameTime)
        {
            if (global.input.keyboard_state.WasKeyPressed(Keys.V))
            {
                ScreenManager.ReplaceScreen(new scene_test_anything(game_ref), fade_transition);
            }
            else if (start_button.clicked)
            {
                ScreenManager.ReplaceScreen(new test_level(game_ref,"test_1.json"), fade_transition);
            }
            else if (global.input.keyboard_state.WasKeyPressed(Keys.B))
            {
                ScreenManager.ReplaceScreen(new scene_level_editor(game_ref), fade_transition);
            }

            update_all(gameTime);
        }
        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);
        }
    }
}