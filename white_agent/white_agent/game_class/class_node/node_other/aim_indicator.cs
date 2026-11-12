using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace old_heart
{
    public class aim_indicator
    {
        public Texture2D texture;
        public bool visible = false;            // ซ่อนเป็นค่าเริ่มต้น

        public float orbit_radius = 60f;        // ระยะจากกลางตัวละครถึงกลาง reticle (px)
        public float follow_speed = 40f;        // ยิ่งสูงยิ่งตามสติ๊กติด (<= 0 = snap ทันที)
        public float sprite_angle_offset = 0f;  // sprite ชี้ขวาที่ 0 rad; ถ้าชี้ขึ้นให้ใส่ MathHelper.PiOver2
        public float layer_depth = 1f;          // 1 = บนสุด (เหมือน dash cooldown bar ที่ใช้ 1 อยู่แล้ว)
        public Color color = Color.White;

        float current_angle;
        Vector2 orbit_center;

        public aim_indicator(ContentManager content)
        {
            // placeholder: ใช้ crosshair ที่มีอยู่ก่อน แล้วเปลี่ยนเป็น sprite ลูกศร/chevron ของคุณ
            texture = content.Load<Texture2D>("assets/image/ui/ui_crosshair");
        }

        public void update(float delta_time, bool should_show, Vector2 center, Vector2 aim_direction)
        {
            if (!should_show || aim_direction == Vector2.Zero)
            {
                visible = false;   // ซ่อนทันที
                return;
            }

            // มุมของสติ๊ก: Y ของ aim_direction เป็น screen space แล้ว (Y ลง = บวก) → Atan2 ตรงกับ rotation ของ SpriteBatch
            float target_angle = MathF.Atan2(aim_direction.Y, aim_direction.X);

            if (!visible || follow_speed <= 0f)
            {
                current_angle = target_angle;   // เฟรมแรกที่โผล่ snap เลย ไม่หมุนมาจากมุมเก่า
            }
            else
            {
                float diff = MathHelper.WrapAngle(target_angle - current_angle);   // ทางสั้นสุด (-π..π)
                float t = 1f - MathF.Exp(-follow_speed * delta_time);              // ไม่ขึ้นกับ fps
                current_angle = MathHelper.WrapAngle(current_angle + diff * t);
            }

            orbit_center = center;
            visible = true;
        }

        public void draw(SpriteBatch sprite_batch)
        {
            if (!visible) { return; }

            // ตำแหน่งบนวงโคจร: x = r·cosθ, y = r·sinθ
            Vector2 offset = new Vector2(MathF.Cos(current_angle), MathF.Sin(current_angle)) * orbit_radius;

            sprite_batch.Draw(texture, orbit_center + offset, null, color,
                current_angle + sprite_angle_offset,
                texture.Bounds.Size.ToVector2() / 2f, Vector2.One, SpriteEffects.None, layer_depth);
        }
    }
}