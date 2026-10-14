using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace old_heart
{
    public class item_dna : item_base
    {
        public int value;

        public item_dna(ContentManager content_set, Vector2 position, int value = 1) : base(content_set, position )
        {
            this.value = value;

            texture = content.Load<Texture2D>("assets/image/ui/ui_dna");
            sprite_scale = new Vector2(0.5f, 0.5f);
        }


        public override void on_hit_entity(entity target_entity)
        {
            if (target_entity is not player player) { return; }
            if (pickup_delay > 0) { return; }

            player.run_data.dna_temporary += value;
            time_out();

            base.on_hit_entity(target_entity);
        }
    }
}