using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;
using MonoGame.Extended.Screens;

namespace old_heart
{
    public class scene_gameplay : base_screen       // will be uselater                not use now
    {
        private SpriteFont font;

        public string current_map_file;

        public scene_gameplay(Game1 game) : base(game)
        {
        }
        public override void LoadContent()
        {
            base.LoadContent();

            font = Content.Load<SpriteFont>("assets/font/test_font");

            game_manager.level_manager.set_level_file("test_1.json");
        }
        public override void Update(GameTime gameTime)
        {
            if (global.input.keyboard_state.WasKeyPressed(Keys.V))
            {
                ScreenManager.ReplaceScreen(new scene_main_menu(game_ref), fade_transition);
            }


            update_all(gameTime);
        }
    }
}