using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace old_heart
{
    // ข้อมูลของ melee attack แต่ละ hit ในคอมโบ แยกออกมาให้ปรับ/อัพเกรดทีหลังได้ง่าย
    public class melee_combo_hit_data
    {
        public int damage;
        public float lunge_speed;
        public float range;
        public float hitbox_lifetime;
        public float knockback_speed;

        public melee_combo_hit_data(int damage, float lunge_speed, float range, float hitbox_lifetime, float knockback_speed)
        {
            this.damage = damage;
            this.lunge_speed = lunge_speed;
            this.range = range;
            this.hitbox_lifetime = hitbox_lifetime;
            this.knockback_speed = knockback_speed;
        }
    }
}