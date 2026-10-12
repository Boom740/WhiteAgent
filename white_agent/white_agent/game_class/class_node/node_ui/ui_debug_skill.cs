using Autofac.Features.GeneratedFactories;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Input;
using MonoGame.Extended.Timers;
using System;
using System.Collections.Generic;

namespace old_heart
{
    public class ui_debug_skill : node
    {
        run_data_manager run_data;

        Vector2 skill_show_position = new Vector2(10, 200);
        Point skill_button_size = new Point(200, 30);
        Point spacing = new Point(250, 5);
        Vector2 show_button_position = new Vector2(10, 500);
        Vector2 text_position_offset = new Vector2(5, 5);

        ui_text skill_text;

        ui_button show_button_button;
        ui_text show_button_text;

        ui_button reset_skill_button;
        ui_text reset_skill_text;

        bool showing_skill = false;

        List<upgrade_base> all_upgrade_list = new List<upgrade_base> {
            new upgrade_ricochet_head(),
            new upgrade_explosive_impact(),
            new upgrade_double_dash(),
            new upgrade_lethal_dash(),
            new upgrade_projectile_deflection(),
            new upgrade_adrenaline_rush(),
            new upgrade_emergency_knockback()
        };

        List<ui_button> upgrade_button_list = new List<ui_button> { };
        List<ui_text> upgrade_button_text_list = new List<ui_text> { };
        public ui_debug_skill(ContentManager content ,run_data_manager run_data)
        {
            this.run_data = run_data;

            skill_text = new ui_text("", skill_show_position + new Vector2(spacing.X,0), text_scale: new Vector2(0.25f, 0.25f));
            skill_text.visible = false;

            show_button_button = new ui_button(content, new Rectangle(show_button_position.ToPoint(), skill_button_size));
            show_button_text = new ui_text("choose upgrade", show_button_position + text_position_offset, text_scale: new Vector2(0.25f, 0.25f),text_color: Color.Black);

            reset_skill_button = new ui_button(content, new Rectangle(show_button_position.ToPoint() + new Point( spacing.X,0) , skill_button_size));
            reset_skill_button.visible = false;

            reset_skill_text = new ui_text("reset skill", show_button_position + new Vector2(spacing.X, 0) + text_position_offset, text_scale: new Vector2(0.25f, 0.25f), text_color: Color.Black);
            reset_skill_text.visible = false;

            for (int i=0;i<all_upgrade_list.Count;i++) {
                upgrade_base upgrade = all_upgrade_list[i];

                Vector2 new_poisition = skill_show_position + (new Vector2(0, i * (skill_button_size.Y + spacing.Y)));

                ui_button new_buttton = new ui_button(content, new Rectangle(new_poisition.ToPoint(), skill_button_size));
                new_buttton.visible = false;

                ui_text new_text = new ui_text(upgrade.GetType().Name, new_poisition + text_position_offset,  text_scale: new Vector2(0.15f, 0.15f), text_color: Color.Black);
                new_text.visible = false;

                upgrade_button_list.Add(new_buttton);
                upgrade_button_text_list.Add(new_text);
            }
        }

        public override void Update(GameTime gameTime)
        {
            if (!visible) return; // make it not work when it is invisible

            skill_text.text_string = "current upgrade :";
            foreach (upgrade_base upgrade in run_data.upgrade_list)
            {
                skill_text.text_string += "\n- " + upgrade.GetType().Name;
            }

            if (show_button_button.clicked)
            {
                if(showing_skill == false)
                {
                    showing_skill = true;
                }
                else
                {
                    showing_skill = false;
                }

                foreach (ui_button upgrade in upgrade_button_list)
                {
                    upgrade.visible = showing_skill;
                }
                foreach (ui_text upgrade in upgrade_button_text_list)
                {
                    upgrade.visible = showing_skill;
                }

                skill_text.visible = showing_skill;
                reset_skill_button.visible = showing_skill;
                reset_skill_text.visible = showing_skill;
            }

            for (int i = 0; i < upgrade_button_list.Count; i++)
            {
                ui_button upgrade_button = upgrade_button_list[i];
                if (upgrade_button.clicked)
                {
                    run_data.upgrade_list.Add(all_upgrade_list[i]);
                }
            }
            if (reset_skill_button.clicked)
            {
                run_data.upgrade_list.Clear();
            }

            show_button_button.Update(gameTime);
            reset_skill_button.Update(gameTime);
            foreach (node node in upgrade_button_list)
            {
                node.Update(gameTime);
            }

            skill_text.Update(gameTime);
        }

        public override void Draw(SpriteBatch sprite_batch)
        {
            show_button_button.Draw(sprite_batch);
            show_button_text.Draw(sprite_batch);
            reset_skill_button.Draw(sprite_batch);
            reset_skill_text.Draw(sprite_batch);
            foreach (node node in upgrade_button_list)
            {
                node.Draw(sprite_batch);
            }
            foreach (node node in upgrade_button_text_list)
            {
                node.Draw(sprite_batch);
            }
            skill_text.Draw(sprite_batch);
        }

    }
}
