using System.Diagnostics;

namespace old_heart
{
    public abstract class upgrade_base
    {
        public string upgrade_texture = "";
        public float cooldown_time = 0f; // for ui visual
        public int price = 10;

        public string description = "none";
        public upgrade_base()
        {
        }

        public virtual void apply_upgrade(player player)
        {
            cooldown_time = 0f; // for ui visual
        }
    }

    public class upgrade_Ricochet : upgrade_base
    {
        public upgrade_Ricochet()
        {
            upgrade_texture = "assets/image/ui/ui_upgrade_ricochet";
            description = "Upon hitting a target,\nthe head bounces to the \nnearest enemy, stunning\nthem as well.";
            price = 16;
        }
        public override void apply_upgrade(player player)
        {
            player.head_ricochet += 1;

            base.apply_upgrade(player);
        }
    }
    public class upgrade_Spike_head : upgrade_base
    {
        public upgrade_Spike_head()
        {
            upgrade_texture = "assets/image/ui/ui_upgrade_explosive_impact";
            description = "The head deals a set amount\nof damage to enemies\nupon impact.";
            price = 18;
        }
        public override void apply_upgrade(player player)
        {
            player.head_explosive_impact = true;

            base.apply_upgrade(player);
        }
    }
    public class upgrade_Double_dash : upgrade_base
    {
        public upgrade_Double_dash()
        {
            upgrade_texture = "assets/image/ui/ui_upgrade_double_dash";
            description = "Perform two consecutive\ndashes.";
            price = 10;
        }
        public override void apply_upgrade(player player)
        {
            player.max_dash += 1;

            base.apply_upgrade(player);
        }
    }
    public class upgrade_Lethal_dash : upgrade_base
    {
        public upgrade_Lethal_dash()
        {
            upgrade_texture = "assets/image/ui/ui_upgrade_lethal_dash";
            description = "Dash directly through\nunarmored enemies to\ninflict damage.";
            price = 12;
        }
        public override void apply_upgrade(player player)
        {
            player.lethal_dash_enable = true;

            base.apply_upgrade(player);
        }
    }
    public class upgrade_Projectile_deflection : upgrade_base
    {
        public upgrade_Projectile_deflection()
        {
            upgrade_texture = "assets/image/ui/ui_upgrade_projectile_deflection";
            description = "Deflected projectiles\nbounce back in the\ndirection of the swing,\nstunning and breaking\nthe armor of any enemy\nthey hit.";
            price = 12;
        }
        public override void apply_upgrade(player player)
        {
            player.projectile_deflection_enable = true;

            base.apply_upgrade(player);
        }
    }
    public class upgrade_Adrenaline_rush : upgrade_base
    {
        public upgrade_Adrenaline_rush()
        {
            upgrade_texture = "assets/image/ui/ui_upgrade_adrenaline_rush";
            description = "Killing an enemy grants\na 2-second burst of\nmovement speed and\ninvincibility\n(has a cooldown).";
            price = 14;
        }
        public override void apply_upgrade(player player)
        {
            player.adrenaline_rush_enable = true;

            base.apply_upgrade(player);
        }
    }
    public class upgrade_Emergency_knockback : upgrade_base
    {
        public upgrade_Emergency_knockback()
        {
            upgrade_texture = "assets/image/ui/ui_upgrade_emergency_knockback";
            description = "Taking a fatal hit triggers\na shockwave that knocks\nback and stuns nearby\nenemies, but leaves their\narmor intact.";
            price = 10;
        }
        public override void apply_upgrade(player player)
        {
            player.emergency_knockback_enable = true;

            base.apply_upgrade(player);
        }
    }
    public class upgrade_Magnet : upgrade_base
    {
        public upgrade_Magnet()
        {
            upgrade_texture = "assets/image/ui/ui_upgrade_magnet";
            description = "Automatically pulls in\nnearby dropped currency\nfrom a wider radius.";
            price = 8;
        }
        public override void apply_upgrade(player player)
        {
            player.magnet_enable = true;

            base.apply_upgrade(player);
        }
    }
}