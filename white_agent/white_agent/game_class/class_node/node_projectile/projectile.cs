using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using MonoGame.Extended.Collisions;
using System.Diagnostics;

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

        public bool deflectable = false;
        public bool deflected_by_player = false;
        public projectile(ContentManager content_set, Vector2 position , float time_left = 1 , entity owner = null)
        {
            content = content_set;
            this.time_left = time_left;
            this.position = position;
            this.collision = new collision_shape_circle(new BoundingCircle2D(position, hit_box_radius));
            collision.owner = this;
            this.owner = owner;

            shadow_texture = content.Load<Texture2D>("assets/image/other/sprite_shadow");
        }
        public virtual void spawn(projectile_manager projectile_manager, collision_manager collision_manager, debug_manager debug_manager)
        {
            projectile_manager.add(this);
            debug_manager.add(collision);

            if (owner is player)
            {
                collision_manager.add(collision, "player_hitbox");
            }
            else
            {
                collision_manager.add(collision, "enemy_hitbox");
            }
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
            if (deflected_by_player && target_entity is enemy target_enemy)
            {
                target_enemy.enter_dizzy(true);
                time_out();
            }
        }

        public virtual void time_out()
        {
            alive = false;
            active = false; // active = false make this get instant delete
        }

        public virtual void collide_wall(CollisionPair2D pair , float delta_time) // wall collision get call from collision_manager
        {
            velocity = Vector2.Zero;  // stop move and time_out when hit wall
            time_left = 0;
        }

        public virtual void on_collide_hit_box(projectile target_projectile)
        {
        }
        public projectile clone()  // use in projectile deflection 
        {
            projectile cloned_projectile = (projectile)this.MemberwiseClone();
            cloned_projectile.collision = new collision_shape_circle(new BoundingCircle2D(position, hit_box_radius));
            cloned_projectile.collision.owner = cloned_projectile;

            return cloned_projectile;
        }
        public override void Draw(SpriteBatch sprite_batch)
        {
            Draw(sprite_batch,alpha: 1f);
        }
        public virtual void Draw(SpriteBatch sprite_batch,float alpha)
        {
            float layer_depth = (position.Y + 50000f) / 100000f;
            Color color = Color.White * alpha;
            sprite_batch.Draw(texture, position, null, color, rotation, sprite_origin, sprite_scale, SpriteEffects.None, layer_depth);

            Vector2 shadow_scale = new Vector2((hit_box_radius * 2) / shadow_texture.Width, (hit_box_radius * 2) / shadow_texture.Height) * 1.5f;
            Vector2 shadow_origin = new Vector2(shadow_texture.Width, shadow_texture.Height) / 2;   // center
            sprite_batch.Draw(shadow_texture, position, null, Color.White * new Color(1, 1, 1, 0.4f) * alpha, 0, shadow_origin, shadow_scale, SpriteEffects.None, 0);
        }

        public virtual void delete_clean_up(collision_manager collision_manager, debug_manager debug_manager)
        {
            if (collision is ICollisionActor actor)
            {
                collision_manager.remove(actor); // remove it from collision manager
            }
            if (collision is node node)
            {
                debug_manager.remove(node);
            }
        }
    }
}
