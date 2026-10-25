using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace old_heart
{
    public class scene_gameplay : scene_base  // copy of test level for now
    {
        public string level_file;

        public ui_resource ui_resource;

        public ui_pause ui_pause;
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

            ui_pause = new ui_pause(Content, game_manager, run_data: game.run_data_manager);
            game_manager.add_ui(ui_pause);

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
                        if (game_manager.pause == false)
                        {
                            game_manager.pause = true;
                            ui_pause.current_result = ui_pause.result.none;
                            ui_pause.active = true;
                        }
                    }
                }
            }else if (ui_pause.current_result == ui_pause.result.un_pause)
            {
                game_manager.pause = false;
                ui_pause.active = false;
            }
            else if (ui_pause.current_result == ui_pause.result.return_to_main_menu)
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