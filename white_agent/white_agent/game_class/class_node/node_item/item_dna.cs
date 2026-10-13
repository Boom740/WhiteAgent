using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using white_agent.game_class.class_node.node_item;

namespace old_heart
{
    public class item_dna : item_drop
    {
        public int value;

        public item_dna(ContentManager content_set, Vector2 position, int value = 1) : base(content_set, position)
        {
            this.value = value;
            hit_box_radius = 3f;
           // placeholder_color = new Color(80, 220, 160);

            texture = content.Load<Texture2D>("assets/image/ui/ui_dna");
        }

        protected override void on_picked_up(player target)
        {
            target.run_data.dna.add(value);

            // TODO: เสียงเก็บ DNA (ตอนนี้ sound_name ยังไม่มี ต้องเพิ่ม enum + โหลดไฟล์ใน global.sound.load ก่อน)
        }
    }
}