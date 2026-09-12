using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace old_heart
{
    public class world_text_manager
    {
        public int limit = 300;
        public List<projectile> world_text_list = new List<projectile> { }; //  all projectile in this scene
        public world_text_manager()
        {

        }
        public void add(projectile projectile)
        {
            world_text_list.Add(projectile);
        }
        public void remove(projectile projectile)
        {
            world_text_list.Remove(projectile);
        }
        public void update(GameTime gameTime)
        {
            float delta_time = (float)gameTime.ElapsedGameTime.TotalSeconds;
            foreach (projectile projectile in world_text_list) // update world projectile
            {
                projectile.Update(gameTime);
            }
        }
        public void draw(SpriteBatch sprite_batch)
        {
            foreach (projectile projectile in world_text_list)
            {
                if (projectile.visible)
                {
                    projectile.Draw(sprite_batch);
                }
            }
        }
    }
}