using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;

namespace old_heart
{
    // ข้อมูลของ melee attack แต่ละ hit ในคอมโบ แยกออกมาให้ปรับ/อัพเกรดทีหลังได้ง่าย
    public class melee_data
    {
        public int damage;
        public float lunge_speed;
        public float range;
        public float hitbox_lifetime;
        public float knockback_speed;
        public int hitbox_radius;
        public melee_data(int damage = 1, float lunge_speed = 100f, float range = 100f, float hitbox_lifetime = 0.3f, float knockback_speed = 100, int hitbox_radius = 15)
        {
            this.damage = damage;
            this.lunge_speed = lunge_speed;
            this.range = range;
            this.hitbox_lifetime = hitbox_lifetime;
            this.knockback_speed = knockback_speed;
            this.hitbox_radius = hitbox_radius;
        }

        public void spawn_melee_projectile(ContentManager content, entity owner, Vector2 position , Vector2 aim_direction )
        {
            float travel_distance = 1;  // minimum melee distance  (if range is too low   or   hitbox is already high)
            if (range - hitbox_radius > travel_distance)
            {
                travel_distance = range - hitbox_radius;
            }

            projectile_melee melee_projectile = new projectile_melee(content, position, aim_direction, travel_distance , hitbox_lifetime, damage,hitbox_radius);
            melee_projectile.owner = owner;
            melee_projectile.knockback_speed = knockback_speed; // set หลังสร้าง เพราะ constructor เดิมไม่รับ knockback_speed
            global.signal.spawn_projectile(melee_projectile);
        }
    }
}