using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Screens;
using MonoGame.Extended.Screens.Transitions;
using System.Diagnostics;
using System.IO;

namespace old_heart
{
    public abstract class base_screen : GameScreen
    {
        public Game1 game = null;
        public FadeTransition fade_transition;
        public SpriteBatch sprite_batch;

        public game_manager game_manager;
        public bool player_respawned = false;
        public base_screen(Game1 game) : base(game)
        {
            this.game = game;
            sprite_batch = game.sprite_batch;
        }

        public override void LoadContent()
        {
            fade_transition = new FadeTransition(game.GraphicsDevice, Color.Black, 0.5f); // setup transition screen for all inheried scene to use
            game_manager = new game_manager(Content, game.Window, GraphicsDevice);
        }

        public void update_all(GameTime gameTime)
        {
            game_manager.update(gameTime);

            if (player_respawned) {return; } 

            if (game_manager.current_game_state == game_manager.game_state.level_clear)       // un finish random level system
            {
                bool play_latest_level = false;
                if (game.run_data_manager.cleared_level < game.run_data_manager.level_list.Count)  // check if clear_level not exceed level_list index
                {
                    play_latest_level = game.run_data_manager.level_list[game.run_data_manager.cleared_level] + ".json" == Path.GetFileName(game_manager.level_manager.current_level_file);
                }
                if (play_latest_level)
                {
                    game.run_data_manager.cleared_level++;
                    bool has_next_level = false;
                    has_next_level = game.run_data_manager.cleared_level < game.run_data_manager.level_list.Count;

                    if (has_next_level)
                    {
                        string next_level_name = game.run_data_manager.level_list[game.run_data_manager.cleared_level].ToString();

                        ScreenManager.ReplaceScreen(new test_level(game, next_level_name), fade_transition);
                        game_manager.player.i_frame_time = 100f; // cant take damage when move to next level
                    }

                    Debug.WriteLine("clear latest level has_next_level? : " + has_next_level);
                }
            }
            else if (game_manager.current_game_state == game_manager.game_state.game_over && game_manager.level_manager.current_level_file != null)  // respawn
            {
                if (game.run_data_manager.respawn_left > 0)  
                {
                    player_respawned = true;
                    game.run_data_manager.respawn_left--;
                    ScreenManager.ReplaceScreen(new test_level(game, game_manager.level_manager.current_level_file), fade_transition);

                    Debug.WriteLine("scene base respawn logic respawn_left : " + game.run_data_manager.respawn_left);
                }

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