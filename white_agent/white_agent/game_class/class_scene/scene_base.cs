using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Screens;
using MonoGame.Extended.Screens.Transitions;
using System.Diagnostics;

namespace old_heart
{
    public abstract class scene_base : GameScreen
    {
        public Game1 game = null;
        public FadeTransition fade_transition;
        public SpriteBatch sprite_batch;

        public game_manager game_manager;

        public ui_game_over ui_game_over;
        public scene_base(Game1 game) : base(game)
        {
            this.game = game;
            sprite_batch = game.sprite_batch;

            global.change_mouse_state(global.mouse_state.normal);

            game_manager = new game_manager(Content, game.Window, GraphicsDevice); 

            foreach (upgrade_base upgrade in game.run_data_manager.upgrade_list)
            {
                upgrade.cooldown_time = 0;
            }
        }

        public override void LoadContent()
        {
            fade_transition = new FadeTransition(game.GraphicsDevice, Color.Black, 0.5f); // setup transition screen for all inheried scene to use

            ui_game_over = new ui_game_over(Content, game_manager , game.run_data_manager);
            game_manager.add_ui(ui_game_over);
        }

        public void update_all(GameTime gameTime)
        {
            game_manager.update(gameTime);

            if (game.scene_transitioning_timer > 0) {return; }  // game class is transitioning

            if (game_manager.current_game_state == game_manager.game_state.level_clear && game_manager.player != null)       // finished level
            {
                bool player_entered_portal = game_manager.player.current_combat_state == player.combat_state.change_scene && game_manager.player.animation_player.is_finished == true;
                if (player_entered_portal == false) // not enter next level yet
                {
                    if (game_manager.next_level_portal == null)
                    {
                        //Debug.WriteLine("scene_base   level_clear but there is no portal loaded ");

                    }else if (game_manager.next_level_portal.spawned == false)
                    {
                        global.signal.spawn_projectile(game_manager.next_level_portal);
                        game_manager.next_level_portal.spawned = true;
                    }
                    return;
                }

                bool play_latest_level = false;
                if (game.run_data_manager.cleared_level < game.run_data_manager.level_list.Count)  // check if clear_level not exceed level_list index
                {
                    play_latest_level = game.run_data_manager.level_list[game.run_data_manager.cleared_level] == game_manager.level_manager.current_level_file;
                }

                if (play_latest_level)
                {
                    game.run_data_manager.cleared_level++;
                    bool has_next_level = false;
                    has_next_level = game.run_data_manager.cleared_level < game.run_data_manager.level_list.Count;

                    if (has_next_level)
                    {
                        string next_level_name = game.run_data_manager.level_list[game.run_data_manager.cleared_level];

                        game.change_scene(new scene_gameplay(game, next_level_name), fade_transition);
                    }

                    Debug.WriteLine("clear latest level has_next_level? : " + has_next_level);
                }
            }
            else if (game_manager.current_game_state == game_manager.game_state.game_over)  // respawn
            {
                if(ui_game_over.active == false)
                {
                    ui_game_over.active = true;
                }else
                {
                    if (ui_game_over.current_result == ui_game_over.result.respawn && game_manager.level_manager.current_level_file != null)
                    {
                        game.change_scene(new scene_gameplay(game, game_manager.level_manager.current_level_file), fade_transition);
                    }else if(ui_game_over.current_result == ui_game_over.result.return_to_main_menu)
                    {
                        game.change_scene(new scene_main_menu(game), fade_transition);
                    }
                }

                /*   && game_manager.level_manager.current_level_file != null
            if (game.run_data_manager.respawn_left > 0)  
            {
                player_respawned = true;
                game.run_data_manager.respawn_left--;
                game.change_scene(new scene_gameplay(game, game_manager.level_manager.current_level_file), fade_transition);

                Debug.WriteLine("scene base respawn logic respawn_left : " + game.run_data_manager.respawn_left);
            }
                */
            }
        }
        public override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black); // black border   outside viewport

            game_manager.draw(sprite_batch);
        }
        public override void UnloadContent()
        {
            // call unscribe function of the game manager
            game_manager.unload();
        }
    }
}