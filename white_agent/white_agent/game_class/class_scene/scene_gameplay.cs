using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.IO;

namespace old_heart
{
    public class scene_gameplay : scene_base  // copy of test level for now
    {
        public ui_text test_text;
        public ui_text test_text_2;
        public ui_text test_text_3;
        public image live_image;
        
        public image dna_image;
        public ui_text dna_amount;

        public string level_file;

        public ui_upgrade_bar ui_upgrade_bar;
        public scene_gameplay(Game1 game, string level_file) : base(game)
        {
            this.level_file = level_file;

            global.change_mouse_state(global.mouse_state.combat);
        }
        public override void LoadContent()
        {
            test_text = new ui_text("x 0", new Vector2(80, 20));
            //test_text.text_color = Color.Black;
            test_text.text_scale = new Vector2(0.5f, 0.5f);
            game_manager.add_ui(test_text);

            dna_amount = new ui_text("x 0", new Vector2(80, 70));
            dna_amount.text_scale = new Vector2(0.5f, 0.5f);
            game_manager.add_ui(dna_amount);

            ui_upgrade_bar = new ui_upgrade_bar(Content, game_manager , game.run_data_manager);
            game_manager.add_ui(ui_upgrade_bar);

            test_text_2 = new ui_text("Dash cooldown : 0", new Vector2(10, 510));
            //test_text_2.text_color = Color.Black;
            test_text_2.text_scale = new Vector2(0.5f, 0.5f);
            //game_manager.add_ui(test_text_2);

            test_text_3 = new ui_text(" [Esc]\nPause", new Vector2(550, 300));
            test_text_3.text_scale = new Vector2(1.5f, 1.5f);
            test_text_3.text_color *= 0.5f; // transparent
            test_text_3.visible = false;
            game_manager.add_ui(test_text_3);

            live_image = new image(game.Content, new Vector2(20, 20),file_path: "assets/image/ui/ui_life");
            game_manager.add_ui(live_image);
            dna_image = new image(game.Content, new Vector2(20, 70), file_path: "assets/image/ui/ui_dna");
            game_manager.add_ui(dna_image);
            game_manager.level_manager.set_level_file(level_file, game.run_data_manager);

            base.LoadContent();
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
                game.change_scene(new scene_main_menu(game), fade_transition);
            }
            //if (global.input.keyboard_state.WasKeyPressed(Keys.B))
            //{
            //    game.change_scene(new scene_level_editor(game), fade_transition);
            //}



            if (game_manager.player != null)
            {
                test_text.text_string = $"x " +
                $"{game_manager.player.run_data.respawn_left}" +
                $"";

                dna_amount.text_string = $"x {game_manager.player.run_data.dna_temporary}";
            }


            update_all(gameTime);
        }
    }
}