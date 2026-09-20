using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using MonoGame.Extended.Collisions;
using System;

namespace old_heart
{
    public abstract class entity : node
    {
        public ContentManager content;

        public enum direction {down,up,left,right}
        
        public direction current_direction = direction.down;
        public Vector2 current_direction_vector = new Vector2(0, 1);   // use for set direction

        public bool movement_locked = false;
        public Vector2 knockback_velocity = Vector2.Zero;
        public float knockback_friction = 8f;

        public animation_player_base animation_player;
        public Texture2D shadow_texture;

        public collision_shape collision;
        public float hit_box_radius = 10; // for collision // hit_box is circle (BoundingCircle2D)

        public float ground_friction = 10f;
        public float max_velocity = 400;

        public Vector2 position = new Vector2(0, 0);
        public Vector2 velocity = new Vector2(0,0);
        public Vector2 acceleration = new Vector2(0,0);
        public bool alive = true;
        public int max_hp = 10;
        public int hp = 10;
        public float speed = 100;     // for acceralation
        public entity(ContentManager content_set, Vector2 position, int max_hp = 4, float speed = 100)
        {
            content = content_set;
            this.max_hp = max_hp;
            this.hp = max_hp;
            this.position = position;
            this.speed = speed;

            this.collision = new collision_shape_circle(new BoundingCircle2D(position, hit_box_radius));
            collision.owner = this;

            shadow_texture = content.Load<Texture2D>("assets/image/other/sprite_shadow");
        }
        public override void Update(GameTime gameTime)
        {
            float delta_time = (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (movement_locked == false)
            {
                velocity -= velocity * ground_friction * delta_time;
                velocity += acceleration * delta_time;

                if (velocity.Length() > max_velocity)
                {
                    velocity = Vector2.Normalize(velocity) * max_velocity;
                }

                position += velocity * delta_time;
            }

            if (knockback_velocity != Vector2.Zero)
            {
                knockback_velocity -= knockback_velocity * knockback_friction * delta_time;
                position += knockback_velocity * delta_time;
                if (knockback_velocity.Length() < 5f)
                {
                    knockback_velocity = Vector2.Zero;
                }
            }

            collision.Shape = new CollisionShape2D(new BoundingCircle2D(position, hit_box_radius));  // update collision position

            
            update_direction(current_direction_vector);
            update_animation(delta_time);

            void update_direction(Vector2 direction_vector)
            {
                float abs_x = MathF.Abs(direction_vector.X); // for calculate direction
                float abs_y = MathF.Abs(direction_vector.Y); // for calculate direction

                if (abs_x < 0.01f && abs_y < 0.01f) // 0 direction
                {
                    return;
                }

                direction new_direction_x = direction_vector.X > 0 ? direction.right : direction.left;
                direction new_direction_y = direction_vector.Y > 0 ? direction.down : direction.up;

                if (abs_x == abs_y)   // diagonal 
                {
                    if (current_direction == new_direction_x || current_direction == new_direction_y)  // same direction as curently
                    {
                        return;
                    }
                    else       // last direction is not same diagonal
                    {
                        current_direction = new_direction_x;
                    }
                    return;
                }

                if(abs_x > abs_y)
                {
                    current_direction = new_direction_x;
                }
                else
                {
                    current_direction = new_direction_y;
                }
            }
        }
        public virtual void update_animation(float delta_time)
        {
            animation_player.update(delta_time, current_direction.ToString());
        }

        public virtual void die()
        {
            alive = false;
            global.signal.spawn_particle(particle_manager.particle_name.enemy_die_efx, position,high_layer : true);
            active = false; // active = false make this get instant delete
        }
        public virtual bool take_damage(int damage_taken , node damage_dealer = null) //เติม virtual ให้ใช้กับ enemy ได้
        {
            if ( ! alive ) { return false; }
            hp -= damage_taken;

            animation_player.flash();

            world_text damage_text = new world_text(damage_taken.ToString(), position);
            global.signal.spawn_world_text(damage_text);

            if (hp  <= 0)
            {
                hp = 0;
                die();
            }
            return true;
        }
        public void collide_wall(CollisionPair2D pair , float delta_time) // wall collision get call from collision_manager
        {
            //position += pair.FirstResult.MinimumTranslationVector;    
        }
        public virtual void collide_entity(entity entity)
        {
            // for collecting item
        }
        public override void Draw(SpriteBatch sprite_batch)
        {
            animation_player.draw(sprite_batch, position);
            
            Vector2 shadow_scale = new Vector2((hit_box_radius * 2) / shadow_texture.Width ,( hit_box_radius * 2) / shadow_texture.Height) * 1.5f;
            Vector2 shadow_origin = new Vector2(shadow_texture.Width,shadow_texture.Height)    /2;   // center
            sprite_batch.Draw(shadow_texture, position, null, Color.White * new Color(1,1,1,0.4f), 0, shadow_origin, shadow_scale, SpriteEffects.None, 0);
        }
        public void apply_knockback(Vector2 direction, float speed)
        {
            knockback_velocity = direction * speed;
        }
    }
}
