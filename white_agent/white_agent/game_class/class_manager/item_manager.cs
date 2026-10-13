using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using white_agent.game_class.class_node.node_item;

namespace old_heart
{
    public class item_manager
    {
        public List<item_drop> item_list = new List<item_drop>();
        public int limit = 200;

        public void add(item_drop item)
        {
            item_list.Add(item);
        }

        public void remove(item_drop item)
        {
            item_list.Remove(item);
        }

        public void update(GameTime gameTime, player player)
        {
            float delta_time = (float)gameTime.ElapsedGameTime.TotalSeconds;
            foreach (item_drop item in item_list)
            {
                item.Update(gameTime);
                item.update_pickup(player, delta_time);
            }
        }

        public void draw(SpriteBatch sprite_batch)
        {
            foreach (item_drop item in item_list)
            {
                if (item.visible)
                {
                    item.Draw(sprite_batch);
                }
            }
        }
    }
}