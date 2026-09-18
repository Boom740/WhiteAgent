using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using MonoGame.Extended.Collisions;

namespace old_heart
{
    public abstract class projectile : node 
    {
        public entity owner;

        public ContentManager content;
        public Texture2D texture;
        public Texture2D shadow_texture;

        public collision_shape collision;

        public float hit_box_radius = 15; // for collision // hit_box is circle (BoundingCircle2D)
        public Vector2 sprite_origin;
        public Vector2 sprite_scale = new Vector2(1,1);

        public float rotation = 0;

        public Vector2 position = new Vector2(0, 0);
        public Vector2 velocity = new Vector2(0,0);
        public Vector2 acceleration = new Vector2(0,0);

        public bool alive = true;
        public float time_left = 0;

        //public float ground_friction = 5f;
        public float max_velocity = 1000;
        public projectile(ContentManager content_set, Vector2 position , float time_left = 1 , entity owner = null)
        {
            content = content_set;
            this.time_left = time_left;
            this.position = position;
            this.collision = new collision_shape_circle(new BoundingCircle2D(position, hit_box_radius));
            collision.owner = this;
            this.owner = owner;

            shadow_texture = content.Load<Texture2D>("assets/image/other/white_pixel");
        }

        public override void Update(GameTime gameTime)
        {
            if (!alive) return;
            float delta_time = (float)gameTime.ElapsedGameTime.TotalSeconds;
            
            time_left -= delta_time;
            if (time_left <= 0)
            {
                time_out();
            }
            

            //velocity -= velocity * ground_friction * delta_time;   // friction
            

            velocity += acceleration * delta_time;
            position += velocity * delta_time;

            if (velocity.Length() > max_velocity)
            {
                velocity = Vector2.Normalize(velocity) * max_velocity;
            }

            collision.Shape = new CollisionShape2D(new BoundingCircle2D(position, hit_box_radius));  // update collision position
        }

        // เรียกจาก collision_manager ตอน hitbox ของ projectile นี้ชนกับ entity เป้าหมาย
        public virtual void on_hit_entity(entity target_entity)
        {
        }

        public void time_out()
        {
            alive = false;
            // play efx or something
            active = false; // active = false make this get instant delete
        }

        public virtual void collide_wall(CollisionPair2D pair , float delta_time) // wall collision get call from collision_manager
        {
            velocity = Vector2.Zero;  // stop move and time_out when hit wall
            time_left = 0;
        }
        public override void Draw(SpriteBatch sprite_batch)
        {
            float layer_depth = (position.Y + 50000f) / 100000f;
            sprite_batch.Draw(texture, position,null,Color.White,rotation, sprite_origin, sprite_scale,SpriteEffects.None, layer_depth);

            Vector2 shadow_scale = new Vector2((hit_box_radius * 2) / shadow_texture.Width, (hit_box_radius) / shadow_texture.Height);
            sprite_batch.Draw(shadow_texture, position - (shadow_scale / 2), null, Color.White, 0, Vector2.Zero, shadow_scale, SpriteEffects.None, 0);
        }
    }
}
