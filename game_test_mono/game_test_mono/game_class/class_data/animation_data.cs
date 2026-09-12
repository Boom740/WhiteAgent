using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace old_heart
{
    public class animation_data
    {
        public Dictionary<Enum, animation> data = new Dictionary<Enum, animation>();
        public animation_data()
        {
        }
    }

    public class animation
    {
        public string name; // for debug only 
        public Texture2D sprite_sheet;
        public Point sprite_size = new Point(64,64);
        public Vector2 sprite_scale = new Vector2(1,1);
        public bool loop = true;
        public int frame_count = 1;
        public float frame_time = 0.166f;
        public Vector2 sprite_origin = new Vector2(16,24);
        public animation(Texture2D sprite_sheet,bool loop = true,float frame_per_sec = 4, Point? sprite_size = null )
        {
            this.sprite_sheet = sprite_sheet;
            this.loop = loop;
            this.frame_time = 1f/frame_per_sec;
            if (sprite_size != null)
            {
                this.sprite_size = sprite_size.Value;
            }
            this.sprite_origin = new Vector2(this.sprite_size.X / 2, (this.sprite_size.Y*3) /4);
            frame_count = sprite_sheet.Height / this.sprite_size.Y;
        }
    }

    public abstract class animation_player_base
    {
        public animation_data data;
        public animation default_animation;
        public animation current_animation;

        public int current_frame_index = 0;
        public int current_direction = 0;
        public float current_time = 0;

        public readonly float flash_time = 0.1f;
        public float current_flash_time = 0;

        public bool pause = false;
        public bool is_finished = false;
        public animation_player_base()
        {
        }
        public void update_direction(string direction)
        {
            if (direction == "down") current_direction = 0;
            else if (direction == "up") current_direction = 1;
            else if (direction == "left") current_direction = 2;
            else if (direction == "right") current_direction = 3;
        }
        
        public void update(float delta_time , string direction )
        {
            if (pause) { return; }

            current_time += delta_time ;
            
            if (current_time > current_animation.frame_time)  // next frame
            {
                current_time -= current_animation.frame_time;

                if (current_frame_index >= current_animation.frame_count -1)  // end animation
                {
                    if (current_animation.loop)  // only reset frame when loop     if not loop stay at last frame
                    {
                        current_frame_index = 0;
                    }

                    is_finished = true;
                }
                else
                {
                    current_frame_index++;
                }
            }

            if (current_flash_time > 0)  // update flash time
            {
                current_flash_time -= delta_time;
            }

            update_direction(direction);
        }
        public void play(animation animation)
        {
            if (current_animation == animation) { return; } //ป้องกันการรีเซ็ตเฟรม ถ้าเป็นแอนนิเมชั่นเดิม

            if (animation.loop == false)  // start from frame 0 only for not loop animation
            {
                current_frame_index = 0;
            }
            else if (current_frame_index >= animation.frame_count)    // changed animation but index is still higher 
            {
                current_frame_index = 0;
            }

            current_animation = animation;
            current_time = 0f;
            is_finished = false;
        }
        public void flash()
        {
           current_flash_time = flash_time;
        }
        public void draw(SpriteBatch sprite_batch, Vector2 position)
        {
            Texture2D texture = current_animation.sprite_sheet;
            Rectangle source_rectangle = new Rectangle(current_animation.sprite_size.X * current_direction, current_animation.sprite_size.Y * current_frame_index, current_animation.sprite_size.X, current_animation.sprite_size.Y);
            Vector2 sprite_scale = current_animation.sprite_scale;
            Vector2 sprite_origin = current_animation.sprite_origin;
            float layer_depth = (position.Y + 50000f) / 100000f;
            Color color = Color.White;
            if (current_flash_time > 0)
            {
                color = new Color(255,0,0,255); // flash red
            }
            sprite_batch.Draw(texture, position, source_rectangle, color, 0,sprite_origin,sprite_scale,SpriteEffects.None, layer_depth);
        }

    }
}