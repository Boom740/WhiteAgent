using Autofac.Features.GeneratedFactories;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Content;
using MonoGame.Extended.Input;
using MonoGame.Extended.Timers;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace old_heart
{
    public class ui_upgrade_bar : node
    {
        run_data_manager run_data;

        Vector2 upgrade_slot_position;
        Point spacing;

        image upgrade_slot; // empty 4 slot image

        List<ui_button> upgrade_button_list = new List<ui_button> { };
        List<image> upgrade_image_list = new List<image> { };
        List<ui_text> upgrade_cooldown_text_list = new List<ui_text> { };
        ui_text upgrade_text; // when hover over that upgrade slot
        float upgrade_text_alpha_change_rate = 0.05f;
        public ui_upgrade_bar(ContentManager content,  game_manager game_manager , run_data_manager run_data)
        {
            this.run_data = run_data;

            Texture2D upgrade_slot_texture = content.Load<Texture2D>("assets/image/ui/ui_upgrade_slot");
            upgrade_slot_position = global.render_size * new Vector2(1, 0.3f) - new Vector2(upgrade_slot_texture.Bounds.Width, 0);
            spacing = new Point(0, (int)((float)upgrade_slot_texture.Bounds.Height  / (float)4 ) + 2 );
            upgrade_slot = new image(content, upgrade_slot_position, texture: upgrade_slot_texture);
            game_manager.add_ui(upgrade_slot);

            for (int i = 0; i < run_data.upgrade_list.Count; i++)
            {
                upgrade_base upgrade = run_data.upgrade_list[i];
                Vector2 upgrade_position = upgrade_slot_position + (i * spacing.ToVector2());
                Texture2D upgrade_texture = content.Load<Texture2D>(upgrade.upgrade_texture);
                image upgrade_image = new image(content, upgrade_position, texture: upgrade_texture);
                upgrade_image_list.Add(upgrade_image);
                game_manager.add_ui(upgrade_image);

                ui_button upgrade_button = new ui_button(content, new Rectangle(upgrade_position.ToPoint() , upgrade_texture.Bounds.Size));
                upgrade_button_list.Add(upgrade_button);
                game_manager.add_ui(upgrade_button);
                upgrade_button.visible = false;

                ui_text upgrade_cooldown_text = new ui_text("0", Vector2.Zero , text_scale: new Vector2(0.25f, 0.25f));
                upgrade_cooldown_text.visible = false;
                upgrade_cooldown_text_list.Add(upgrade_cooldown_text);
                game_manager.add_ui(upgrade_cooldown_text);

            }

            upgrade_text = new ui_text("", Vector2.Zero , text_scale: new Vector2(0.25f, 0.25f));
            upgrade_text.visible = false;
            upgrade_text.alpha = 0f;
            game_manager.add_ui(upgrade_text);
        }
        public override void Update(GameTime gameTime)
        {
            if (!active) return;

            upgrade_text.alpha = Math.Max(0, upgrade_text.alpha - upgrade_text_alpha_change_rate);

            if (upgrade_button_list.Count > run_data.upgrade_list.Count) return;
            for (int i = 0; i < upgrade_button_list.Count; i++)
            {
                ui_button upgrade_button = upgrade_button_list[i];
                image upgrade_image = upgrade_image_list[i];
                ui_text upgrade_cooldown_text = upgrade_cooldown_text_list[i];
                upgrade_base upgrade = run_data.upgrade_list[i];

                if (upgrade_button.mouse_hover)
                {
                    upgrade_text.text_string = upgrade.GetType().Name.Remove(0,8).Replace('_',' ');
                    upgrade_text.position = new Vector2(upgrade_button.button_rectangle.Left - 5, upgrade_button.button_rectangle.Center.Y) - (upgrade_text.text_font.MeasureString(upgrade_text.text_string) * upgrade_text.text_scale * new Vector2(1,0.5f));
                    upgrade_text.alpha = Math.Min(upgrade_text.alpha + upgrade_text_alpha_change_rate * 2, 1);
                    upgrade_text.visible = true;
                }

                if (upgrade.cooldown_time > 0)
                {
                    upgrade_image.color = new Color(new Vector3( 0.2f));
                    upgrade_cooldown_text.text_string = upgrade.cooldown_time.ToString("F1");
                    upgrade_cooldown_text.position = upgrade_button.button_rectangle.Center.ToVector2() - (upgrade_cooldown_text.text_font.MeasureString(upgrade_cooldown_text.text_string) * upgrade_text.text_scale / 2);
                    upgrade_cooldown_text.visible = true;
                }
                else
                {
                    upgrade_image.color = Color.White;
                    upgrade_cooldown_text.visible = false;
                }
            }
        }

        public override void Draw(SpriteBatch sprite_batch)
        {
        }
    }
}
