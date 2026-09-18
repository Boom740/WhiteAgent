using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using System;
using static old_heart.enemy_leukemia;
using static old_heart.player;

namespace old_heart
{
    public abstract class enemy : entity
    {
        public enum enemy_state { normal , frightened , dizzy, died , chase , attack}
        public enemy_state state = enemy_state.normal;

        // --- target (player) ---
        public entity target;

        // --- shield ---
        public bool shield = true;
        public float shield_timer = 3f; // ตั้งเท่ากับ dizzy_timer ไว้ก่อน ปรับแยกได้ทีหลัง
        public float shield_timer_current = 0f;

        // --- dizzy ---
        public float dizzy_timer = 3f;
        public float dizzy_timer_current = 0f;

        public Random random = new Random();

        public enemy(ContentManager content_set, Vector2 position , int max_hp, float speed) : base(content_set, position, max_hp:max_hp, speed:speed)
        {
        }
        // ---------------- Damage / Death ----------------
        public override bool take_damage(int damage_taken)
        {
            if (alive == false) { return false; }
            if (shield) { return false; } // มี shield อยู่ โจมตีธรรมดาไม่เข้า

            base.take_damage(damage_taken);
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
            if (state != enemy_state.dizzy && state != enemy_state.died)
            {

                state = enemy_state.dizzy;
                shield = false;
                dizzy_timer_current = dizzy_timer;
                shield_timer_current = shield_timer;
                velocity = Vector2.Zero;
                acceleration = Vector2.Zero;

                animation_player.flash();
            }
        }
        
        public override void die()
        {
            base.die();
            global.signal.screen_shake(1.0f);
        }
    }
}