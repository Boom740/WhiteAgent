using System.Diagnostics;

namespace old_heart
{
    public abstract class upgrade_base
    {
        public string upgrade_texture = "";
        public float cooldown_time = 0f; // for ui visual
        public int price = 10;

        public string description = "no description CBA\nChess\nBattle\nAdvanced";
        public upgrade_base()
        {
        }

        public virtual void apply_upgrade(player player)
        {
        }
    }

    public class upgrade_ricochet_head : upgrade_base
    {
        public upgrade_ricochet_head()
        {
            upgrade_texture = "assets/image/ui/ui_upgrade_ricochet";
            price = 12;
        }
        public override void apply_upgrade(player player)
        {
            player.head_ricochet += 1;
        }
    }
    public class upgrade_explosive_impact : upgrade_base
    {
        public upgrade_explosive_impact()
        {
            upgrade_texture = "assets/image/ui/ui_upgrade_explosive_impact";
            price = 12;
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
            upgrade_texture = "assets/image/ui/ui_upgrade_double_dash";
            price = 3;
        }
        public override void apply_upgrade(player player)
        {
            player.max_dash += 1;
        }
    }
    public class upgrade_lethal_dash : upgrade_base
    {
        public upgrade_lethal_dash()
        {
            upgrade_texture = "assets/image/ui/ui_upgrade_lethal_dash";
            price = 7;
        }
        public override void apply_upgrade(player player)
        {
            player.lethal_dash_enable = true;
        }
    }
    public class upgrade_projectile_deflection : upgrade_base
    {
        public upgrade_projectile_deflection()
        {
            upgrade_texture = "assets/image/ui/ui_upgrade_projectile_deflection";
            price = 5;
        }
        public override void apply_upgrade(player player)
        {
            player.projectile_deflection_enable = true;
        }
    }
    public class upgrade_adrenaline_rush : upgrade_base
    {
        public upgrade_adrenaline_rush()
        {
            upgrade_texture = "assets/image/ui/ui_upgrade_adrenaline_rush";
            price = 3;
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
            upgrade_texture = "assets/image/ui/ui_upgrade_emergency_knockback";
            price = 6;
        }
        public override void apply_upgrade(player player)
        {
            player.emergency_knockback_enable = true;
        }
    }
    public class upgrade_magenet : upgrade_base
    {
        public upgrade_magenet()
        {
            upgrade_texture = "assets/image/ui/ui_upgrade_magnet";
            price = 3;
        }
        public override void apply_upgrade(player player)
        {
            player.magnet_enable = true;
        }
    }
}