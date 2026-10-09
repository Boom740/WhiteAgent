using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Screens;
using MonoGame.Extended.Screens.Transitions;

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

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            _graphics.PreferredBackBufferWidth = 960;
            _graphics.PreferredBackBufferHeight = 540;
            _graphics.HardwareModeSwitch = false; // _graphics.HardwareModeSwitch = false     to enable alt tap in full screen

            //_graphics.SynchronizeWithVerticalRetrace = false;         // unlimited fps cap
            //IsFixedTimeStep = false;                                  // fps in game fix? // dont = false in real game   (only = false in fps test)

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
            
            screen_manager.ShowScreen(new scene_main_menu(this)); // start in main menu naja
            run_data_manager = new run_data_manager(this);
        }

        protected override void LoadContent()
        {
            sprite_batch = new SpriteBatch(GraphicsDevice);

        }

        protected override void Update(GameTime gameTime)
        {
            global.input.update_input_state();

            if (scene_transitioning_timer > 0f)
            {
                scene_transitioning_timer -=(float)gameTime.ElapsedGameTime.TotalSeconds;
            }
            else if (next_scene != null)
            {
                if (next_scene_transition == null)
                {
                    screen_manager.ReplaceScreen(next_scene);
                }
                else
                {
                    screen_manager.ReplaceScreen(next_scene,next_scene_transition);
                    scene_transitioning_timer = next_scene_transition.Duration;
                }

                next_scene = null;
                next_scene_transition = null;
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
