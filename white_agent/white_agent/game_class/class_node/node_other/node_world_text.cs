using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Graphics.PackedVector;
using System.Diagnostics;

namespace old_heart
{
    public class world_text : node
    {
        public SpriteFont text_font = global.theme.default_font;
        public Color text_start_color = Color.White;
        public Color text_color = Color.White;
        public string text_string;
        public Vector2 text_size;
        public Vector2 text_scale = new Vector2(0.5f, 0.5f);
        public float rotation = 0;
        public Vector2 origin = new Vector2(0, 0);

        public Vector2 start_position = new Vector2(0, 0);
        public Vector2 position;

        public float text_jump_intensity = 20;

        public float fade_time = 0.3f;
        public float jump_time = 0.5f;

        public float current_time = 0;


        public world_text(string text_string_set, Vector2 position)
        {
            text_string = text_string_set;
            this.start_position = position;
            this.position = start_position;
            
            text_size = text_font.MeasureString(text_string);
            origin = new Vector2(text_size.X/2 , text_size.Y);

        }

        public override void Update(GameTime gameTime)
        {
            float delta_time = (float)gameTime.ElapsedGameTime.TotalSeconds;

            current_time += delta_time;
            if (current_time >= fade_time + jump_time)
            {
                time_out();
            }

            if (current_time < jump_time)       // use tween to move position
            {
                float tween_progress = current_time / jump_time;
                tween_progress = 1 - tween_progress;                // inverse for snappy tween
                tween_progress = 1 - (tween_progress * tween_progress * tween_progress * tween_progress);
                position = start_position + new Vector2(0, -(tween_progress * text_jump_intensity));
            }
            else        // use tween to change transparent
            {
                float tween_progress = (current_time - jump_time) / fade_time;
                tween_progress = 1 - tween_progress;                // inverse for snappy tween
                tween_progress = 1 - (tween_progress * tween_progress * tween_progress * tween_progress);
                text_color = text_start_color * (1-tween_progress) ;
            }

        }
        public void time_out()
        {
            // play efx or something
            active = false; // active = false make this get instant delete
        }

        public override void Draw(SpriteBatch sprite_batch)
        {
            sprite_batch.DrawString(text_font, text_string, position, text_color, rotation , origin , text_scale, SpriteEffects.None, 1);
        }

    }
}
