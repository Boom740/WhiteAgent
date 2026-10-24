using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace old_heart
{
    public class scene_main_menu : scene_base
    {
        public ui_main_menu ui_main_menu;

        public scene_main_menu(Game1 game) : base(game)
        {
        }
        public override void LoadContent()
        {
            ui_main_menu = new ui_main_menu(Content, game_manager, game.run_data_manager);
            game_manager.add_ui(ui_main_menu);

            base.LoadContent();
        }

        public override void Update(GameTime gameTime)
        {

            if (ui_setting.current_result == ui_setting.result.reset_data)
            {
                game.run_data_manager = new run_data_manager();
                ui_setting.run_data = game.run_data_manager;
                ui_main_menu.run_data = game.run_data_manager;
                ui_setting.current_result = ui_setting.result.none;
            }
            if (ui_main_menu.current_result == ui_main_menu.result.setting && ui_setting.active == false)
            {
                ui_setting.current_result = ui_setting.result.none;
                ui_setting.active = true;
            }
            else if (ui_setting.current_result == ui_setting.result.leave && ui_setting.active == true)
            {
                ui_main_menu.current_result = ui_main_menu.result.none;
                ui_setting.active = false;
            }
            else if (ui_main_menu.current_result == ui_main_menu.result.start)
            {
                game.change_scene(new scene_gameplay(game, game.run_data_manager.level_list[game.run_data_manager.cleared_level]), fade_transition);
            }
            else if (ui_main_menu.current_result == ui_main_menu.result.exit)
            {
                game.Exit();
            }
            else if (global.input.keyboard_state.WasKeyPressed(Keys.V))
            {
                game.change_scene(new scene_test_anything(game), fade_transition);
            }
            else if (global.input.keyboard_state.WasKeyPressed(Keys.B))
            {
                game.change_scene(new scene_level_editor(game), fade_transition);
            }

            update_all(gameTime);
        }
        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);
        }
    }
}