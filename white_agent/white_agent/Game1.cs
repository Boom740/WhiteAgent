using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Screens;
using MonoGame.Extended.Screens.Transitions;
using System.Diagnostics;

namespace old_heart
{
    public class Game1 : Game
    {
        public GraphicsDeviceManager _graphics;
        public SpriteBatch sprite_batch;


        public run_data_manager run_data_manager;

        public ScreenManager screen_manager;
        public scene_base next_scene;
        public Transition next_scene_transition;
        public float scene_transitioning_timer = 0f; // track current transition time to prevent transition in that time
        public float halved_scene_transion_time = 1f; // tract time that actually change scene ( 1/2 of scene traisition time)
        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            _graphics.PreferredBackBufferWidth = 960;
            _graphics.PreferredBackBufferHeight = 540;
            _graphics.HardwareModeSwitch = false; // _graphics.HardwareModeSwitch = false     to enable alt tap in full screen

            _graphics.SynchronizeWithVerticalRetrace = false;         // unlimited fps cap
            IsFixedTimeStep = false;                                  // fps in game fix? // dont = false in real game   (only = false in fps test)

            _graphics.ApplyChanges();
            //_graphics.ToggleFullScreen();

            Window.Title = "White Agent";
            Window.AllowUserResizing = true;

            Content.RootDirectory = "Content";
            IsMouseVisible = false;
            
            screen_manager = new ScreenManager();

            Components.Add(screen_manager); // auto update screen_manager
        }

        protected override void Initialize()
        {
            base.Initialize();

            global.load(Content,GraphicsDevice);

            run_data_manager = new run_data_manager();

            screen_manager.ShowScreen(new scene_main_menu(this)); // start in main menu naja
        }

        protected override void LoadContent()
        {
            sprite_batch = new SpriteBatch(GraphicsDevice);

        }

        protected override void Update(GameTime gameTime)
        {
            global.input.update_input_state();

            if (global.setting.full_screen != _graphics.IsFullScreen)
            {
                _graphics.ToggleFullScreen();
            }

            if (scene_transitioning_timer > 0f)
            {
                scene_transitioning_timer -=(float)gameTime.ElapsedGameTime.TotalSeconds;
            }
            else if (next_scene != null)
            {
                if (next_scene_transition != null)
                {
                    screen_manager.ReplaceScreen(next_scene, next_scene_transition);
                    scene_transitioning_timer = next_scene_transition.Duration;
                    halved_scene_transion_time = next_scene_transition.Duration / 2f;
                }
                else
                {
                    screen_manager.ReplaceScreen(next_scene);
                }

                this.next_scene = null;
                this.next_scene_transition = null;
            }

            base.Update(gameTime);

        }
        protected override void UnloadContent()
        {

            base.UnloadContent();
        }

        public void change_scene(scene_base scene , Transition transition = null)
        {
            if (next_scene != null) { return; } // already queue next scene
            if (scene_transitioning_timer >= halved_scene_transion_time) { return; } // curently changing to next scene   dont queue new scene

            next_scene = scene;
            next_scene_transition = transition;
        }

        //protected override void Draw(GameTime gameTime)
        //{
        //    GraphicsDevice.Clear(Color.CornflowerBlue);

        //    sprite_batch.Begin(samplerState: SamplerState.PointClamp);
               
        //    sprite_batch.End();

        //    base.Draw(gameTime);
        //}
    }
}
