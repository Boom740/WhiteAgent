using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using System.Collections.Generic;

namespace old_heart
{
    // เทรลสีขาวปลายแหลม: เจ้าของ (projectile/entity) แค่เรียก add_point() ทุกเฟรมแล้วเรียก draw() ตอนวาด
    public class trail_effect
    {
        private List<Vector2> points = new List<Vector2>();

        public int max_points;               // ความยาว Trail
        public float max_width;              // ความกว้างที่โคนTrail
        public float min_point_distance;     // ระยะห่างขั้นต่ำก่อนบันทึกจุดใหม่
        public Color color;

        public trail_effect(int max_points = 10, float max_width = 10f, float min_point_distance = 2f, Color? color = null)
        {
            this.max_points = max_points;
            this.max_width = max_width;
            this.min_point_distance = min_point_distance;
            this.color = color ?? Color.White;
        }

        public void add_point(Vector2 world_position)
        {
            if (points.Count == 0 || Vector2.DistanceSquared(points[0], world_position) >= min_point_distance * min_point_distance)
            {
                points.Insert(0, world_position);
                if (points.Count > max_points)
                {
                    points.RemoveAt(points.Count - 1);
                }
            }
        }

        public void clear()
        {
            points.Clear();
        }

        public void draw(SpriteBatch sprite_batch, float alpha, float layer_depth)
        {
            int count = points.Count;
            if (count < 2) return;

            for (int i = 0; i < count - 1; i++)
            {
                float t_mid = (i + 0.5f) / (count - 1); // 0 = ใกล้ตัว, 1 = ปลายหาง

                float thickness = MathHelper.Lerp(max_width, 0f, t_mid);
                float segment_alpha = MathHelper.Lerp(alpha, 0f, t_mid);

                sprite_batch.DrawLine(points[i], points[i + 1], color * segment_alpha, thickness, layer_depth);
            }
        }
    }
}