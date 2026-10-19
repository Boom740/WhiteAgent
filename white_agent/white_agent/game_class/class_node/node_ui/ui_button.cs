using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Input;

namespace old_heart
{
    public class ui_button : node
    {
        public Texture2D button_texture;
        public Rectangle button_rectangle = new Rectangle(0,0,200,100);
        public Color button_color = Color.White;
        public Color base_color = Color.White;
        public float alpha = 1.0f;

        public Color hover_color = Color.Gray;
        public Texture2D hover_texture;
        public Color hold_color = Color.Black;
        public Texture2D hold_texture;

        public bool mouse_hover = false;
        public bool mouse_hold = false;
        public bool clicked = false;

        public ui_button(ContentManager content ,Rectangle rectangle)
        {
            button_rectangle = rectangle;

            button_texture = content.Load<Texture2D>("assets/image/other/white_pixel");
        }

        public override void Update(GameTime gameTime)
        {
            if (!active) return; // make it not work when it is not active

            MouseStateExtended mouse_state = global.input.mouse_state;
            Point mouse_position = global.input.scaled_mouse_position;
            mouse_hover = button_rectangle.Contains(mouse_position);

            clicked = false; // reset clicked every frame
            mouse_hold = false;

            if (mouse_hover) // mouse inside button rectangle
            {
                if (mouse_state.WasButtonPressed(MouseButton.Left))  // left click detected XD
                {
                    clicked = true;
                }

                if (mouse_state.IsButtonDown(MouseButton.Left))
                {
                    mouse_hold = true;
                    if (hold_texture == null)
                    {
                        button_color = hold_color;
                    }
                }
                else if(hover_texture == null)
                {
                    button_color = hover_color;
                }
                else
                {
                    button_color = base_color;
                }
            }
            else
            {
                button_color = base_color;
            }

        }

        public override void Draw(SpriteBatch sprite_batch)
        {
            if (!visible) return;
            Texture2D draw_texture = button_texture;
            if (hold_texture != null && mouse_hold)
            {
                draw_texture = hold_texture;
            }else if (hover_texture != null && mouse_hover)
            {
                draw_texture = hover_texture;
            }

            sprite_batch.Draw(draw_texture, button_rectangle, button_color*alpha);
        }

    }
}
