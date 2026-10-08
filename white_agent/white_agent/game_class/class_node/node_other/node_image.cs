using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace old_heart
{
    public class image : node
    {
        public Texture2D texture;
        public Vector2 position;
        public Vector2 scale = new Vector2(1,1);
        public image(ContentManager content, Vector2 position,  Texture2D texture = null , string file_path = null)
        {
            this.position = position;
            if (texture != null)
            {
                this.texture = texture;
            }
            else
            {
                this.texture = content.Load<Texture2D>(file_path);
            }
        }
        public override void Update(GameTime gameTime)
        {
        }

        public override void Draw(SpriteBatch sprite_batch)
        {
            sprite_batch.Draw(texture,position,null,Color.White,0,new Vector2(0,0),scale,SpriteEffects.None,0);
        }

    }
}
