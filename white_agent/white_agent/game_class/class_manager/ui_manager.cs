using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.ConstrainedExecution;

namespace old_heart
{
    public class ui_manager
    {
        public List<node> ui_node_list = new List<node> { }; //  ui

        public Texture2D cursor_normal;
        public Texture2D cursor_combat;
        public Texture2D cursor_combat_aim;

        public ui_manager(ContentManager content)
        {
            cursor_normal = content.Load<Texture2D>("assets/image/ui/ui_cursor");
            cursor_combat = content.Load<Texture2D>("assets/image/ui/ui_crosshair");
        }
        public void add(node node)
        {
            ui_node_list.Add(node);
        }
        public void remove(node node)
        {
            ui_node_list.Remove(node);
        }
        public void update(GameTime gameTime)
        {
            foreach (node node in ui_node_list) // update ui
            {
                node.Update(gameTime);
            }
        }

        public void draw(SpriteBatch sprite_batch)
        {
            global.mouse_state mouse_state = global.current_mouse_state;
            Texture2D mouse_cursor = cursor_normal;

            if (mouse_state == global.mouse_state.combat)
            {
                mouse_cursor = cursor_combat;
            }
            else if (mouse_state == global.mouse_state.combat_aim)
            {
                //Debug.WriteLine("mouse combat_aim");
            }

            
            foreach (node node in ui_node_list)
            {
                if (node.visible)
                {
                    node.Draw(sprite_batch);
                }
            }

            // draw mouse cursor
            sprite_batch.Draw(mouse_cursor, global.input.scaled_mouse_position.ToVector2() , null, Color.White, 0, mouse_cursor.Bounds.Size.ToVector2() / 2, Vector2.One, SpriteEffects.None, 1);
        }
    }
}