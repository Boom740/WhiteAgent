using Autofac.Features.GeneratedFactories;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace old_heart
{
    public class ui_guide : node
    {
        ContentManager content;

        public image ui_key_image;
        public ui_text ui_key;

        public ui_text ui_guide_text;

        public bool show_guide = false;

        public float alpha = 0f;
        public float show_guide_time = 0.3f;
        public ui_guide(ContentManager content,  game_manager game_manager)
        {
            this.content = content;
            Vector2 render_size = global.render_size;

            Texture2D key_image_texture = content.Load<Texture2D>("assets/image/ui/ui_guide_key");
            ui_key_image = new image(content, render_size * new Vector2(0.4f, 0.9f), key_image_texture);
            ui_key_image.origin = key_image_texture.Bounds.Size.ToVector2() / 2;
            game_manager.add_ui(ui_key_image);

            ui_key = new ui_text("E",ui_key_image.position,text_scale: new Vector2(0.5f,0.5f) , text_color: Color.White);
            ui_key.position = ui_key_image.position - (ui_key.text_font.MeasureString(ui_key.text_string) * ui_key.text_scale / 2) ;
            game_manager.add_ui(ui_key);

            ui_guide_text = new ui_text("to interact", Vector2.Zero , text_scale: new Vector2(0.5f, 0.5f), text_color: Color.White);
            ui_guide_text.position = ui_key_image.position + new Vector2(40, -((ui_guide_text.text_font.MeasureString(ui_guide_text.text_string) * ui_guide_text.text_scale).Y /2));
            game_manager.add_ui(ui_guide_text);
        }
        public override void Update(GameTime gameTime)
        {
            if (!active) return;
            float delta_time = (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (show_guide)
            {
                alpha = Math.Clamp(alpha + (delta_time / show_guide_time), 0 , 1);
            }
            else
            {
                alpha = Math.Clamp(alpha - (delta_time / show_guide_time), 0, 1);
            }

            ui_key_image.alpha = alpha;
            ui_key.alpha = alpha;
            ui_guide_text.alpha = alpha;
        }
        public override void Draw(SpriteBatch sprite_batch)
        {
        }
    }
}
