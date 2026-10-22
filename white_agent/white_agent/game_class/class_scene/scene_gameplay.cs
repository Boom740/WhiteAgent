using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace old_heart
{
    public class scene_gameplay : scene_base  // copy of test level for now
    {
        public string level_file;

        public ui_resource ui_resource;

        public ui_text pause_text;
        public scene_gameplay(Game1 game, string level_file) : base(game)
        {
            this.level_file = level_file;

            global.change_mouse_state(global.mouse_state.combat);
        }
        public override void LoadContent()
        {
            ui_resource = new ui_resource(Content,game_manager, run_data: game.run_data_manager);
            game_manager.add_ui(ui_resource);

            ui_upgrade_bar = new ui_upgrade_bar(Content, game_manager, run_data: game.run_data_manager , ui_shop);
            game_manager.add_ui(ui_upgrade_bar);


            pause_text = new ui_text(" [Esc]\nPause", new Vector2(550, 300));
            pause_text.text_scale = new Vector2(1.5f, 1.5f);
            pause_text.text_color *= 0.5f; // transparent
            pause_text.visible = false;
            game_manager.add_ui(pause_text);

            game_manager.level_manager.set_level_file(level_file, game.run_data_manager);
            if (level_file == "level_boss")
            {
                game_manager.camera_manager.camera.ZoomOut(0.25f);
            }

            base.LoadContent();
        }
        public override void Update(GameTime gameTime)
        {
            if (global.input.keyboard_state.WasKeyPressed(Keys.Escape)){
                if (game_manager.player != null)
                {
                    if (game_manager.player.current_combat_state != player.combat_state.in_shop)
                    {
                        if (game_manager.pause == true)
                        {
                            game_manager.pause = false;
                            pause_text.visible = false;
                        }
                        else
                        {
                            game_manager.pause = true;
                            pause_text.visible = true;
                        }
                    }
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



            


            update_all(gameTime);
        }
    }
}