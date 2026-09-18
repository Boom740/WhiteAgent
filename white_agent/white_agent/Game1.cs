using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended.Screens;
using System;
using System.Diagnostics;

namespace old_heart
{
    public class Game1 : Game
    {
        public GraphicsDeviceManager _graphics;
        public SpriteBatch sprite_batch;


        readonly ScreenManager screen_manager;
        public run_data_manager run_data_manager;

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

            Window.Title = "Chess Battle Advanced" ;
            Window.AllowUserResizing = true;

            Content.RootDirectory = "Content";
            IsMouseVisible = true;
            
            screen_manager = new ScreenManager();

            Components.Add(screen_manager); // auto update screen_manager
        }

        protected override void Initialize()
        {
            base.Initialize();

            global.load(Content);
            
            screen_manager.ShowScreen(new scene_main_menu(this)); // start in main menu naja
            run_data_manager = new run_data_manager(this);

            global.signal.signal_change_mouse_state += change_mouse_state;
        }

        protected override void LoadContent()
        {
            sprite_batch = new SpriteBatch(GraphicsDevice);

        }

        protected override void Update(GameTime gameTime)
        {
            global.input.update_input_state();

            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || global.input.keyboard_state.IsKeyDown(Keys.Escape))
                Exit();


            base.Update(gameTime);
        }
        protected override void UnloadContent()
        {

            base.UnloadContent();
        }
        public void change_mouse_state(global.mouse_state mouse_state)
        {
            if (mouse_state == global.mouse_state.normal)
            {
                Debug.WriteLine("mouse normal");
            }else if (mouse_state == global.mouse_state.combat)
            {
                Debug.WriteLine("mouse combat");
            }
            else if (mouse_state == global.mouse_state.combat_aim)
            {
                Debug.WriteLine("mouse combat_aim");
            }
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
