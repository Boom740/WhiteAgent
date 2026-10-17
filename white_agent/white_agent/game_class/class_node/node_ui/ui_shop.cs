using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Diagnostics;

namespace old_heart
{
    public class ui_shop : node
    {
        public run_data_manager run_data;
        public enum result { none , exit};
        public result current_result = result.none;

        public image black_background;
        public ui_text upgrade_text;


        public ui_button leave_button;
        public ui_text leave_text;
        public ui_shop(ContentManager content, game_manager game_manager , run_data_manager run_data)
        {
            this.run_data = run_data;
            active = false;

            Texture2D white_pixel = content.Load<Texture2D>("assets/image/other/white_pixel");
            black_background = new image(content, Vector2.Zero, texture: white_pixel);
            black_background.scale = global.render_size;
            black_background.color = Color.Black * 0.67f;
            black_background.visible = false;
            game_manager.add_ui(black_background);

            Texture2D texture_leave_button_unselect = content.Load<Texture2D>("assets/image/ui/ui_button_leave_unselect");
            Texture2D texture_leave_button_selected = content.Load<Texture2D>("assets/image/ui/ui_button_leave_selected");
            leave_button = new ui_button(content, new Rectangle(new Point(800,475) , texture_leave_button_unselect.Bounds.Size));
            leave_button.button_texture = texture_leave_button_unselect;
            leave_button.hover_texture = texture_leave_button_selected;
            leave_button.visible = false;
            leave_button.active = false;
            game_manager.add_ui(leave_button);
        }
        public override void Update(GameTime gameTime)
        {
            if (!active) return;
            run_data.dna_temporary = run_data.dna; // make dna ui update base on real dna
            //float delta_time = (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (black_background.visible == false)
            {
                black_background.visible = true;
                leave_button.visible = true;
                leave_button.active = true;
                return;
            }


            if (current_result != result.none) { return; }

            if (leave_button.clicked)
            {
                current_result = result.exit;
            }
        }
        public override void Draw(SpriteBatch sprite_batch)
        {
        }
    }
}
