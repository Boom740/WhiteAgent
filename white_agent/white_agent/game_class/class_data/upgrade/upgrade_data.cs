using System.Diagnostics;

namespace old_heart
{
    public abstract class upgrade_base
    {
        public enum upgrade_type { dash }
        public upgrade_type type;
        public upgrade_base()
        {
        }

        public virtual void apply_upgrade(player player)
        {
            Debug.WriteLine("no apply_upgrade code in this upgrade class   idk");
        }
    }

    public class upgrade_double_dash : upgrade_base
    {
        public upgrade_double_dash()
        {
            this.type = upgrade_type.dash;
        }
        public override void apply_upgrade(player player)
        {
            player.max_dash = 2;
        }
    }
    public class upgrade_lethal_dash : upgrade_base
    {
        public upgrade_lethal_dash()
        {
            this.type = upgrade_type.dash;
        }
        public override void apply_upgrade(player player)
        {
            player.lethal_dash_enable = true;
        }
    }
}