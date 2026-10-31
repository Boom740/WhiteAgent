using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Diagnostics;

namespace old_heart
{
    public abstract class enemy : entity
    {
        public enum enemy_state { normal , frightened , dizzy, died , chase , attack , dash , tired , summoning , intro , roar}
        public enemy_state state = enemy_state.normal;

        // --- target (player) ---
        public entity target;

        // --- shield ---
        public animation_player_shield animation_player_shield;

        public bool shield = true;
        public float shield_timer = 2.5f; // ตั้งเท่ากับ dizzy_timer ไว้ก่อน ปรับแยกได้ทีหลัง
        public float shield_timer_current = 0f;

        // --- dizzy ---
        public float dizzy_timer = 3.5f;
        public float dizzy_timer_current = 0f;
        public bool can_be_dizzied = true;

        // --- drop ---
        public int drop_count_min = 1;   // จำนวนก้อน DNA ที่ดรอป สามารถสุ่มได้
        public int drop_count_max = 3;
        public int dna_value_each = 1;   // จำนวน DNA ที่ได้ตอนเก็บแต่ละก้อน

        public Random random = new Random();

        public bool death_by_player = false;
        public enemy(ContentManager content_set, Vector2 position, int max_hp, float speed = 600 , float hit_box_radius = 15) : base(content_set, position, max_hp: max_hp, speed: speed,hit_box_radius:hit_box_radius)
        {
            animation_player_shield = new animation_player_shield(content);
            animation_player_shield.draw_scale = new Vector2(hit_box_radius / 15f) * 1.3f;
        }

        // ---------------- Damage / Death ----------------
        public override bool take_damage(int damage_taken, node damage_dealer = null)
        {
            if (alive == false) { return false; }
            if (shield) { return false; } // มี shield อยู่ โจมตีธรรมดาไม่เข้า

            if (base.take_damage(damage_taken) == false) { return false; }

            global.signal.screen_shake(0.6f);
            global.sound.play_sound(global.sound.sound_name.enemy_gethit); 

            dizzy_timer_current = Math.Min(dizzy_timer_current + 0.3f, dizzy_timer);   // increase dizzy time when take damage
            shield_timer_current = Math.Min(shield_timer_current + 0.3f, shield_timer);    // increase shield break time when take damage

            if (alive == false)
            {
                state = enemy_state.died;

                if (damage_dealer is projectile projectile && projectile.owner is player || damage_dealer is player)
                {
                    death_by_player = true;
                }
            }
            return true;
        }
        // ---------------- Dizzy ----------------
        public virtual void enter_dizzy(bool break_shield = true , float dizzy_time_set = 0)
        {
            global.sound.play_sound(global.sound.sound_name.enemy_getheadbump);
            if (state == enemy_state.died) return;

            if (shield && break_shield)
            {
                shield = false;
                shield_timer_current = shield_timer;
                animation_player.flash();
            }

            if (can_be_dizzied == false) return;

            if (state != enemy_state.dizzy )
            {

                state = enemy_state.dizzy;
               
                if (dizzy_time_set == 0)
                {
                    dizzy_timer_current = dizzy_timer;
                }
                else
                {
                    dizzy_timer_current = Math.Min(dizzy_time_set, dizzy_timer);
                }

                    velocity = Vector2.Zero;
                acceleration = Vector2.Zero;

                global.sound.play_sound(global.sound.sound_name.enemy_dizzy);
            }
            else // stun when already dizzy
            {
                dizzy_timer_current = dizzy_timer;
                shield_timer_current = shield_timer;
            }
        }
        
        public override void die()
        {
            bool was_alive = alive;
            base.die();
            global.signal.screen_shake(1.0f);
            global.sound.play_sound(global.sound.sound_name.enemy_die);
            if (was_alive)
            {
                drop_items();
            }
        }
        protected virtual void drop_items(float drop_speed = 80f)
        {
            int count = random.Next(drop_count_min, drop_count_max + 1);
            for (int i = 0; i < count; i++)
            {
                item_dna dna = new item_dna(content, position, dna_value_each);

                float angle = (float)(random.NextDouble() * Math.PI * 2);
                float speed = drop_speed  + (float)random.NextDouble() * drop_speed;
                dna.velocity = new Vector2((float)Math.Cos(angle), (float)Math.Sin(angle)) * speed; // เด้งกระจายออกจากจุดตาย

                global.signal.spawn_projectile(dna);
            }
        }

        public override void Draw(SpriteBatch sprite_batch)
        {
            base.Draw(sprite_batch);

            if(shield == true)
            {
                animation_player_shield.draw(sprite_batch, position + new Vector2(0,0.01f) , alpha : 0.5f ); // draw on top of enemy sprite
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