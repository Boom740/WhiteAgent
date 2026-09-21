using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace old_heart
{
    public abstract class enemy : entity
    {
        public enum enemy_state { normal , frightened , dizzy, died , chase , attack}
        public enemy_state state = enemy_state.normal;

        // --- target (player) ---
        public entity target;

        // --- shield ---
        public animation_player_shield animation_player_shield;

        public bool shield = true;
        public float shield_timer = 3f; // ตั้งเท่ากับ dizzy_timer ไว้ก่อน ปรับแยกได้ทีหลัง
        public float shield_timer_current = 0f;

        // --- dizzy ---
        public float dizzy_timer = 3f;
        public float dizzy_timer_current = 0f;
        public bool can_be_dizzied = true;

        public Random random = new Random();

        public enemy(ContentManager content_set, Vector2 position, int max_hp, float speed = 600 , float hit_box_radius = 15) : base(content_set, position, max_hp: max_hp, speed: speed,hit_box_radius:hit_box_radius)
        {
            animation_player_shield = new animation_player_shield(content);
        }

        // ---------------- Damage / Death ----------------
        public virtual bool take_damage(int damage_taken, node damage_dealer = null)
        {
            if (alive == false) { return false; }
            if (shield) { return false; } // มี shield อยู่ โจมตีธรรมดาไม่เข้า

            if (base.take_damage(damage_taken) == false) { return false; }

            global.signal.screen_shake(0.4f);
            dizzy_timer_current += 0.3f;
            shield_timer_current += 0.3f;
            if (alive == false)
            {
                state = enemy_state.died;
            }
            return true;
        }
        // ---------------- Dizzy ----------------
        public virtual void enter_dizzy()
        {

            if (state == enemy_state.died) return;

            if (shield)
            {
                shield = false;
                shield_timer_current = shield_timer;
                animation_player.flash();
            }

            if (can_be_dizzied == false) return;

            if (state != enemy_state.dizzy )
            {

                state = enemy_state.dizzy;
               
                dizzy_timer_current = dizzy_timer;
                
                velocity = Vector2.Zero;
                acceleration = Vector2.Zero;

            }
        }
        
        public override void die()
        {
            base.die();
            global.signal.screen_shake(1.0f);
        }

        public override void Draw(SpriteBatch sprite_batch)
        {
            base.Draw(sprite_batch);

            if(shield == true)
            {
                animation_player_shield.draw(sprite_batch, position + new Vector2(0,0.01f) , alpha : 0.5f ,   new Vector2(hit_box_radius/15f)  * 1.3f ); // draw on top of enemy sprite
            }
        }
        
    }
    public class animation_player_shield : animation_player_base       // custom animation for this class only
    {
        public enum animation_name { normal }

        public static readonly animation_data animation_data = new animation_data();
        public animation_player_shield(ContentManager content) : base()
        {
            if (animation_data.data.Count == 0)
            {
                load(content);
            }
            base.data = animation_data;

            default_animation = animation_data.data[animation_name.normal];
            current_animation = default_animation;
        }
        public void load(ContentManager content)
        {
            Texture2D normal_texture = content.Load<Texture2D>("assets/image/other/sprite_shield");
            animation normal_animation = new animation(normal_texture, frame_per_sec: 8);
            normal_animation.name = "enemy shield normal";
            normal_animation.sprite_origin = new Vector2(normal_animation.sprite_size.X / 2f, (normal_animation.sprite_size.Y * 4f) / 5f);
            animation_data.data.Add(animation_name.normal, normal_animation);

        }
    }
}