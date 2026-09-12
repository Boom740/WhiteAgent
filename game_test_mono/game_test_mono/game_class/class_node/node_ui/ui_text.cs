using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace old_heart
{
    public class ui_text : node
    {
        public SpriteFont text_font = global.theme.default_font;
        public Color text_color = Color.Black;
        public Vector2 position;
        public string text_string;
        public Vector2 text_scale = global.theme.default_font_scale;
        
        public ui_text(string text_string_set, Vector2 position)
        {
            text_string = text_string_set;
            position = position;
        }

        public override void Update(GameTime gameTime)
        {

        }

        public override void Draw(SpriteBatch sprite_batch)
        {
            sprite_batch.DrawString(text_font,text_string, position, text_color,0,Vector2.Zero,text_scale,SpriteEffects.None,1);
        }

    }
}
