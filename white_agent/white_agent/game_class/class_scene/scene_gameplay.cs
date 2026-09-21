using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended.Screens;
using System;
using System.IO;

namespace old_heart
{
    public class scene_gameplay : base_screen  // copy of test level for now
    {
        public ui_text test_text;
        public ui_text test_text_2;
        public ui_text test_text_3;
        public image live_image;

        public string level_file;

        public scene_gameplay(Game1 game, string level_file) : base(game)
        {
            if (!level_file.EndsWith(".json", StringComparison.OrdinalIgnoreCase))
            {
                level_file += ".json";
            }
            this.level_file = level_file;

            global.change_mouse_state(global.mouse_state.combat);
        }
        public override void LoadContent()
        {
            base.LoadContent();

            test_text = new ui_text("0", new Vector2(80, 10));
            //test_text.text_color = Color.Black;
            test_text.text_scale = new Vector2(1f, 1f);
            game_manager.add_ui(test_text);

            test_text_2 = new ui_text("Dash cooldown : 0", new Vector2(10, 510));
            //test_text_2.text_color = Color.Black;
            test_text_2.text_scale = new Vector2(0.5f, 0.5f);
            //game_manager.add_ui(test_text_2);

            test_text_3 = new ui_text(" [Esc]\nPause", new Vector2(550, 250));
            test_text_3.text_scale = new Vector2(2f, 2f);
            test_text_3.text_color *= 0.5f; // transparent
            test_text_3.visible = false;
            game_manager.add_ui(test_text_3);

            live_image = new image(game.Content, new Vector2(20, 10),"Placeholder/Weapons/Head");
            game_manager.add_ui(live_image);

            game_manager.level_manager.set_level_file(level_file, game.run_data_manager);
        }
        public override void Update(GameTime gameTime)
        {
            if (global.input.keyboard_state.WasKeyPressed(Keys.Escape)){
                if (game_manager.pause == true)
                {
                    game_manager.pause = false;
                    test_text_3.visible = false;
                }
                else
                {
                    game_manager.pause = true;
                    test_text_3.visible = true;
                }
            }
            else if (global.input.keyboard_state.WasKeyPressed(Keys.V))
            {
                ScreenManager.ReplaceScreen(new scene_main_menu(game), fade_transition);
            }
            if (global.input.keyboard_state.WasKeyPressed(Keys.B))
            {
                ScreenManager.ReplaceScreen(new scene_level_editor(game), fade_transition);
            }



            if (game_manager.player != null)
            {
                test_text.text_string = $""+
                $"{game_manager.player.run_data.respawn_left}" +
                $"";
            }


            update_all(gameTime);
        }
    }
}