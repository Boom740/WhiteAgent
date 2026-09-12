using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace old_heart
{
    public class world_text : node
    {
        public SpriteFont text_font = global.theme.default_font;
        public Color text_color = Color.Black;
        public string text_string;
        public Vector2 text_size;
        public Vector2 text_scale = global.theme.default_font_scale;
        public float rotation = 0;
        public Vector2 origin = new Vector2(0, 0);

        public Vector2 position = new Vector2(0, 0);
        public Vector2 velocity = new Vector2(0, -100);
        public Vector2 acceleration = new Vector2(0, 200);

        public float text_wobble = 10;

        public bool alive = true;
        public float time_left = 1;


        public world_text(string text_string_set, Vector2 position)
        {
            text_string = text_string_set;
            this.position = position;

            text_size = text_font.MeasureString(text_string);
            origin = new Vector2(text_size.X/2 , text_size.Y);
        }

        public override void Update(GameTime gameTime)
        {
            float delta_time = (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (alive)
            {
                time_left -= delta_time;
                if (time_left <= 0)
                {
                    time_out();
                }
            }
            else
            {
                return;
            }


            velocity += acceleration * delta_time;
            position += velocity * delta_time;
        }
        public void time_out()
        {
            alive = false;
            // play efx or something
            active = false; // active = false make this get instant delete
        }

        public override void Draw(SpriteBatch sprite_batch)
        {
            sprite_batch.DrawString(text_font, text_string, position, text_color, rotation , origin , text_scale, SpriteEffects.None, 1);
        }

    }
}
