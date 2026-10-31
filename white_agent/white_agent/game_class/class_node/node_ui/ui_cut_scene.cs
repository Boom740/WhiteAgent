using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace old_heart
{
    public class ui_cut_scene : node
    {
        public run_data_manager run_data;
        public enum result { none , finished };
        public result current_result = result.none;

        public image black_screen;

        public image black_background;
        public ui_text ui_text_1;
        public ui_text ui_text_2;

        public float cut_scene_animation_time = 1f;
        public float cut_scene_animation_timer = 0;

        public ui_cut_scene(ContentManager content, game_manager game_manager , run_data_manager run_data)
        {
            this.run_data = run_data;
            active = false;
            Vector2 render_size = global.render_size;

            Texture2D white_pixel = content.Load<Texture2D>("assets/image/other/white_pixel");
            black_background = new image(content, Vector2.Zero, texture: white_pixel);
            black_background.scale = render_size;
            black_background.color = Color.Black;
            black_background.visible = false;
            game_manager.add_ui(black_background);

            ui_text_1 = new ui_text("cutscene 1 intro", Vector2.Zero , text_scale: new Vector2(1f, 1f));
            ui_text_1.position = render_size * new Vector2(0.5f, 0.2f) - ui_text_1.text_font.MeasureString(ui_text_1.text_string) * ui_text_1.text_scale / 2;
            ui_text_1.visible = false;
            game_manager.add_ui(ui_text_1);

            ui_text_2 = new ui_text("cutscene 2 ending", Vector2.Zero, text_scale: new Vector2(1f, 1f));
            ui_text_2.position = render_size * new Vector2(0.5f, 0.2f) - ui_text_1.text_font.MeasureString(ui_text_1.text_string) * ui_text_1.text_scale / 2;
            ui_text_2.visible = false;
            game_manager.add_ui(ui_text_2);

        }
        public override void Update(GameTime gameTime)
        {
            if (!active) return;

            float delta_time = (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (black_background.visible == false)  // open setting menu
            {
                set_ui_visible(true);
                cut_scene_animation_timer = 0;
            }
            cut_scene_animation_timer += delta_time;



            if (current_result != result.none) { return; }

            if (cut_scene_animation_timer > cut_scene_animation_time)
            {
                current_result = result.finished;
                active = false;
                if (run_data.cleared_level != 0)  //  not opening cutscene 
                {
                    set_ui_visible(false);
                }
            }


            void set_ui_visible(bool visible)
            {
                black_background.visible = visible;
                if (run_data.cleared_level == 0)
                {
                    ui_text_1.visible = visible;
                }
                else
                {
                    ui_text_2.visible = visible;
                }
            }
        }
        public override void Draw(SpriteBatch sprite_batch)
        {
        }
    }
}
