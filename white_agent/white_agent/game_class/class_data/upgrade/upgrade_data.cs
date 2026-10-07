using System.Diagnostics;

namespace old_heart
{
    public abstract class upgrade_base
    {
        public upgrade_base()
        {
        }

        public virtual void apply_upgrade(player player)
        {
            Debug.WriteLine("no apply_upgrade code in this upgrade class   idk");
        }
    }

    public class upgrade_ricochet_head : upgrade_base
    {
        public upgrade_ricochet_head()
        {
        }
        public override void apply_upgrade(player player)
        {
            player.head_ricochet = 1;
        }
    }
    public class upgrade_explosive_impact : upgrade_base
    {
        public upgrade_explosive_impact()
        {
        }
        public override void apply_upgrade(player player)
        {
            player.head_explosive_impact = true;
        }
    }
    public class upgrade_double_dash : upgrade_base
    {
        public upgrade_double_dash()
        {
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
        }
        public override void apply_upgrade(player player)
        {
            player.lethal_dash_enable = true;
        }
    }
    public class upgrade_adrenaline_rush : upgrade_base
    {
        public upgrade_adrenaline_rush()
        {
        }
        public override void apply_upgrade(player player)
        {
            player.adrenaline_rush_enable = true;
        }
    }
    public class upgrade_emergency_knockback : upgrade_base
    {
        public upgrade_emergency_knockback()
        {
        }
        public override void apply_upgrade(player player)
        {
            player.emergency_knockback_enable = true;
        }
    }
}