using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace old_heart
{
    public class ui_debug_upgrade : node
    {
        run_data_manager run_data;

        Vector2 upgrade_show_position = new Vector2(10, 200);
        Point upgrade_button_size = new Point(200, 30);
        Point spacing = new Point(250, 5);
        Vector2 show_button_position = new Vector2(10, 500);
        Vector2 text_position_offset = new Vector2(5, 5);

        ui_text upgrade_text;

        ui_button show_button_button;
        ui_text show_button_text;

        ui_button reset_upgrade_button;
        ui_text reset_upgrade_text;

        bool showing_upgrade = false;

        List<upgrade_base> all_upgrade_list = new List<upgrade_base> {
            new upgrade_ricochet_head(),
            new upgrade_explosive_impact(),
            new upgrade_double_dash(),
            new upgrade_lethal_dash(),
            new upgrade_projectile_deflection(),
            new upgrade_adrenaline_rush(),
            new upgrade_emergency_knockback(),
            new upgrade_magenet()
        };

        List<ui_button> upgrade_button_list = new List<ui_button> { };
        List<ui_text> upgrade_button_text_list = new List<ui_text> { };
        public ui_debug_upgrade(ContentManager content ,run_data_manager run_data , game_manager game_manager)
        {
            this.run_data = run_data;

            upgrade_text = new ui_text("", upgrade_show_position + new Vector2(spacing.X,0), text_scale: new Vector2(0.25f, 0.25f));
            upgrade_text.visible = false;
            game_manager.add_ui(upgrade_text);

            show_button_button = new ui_button(content, new Rectangle(show_button_position.ToPoint(), upgrade_button_size));
            show_button_text = new ui_text("choose upgrade", show_button_position + text_position_offset, text_scale: new Vector2(0.25f, 0.25f),text_color: Color.Black);
            game_manager.add_ui(show_button_button);
            game_manager.add_ui(show_button_text);

            reset_upgrade_button = new ui_button(content, new Rectangle(show_button_position.ToPoint() + new Point( spacing.X,0), upgrade_button_size));
            reset_upgrade_button.visible = false;
            game_manager.add_ui(reset_upgrade_button);

            reset_upgrade_text = new ui_text("reset upgrade", show_button_position + new Vector2(spacing.X, 0) + text_position_offset, text_scale: new Vector2(0.25f, 0.25f), text_color: Color.Black);
            reset_upgrade_text.visible = false;
            game_manager.add_ui(reset_upgrade_text);

            for (int i=0;i<all_upgrade_list.Count;i++) {
                upgrade_base upgrade = all_upgrade_list[i];

                Vector2 new_poisition = upgrade_show_position + (new Vector2(0, i * (upgrade_button_size.Y + spacing.Y)));

                ui_button new_buttton = new ui_button(content, new Rectangle(new_poisition.ToPoint(), upgrade_button_size));
                new_buttton.visible = false;

                ui_text new_text = new ui_text(upgrade.GetType().Name, new_poisition + text_position_offset,  text_scale: new Vector2(0.15f, 0.15f), text_color: Color.Black);
                new_text.visible = false;

                upgrade_button_list.Add(new_buttton);
                upgrade_button_text_list.Add(new_text);

                game_manager.add_ui(new_buttton);
                game_manager.add_ui(new_text);
            }

        }

        public override void Update(GameTime gameTime)
        {
            if (!visible) return; // make it not work when it is invisible

            upgrade_text.text_string = "current upgrade :";
            foreach (upgrade_base upgrade in run_data.upgrade_list)
            {
                upgrade_text.text_string += "\n- " + upgrade.GetType().Name;
            }

            if (show_button_button.clicked)
            {
                if(showing_upgrade == false)
                {
                    showing_upgrade = true;
                }
                else
                {
                    showing_upgrade = false;
                }

                foreach (ui_button upgrade in upgrade_button_list)
                {
                    upgrade.visible = showing_upgrade;
                    upgrade.active = showing_upgrade;
                }
                foreach (ui_text upgrade in upgrade_button_text_list)
                {
                    upgrade.visible = showing_upgrade;
                }

                upgrade_text.visible = showing_upgrade;
                reset_upgrade_button.visible = showing_upgrade;
                reset_upgrade_button.active = showing_upgrade;
                reset_upgrade_text.visible = showing_upgrade;
            }

            for (int i = 0; i < upgrade_button_list.Count; i++)
            {
                ui_button upgrade_button = upgrade_button_list[i];
                if (upgrade_button.clicked)
                {
                    run_data.upgrade_list.Add(all_upgrade_list[i]);
                }
            }
            if (reset_upgrade_button.clicked)
            {
                run_data.upgrade_list.Clear();
            }

        }

        public override void Draw(SpriteBatch sprite_batch)
        {
        }

    }
}
