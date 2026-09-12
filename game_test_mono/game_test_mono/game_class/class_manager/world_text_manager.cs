using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace old_heart
{
    public class world_text_manager
    {
        public int limit = 300;
        public List<world_text> world_text_list = new List<world_text> { }; //  all world_text in this scene
        public world_text_manager()
        {

        }
        public void add(world_text world_text)
        {
            world_text_list.Add(world_text);
        }
        public void remove(world_text world_text)
        {
            world_text_list.Remove(world_text);
        }
        public void update(GameTime gameTime)
        {
            float delta_time = (float)gameTime.ElapsedGameTime.TotalSeconds;
            foreach (world_text world_text in world_text_list) // update world world_text
            {
                world_text.Update(gameTime);
            }
            if (global.input.keyboard_state.WasKeyPressed(Microsoft.Xna.Framework.Input.Keys.G))
            {
                world_text test_text = new world_text("E", global.input.scaled_mouse_world_position);
                test_text.text_scale = new Vector2(0.5f, 0.5f);
                test_text.text_color = Color.White;
                global.signal.spawn_world_text(test_text);
            }

        }
        public void draw(SpriteBatch sprite_batch)
        {
            foreach (world_text world_text in world_text_list)
            {
                if (world_text.visible)
                {
                    world_text.Draw(sprite_batch);
                }
            }
        }
    }
}