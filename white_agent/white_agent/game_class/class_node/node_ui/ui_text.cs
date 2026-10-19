using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.BitmapFonts;
using System.Diagnostics;

namespace old_heart
{
    public class ui_text : node
    {
        public BitmapFont text_font = global.theme.default_font_2;
        public Color text_color = Color.White;
        public Vector2 position;
        public string text_string;
        public Vector2 text_scale = global.theme.default_font_scale_2;
        public Vector2 origin = Vector2.Zero;
        public float alpha = 1.0f;
        public float rotation = 0.0f;
        public ui_text(string text_string_set, Vector2 position , Vector2? text_scale = null , Color? text_color = null)
        {
            this.text_string = text_string_set;
            this.position = position;

            if (text_scale != null)
            {
                this.text_scale = text_scale.Value;
            }
            if(text_color != null)
            {
                this.text_color = text_color.Value;
            }
        }

        public override void Update(GameTime gameTime)
        {

        }

        public override void Draw(SpriteBatch sprite_batch)
        {
            if (!visible) return;
            sprite_batch.DrawString(text_font, text_string, position, text_color * alpha, rotation , origin, text_scale, SpriteEffects.None, 1);
        }
    }
}
