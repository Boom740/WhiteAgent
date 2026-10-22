using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace old_heart
{
    public class ui_upgrade_bar : node
    {
        ContentManager content;

        public run_data_manager run_data;
        public int last_upgrade_count;
        public ui_shop ui_shop;

        public Vector2 upgrade_slot_position;
        public Point spacing;

        public image upgrade_slot; // empty 4 slot image

        public int upgrade_bar_limit = 4;
        public List<ui_button> upgrade_button_list = new List<ui_button> { };
        public List<image> upgrade_image_list = new List<image> { };
        public List<ui_text> upgrade_cooldown_text_list = new List<ui_text> { };
        public ui_text upgrade_text; // when hover over that upgrade slot
        public float upgrade_text_alpha_change_rate = 0.05f;

        public int last_selected_upgrade = -1;
        public bool sell_confirm = false;
        public ui_upgrade_bar(ContentManager content,  game_manager game_manager , run_data_manager run_data , ui_shop ui_shop)
        {
            this.content = content;
            this.run_data = run_data;
            this.ui_shop = ui_shop;

            Texture2D upgrade_slot_texture = content.Load<Texture2D>("assets/image/ui/ui_upgrade_slot");
            upgrade_slot_position = global.render_size * new Vector2(1, 0.3f) - new Vector2(upgrade_slot_texture.Bounds.Width, 0);
            spacing = new Point(0, (int)((float)upgrade_slot_texture.Bounds.Height  / (float)4 ) + 2 );
            upgrade_slot = new image(content, upgrade_slot_position, texture: upgrade_slot_texture);
            game_manager.add_ui(upgrade_slot);

            for (int i = 0; i < 4; i++)
            {
                Vector2 upgrade_position = upgrade_slot_position + (i * spacing.ToVector2());

                Texture2D upgrade_texture = content.Load<Texture2D>("assets/image/ui/ui_upgrade_double_dash");  // place holder texture before load
                image upgrade_image = new image(content, upgrade_position, texture: upgrade_texture);
                upgrade_image.visible = false;
                upgrade_image_list.Add(upgrade_image);
                game_manager.add_ui(upgrade_image);

                ui_button upgrade_button = new ui_button(content, new Rectangle(upgrade_position.ToPoint(), upgrade_texture.Bounds.Size));
                upgrade_button.visible = false;
                upgrade_button.active = false;
                upgrade_button_list.Add(upgrade_button);
                game_manager.add_ui(upgrade_button);


                ui_text upgrade_cooldown_text = new ui_text("0", Vector2.Zero, text_scale: new Vector2(0.25f, 0.25f));
                upgrade_cooldown_text.visible = false;
                upgrade_cooldown_text_list.Add(upgrade_cooldown_text);
                game_manager.add_ui(upgrade_cooldown_text);
            }

            upgrade_text = new ui_text("", Vector2.Zero , text_scale: new Vector2(0.25f, 0.25f));
            upgrade_text.visible = false;
            upgrade_text.alpha = 0f;
            game_manager.add_ui(upgrade_text);


            load_upgrade_data();
        }
        public override void Update(GameTime gameTime)
        {
            if (!active) return;

            upgrade_text.alpha = Math.Max(0, upgrade_text.alpha - upgrade_text_alpha_change_rate);

            if (last_upgrade_count < run_data.upgrade_list.Count && last_upgrade_count < upgrade_bar_limit) // has more upgrade than loaded one    and still has slot left to load
            {
                load_upgrade_data();
            }

            if (run_data.upgrade_list.Count < upgrade_button_list.Count)
            {
                load_upgrade_data();  // loaded more than current upgrade 
            }

            for (int i = 0; i < upgrade_button_list.Count; i++)
            {
                ui_button upgrade_button = upgrade_button_list[i];
                if (upgrade_button.active == false) { continue; }

                image upgrade_image = upgrade_image_list[i];
                ui_text upgrade_cooldown_text = upgrade_cooldown_text_list[i];
                upgrade_base upgrade = run_data.upgrade_list[i];

                

                if (upgrade_button.mouse_hover)
                {
                    if (last_selected_upgrade != i)
                    {
                        last_selected_upgrade = i;
                        sell_confirm = false;
                    }

                    if (sell_confirm == false)
                    {
                        upgrade_text.text_string = upgrade.GetType().Name.Remove(0, 8).Replace('_', ' ');
                    }
                    else
                    {
                        upgrade_text.text_string = "sell for " + upgrade.price / 2 + "  DNA?";
                    }

                    upgrade_text.position = new Vector2(upgrade_button.button_rectangle.Left - 5, upgrade_button.button_rectangle.Center.Y) - (upgrade_text.text_font.MeasureString(upgrade_text.text_string) * upgrade_text.text_scale * new Vector2(1,0.5f));
                    upgrade_text.alpha = Math.Min(upgrade_text.alpha + upgrade_text_alpha_change_rate * 2, 1);
                    upgrade_text.visible = true;

                }
                else 
                {
                    if (last_selected_upgrade == i)// not hover over last selected upgrade
                    {
                        sell_confirm = false;
                    }
                }

                if (upgrade_button.clicked && ui_shop.active)
                {
                    if (sell_confirm == false)
                    {
                        sell_confirm = true;
                    }
                    else // confirmed sell
                    {
                        sell_confirm = false;
                        run_data.upgrade_list.RemoveAt(i);
                        run_data.dna += upgrade.price / 2;
                        load_upgrade_data();
                        return;
                    }
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
        public void load_upgrade_data()
        {
            last_upgrade_count = run_data.upgrade_list.Count;
            for (int i = 0; i <  upgrade_button_list.Count; i++)
            {
                ui_button upgrade_button = upgrade_button_list[i];
                image upgrade_image = upgrade_image_list[i];


                if (i > last_upgrade_count  - 1 )  // no upgrade to load
                {
                    upgrade_image.visible = false;

                    upgrade_button.active = false;
                }
                else // can load upgrade data
                {
                    upgrade_base upgrade = run_data.upgrade_list[i];
                    upgrade_image.texture = content.Load<Texture2D>(upgrade.upgrade_texture);
                    upgrade_image.visible = true;

                    upgrade_button.active = true;
                }
            }
        }
        public override void Draw(SpriteBatch sprite_batch)
        {
        }
    }
}
