using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;

namespace old_heart
{
    public class scene_main_menu : scene_base
    {
        public ui_main_menu ui_main_menu;
        public ui_cut_scene ui_cut_scene;
        public scene_main_menu(Game1 game) : base(game)
        {
        }
        public override void LoadContent()
        {
            ui_main_menu = new ui_main_menu(Content, game_manager, game.run_data_manager);
            game_manager.add_ui(ui_main_menu);

            base.LoadContent();

            ui_cut_scene = new ui_cut_scene(Content, game_manager, game.run_data_manager);
            game_manager.add_ui(ui_cut_scene);

            if (game.run_data_manager.cleared_level == game.run_data_manager.level_list.Count)
            {
                ui_cut_scene.active = true;
                ui_main_menu.active = false;
            }
            else
            {
                ui_main_menu.active = true;
            }
        }

        public override void Update(GameTime gameTime)
        {
            
            if (ui_cut_scene.active) 
            { 
                update_all(gameTime); 
                return; 
            }
            else if (ui_cut_scene.current_result == ui_cut_scene.result.finished)
            {
                ui_cut_scene.current_result = ui_cut_scene.result.none;
                if (game.run_data_manager.cleared_level == 0)  // new game cutscene finish
                {
                    game.change_scene(new scene_gameplay(game, game.run_data_manager.level_list[game.run_data_manager.cleared_level]), fade_transition);
                }
                else
                {
                    ui_main_menu.active = true;
                }
            }

            else if (ui_setting.current_result == ui_setting.result.reset_data)
            {
                reset_data();
            }
            else if (ui_main_menu.current_result == ui_main_menu.result.setting && ui_setting.active == false)
            {
                ui_setting.current_result = ui_setting.result.none;
                ui_setting.active = true;
            }
            else if (ui_setting.current_result == ui_setting.result.leave && ui_setting.active == true)
            {
                ui_main_menu.current_result = ui_main_menu.result.none;
                ui_setting.active = false;
            }
            else if (ui_main_menu.current_result == ui_main_menu.result.new_game && game.scene_transitioning_timer <= 0)
            {
                reset_data();
                ui_cut_scene.active = true;
                ui_main_menu.active = false;
            }
            else if (ui_main_menu.current_result == ui_main_menu.result.resume_game && game.scene_transitioning_timer <= 0)
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

            void reset_data()
            {
                game.run_data_manager = new run_data_manager();
                ui_setting.run_data = game.run_data_manager;
                ui_main_menu.run_data = game.run_data_manager;
                ui_cut_scene.run_data = game.run_data_manager;
                ui_setting.current_result = ui_setting.result.none;
            }
        }
        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);
        }
    }
}