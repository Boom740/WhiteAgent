using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace old_heart
{
    public class ui_shop : node
    {
        ContentManager content;

        public run_data_manager run_data;
        public enum result { none , exit };
        public result current_result = result.none;

        public image black_background;
        public ui_text upgrade_text;

        public ui_button leave_button;
        public ui_text leave_text;

        public ui_text shop_text;
        public image shop_background_image;

        public ui_text sell_text;

        public List<ui_button> upgrade_button_list = new List<ui_button> { };
        public List<image> upgrade_image_list = new List<image> { };
        public List<ui_text> upgrade_name_text_list = new List<ui_text> { };
        public List<ui_text> upgrade_description_list = new List<ui_text> { };
        public List<image> upgrade_price_image_list = new List<image> { };
        public List<ui_text> upgrade_price_text_list = new List<ui_text> { };

        public List<image> sold_out_image_list = new List<image> { };
        public List<ui_text> sold_out_text_list = new List<ui_text> { };
        public ui_shop(ContentManager content, game_manager game_manager , run_data_manager run_data)
        {
            this.content = content;
            this.run_data = run_data;
            active = false;
            Vector2 render_size = global.render_size;

            Texture2D white_pixel = content.Load<Texture2D>("assets/image/other/white_pixel");
            black_background = new image(content, Vector2.Zero, texture: white_pixel);
            black_background.scale = global.render_size;
            black_background.color = Color.Black * 0.67f;
            black_background.visible = false;
            game_manager.add_ui(black_background);

            Texture2D texture_leave_button_unselect = content.Load<Texture2D>("assets/image/ui/ui_button_leave_unselect");
            Texture2D texture_leave_button_selected = content.Load<Texture2D>("assets/image/ui/ui_button_leave_selected");
            leave_button = new ui_button(content, new Rectangle(new Point(820,475) , texture_leave_button_unselect.Bounds.Size));
            leave_button.button_texture = texture_leave_button_unselect;
            leave_button.hover_texture = texture_leave_button_selected;
            leave_button.visible = false;
            leave_button.active = false;
            game_manager.add_ui(leave_button);

            leave_text = new ui_text("Leave", Vector2.Zero, text_scale: new Vector2(0.3f, 0.3f), text_color: new Color(89,89,89));
            leave_text.position = leave_button.button_rectangle.Center.ToVector2() - leave_text.text_font.MeasureString(leave_text.text_string) * leave_text.text_scale / 2;
            leave_text.visible = false;
            game_manager.add_ui(leave_text);

            Texture2D texture_shop_background = content.Load<Texture2D>("assets/image/ui/ui_upgrade_container");
            shop_background_image = new image(content, render_size / 2, texture: texture_shop_background);
            shop_background_image.origin = texture_shop_background.Bounds.Center.ToVector2();
            shop_background_image.visible = false;
            game_manager.add_ui(shop_background_image);

            shop_text = new ui_text("UPGRADE PERK", Vector2.Zero ,text_scale: new Vector2(0.5f,0.5f),text_color:Color.White);
            shop_text.position = render_size * new Vector2(0.5f, 0.1f);
            shop_text.origin = shop_text.text_font.MeasureString(shop_text.text_string) / 2;
            shop_text.visible = false;
            game_manager.add_ui(shop_text);

            sell_text = new ui_text("Click to sell", render_size * new Vector2(0.91f, 0.26f) , text_scale: new Vector2(0.15f, 0.15f), text_color: Color.DarkGray);
            sell_text.visible = false;
            game_manager.add_ui(sell_text);

            Texture2D upgrade_button_unselect_texture = content.Load<Texture2D>("assets/image/ui/ui_upgrade_slot_unselect");
            Texture2D upgrade_button_selected_texture = content.Load<Texture2D>("assets/image/ui/ui_upgrade_slot_selected");
            Texture2D upgrade_texture = content.Load<Texture2D>("assets/image/ui/ui_upgrade_double_dash");  // place holder texture before load
            Texture2D upgrade_price_image_texture = content.Load<Texture2D>("assets/image/ui/ui_dna");
            Texture2D sold_out_image_texture = content.Load<Texture2D>("assets/image/ui/ui_button_leave_selected");

            Vector2 shop_button_spacing = render_size * new Vector2(0.2f, 0f); // position of center of button spacing
            for (int i = 0; i < 3; i++)
            {
                Point upgrade_position = (render_size * new Vector2(0.5f,0.5f) + ((i-1) * shop_button_spacing)).ToPoint(); // center of button

                ui_button upgrade_button = new ui_button(content, new Rectangle(upgrade_position - upgrade_button_unselect_texture.Bounds.Size / 2, upgrade_button_unselect_texture.Bounds.Size));
                upgrade_button.button_texture = upgrade_button_unselect_texture;
                upgrade_button.hover_texture = upgrade_button_selected_texture;
                upgrade_button.hold_texture = upgrade_button_selected_texture;
                upgrade_button.visible = false;
                upgrade_button.active = false;
                upgrade_button_list.Add(upgrade_button);
                game_manager.add_ui(upgrade_button);

                image upgrade_image = new image(content, upgrade_position.ToVector2() + render_size * new Vector2(0f,-0.18f) , texture: upgrade_texture);
                upgrade_image.scale = new Vector2(2, 2);
                upgrade_image.origin = upgrade_image.texture.Bounds.Center.ToVector2();
                upgrade_image.visible = false;
                upgrade_image_list.Add(upgrade_image);
                game_manager.add_ui(upgrade_image);

                ui_text upgrade_name = new ui_text("upgrade name", upgrade_position.ToVector2() + render_size * new Vector2(0f, -0.04f), text_scale: new Vector2(0.2f, 0.2f), text_color: Color.White);
                upgrade_name.visible = false;
                upgrade_name_text_list.Add(upgrade_name);
                game_manager.add_ui(upgrade_name);

                ui_text upgrade_description = new ui_text("upgrade description", upgrade_position.ToVector2(), text_scale: new Vector2(0.15f, 0.15f), text_color: Color.White);
                upgrade_description.visible = false;
                upgrade_description_list.Add(upgrade_description);
                game_manager.add_ui(upgrade_description);

                image upgrade_price_image = new image(content, upgrade_position.ToVector2() + render_size * new Vector2(-0.02f, 0.25f), upgrade_price_image_texture);
                upgrade_price_image.scale = new Vector2(0.5f, 0.5f);
                upgrade_price_image.origin = upgrade_price_image_texture.Bounds.Size.ToVector2()/2;
                upgrade_price_image.visible = false;
                upgrade_price_image_list.Add(upgrade_price_image);
                game_manager.add_ui(upgrade_price_image);

                ui_text upgrade_price_text = new ui_text("0", Vector2.Zero , text_scale: new Vector2(0.2f, 0.2f), text_color: Color.White);
                upgrade_price_text.position = upgrade_position.ToVector2() + render_size * new Vector2(0f, 0.25f)  ;
                upgrade_price_text.origin = upgrade_price_text.text_font.MeasureString("0") * new Vector2(0, 0.5f);
                upgrade_price_text.visible = false;
                upgrade_price_text_list.Add(upgrade_price_text);
                game_manager.add_ui(upgrade_price_text);

                image sold_out_image = new image(content,upgrade_position.ToVector2() + render_size * new Vector2(0f, -0.18f), texture: sold_out_image_texture);
                sold_out_image.origin = sold_out_image_texture.Bounds.Center.ToVector2();
                sold_out_image.rotation = -0.167f;
                sold_out_image.visible = false;
                sold_out_image_list.Add(sold_out_image);
                game_manager.add_ui(sold_out_image);

                ui_text sold_out_text = new ui_text("SOLD OUT", upgrade_position.ToVector2() + render_size * new Vector2(0f, -0.18f), text_scale: new Vector2(0.25f, 0.25f), text_color: Color.White);
                sold_out_text.origin = upgrade_price_text.text_font.MeasureString("SOLD OUT") /2;
                sold_out_text.rotation = -0.167f;
                sold_out_text.visible = false;
                sold_out_text_list.Add(sold_out_text);
                game_manager.add_ui(sold_out_text);
            }

            load_shop_upgrade_data();
        }
        public override void Update(GameTime gameTime)
        {
            if (!active) return;
            float delta_time = (float)gameTime.ElapsedGameTime.TotalSeconds;

            run_data.dna_temporary = run_data.dna; // make dna ui update base on real dna


            if (black_background.visible == false)
            {
                black_background.visible = true;
                leave_button.visible = true;
                leave_button.active = true;
                leave_text.visible = true;
                shop_background_image.visible = true;
                shop_text.visible = true;
                sell_text.visible = true;

                for (int i=0 ;i < upgrade_button_list.Count; i++)
                {
                    ui_button upgrade_button = upgrade_button_list[i];
                    if (upgrade_button.active == false) { continue; }

                    upgrade_base upgrade = run_data.shop_upgrade_list[i];

                    image upgrade_image = upgrade_image_list[i];
                    ui_text upgrade_name = upgrade_name_text_list[i];
                    ui_text upgrade_description = upgrade_description_list[i];
                    image upgrade_price_image = upgrade_price_image_list[i];
                    ui_text upgrade_price_text = upgrade_price_text_list[i];

                    upgrade_button.visible = true;
                    upgrade_image.visible = true;
                    upgrade_name.visible = true;
                    upgrade_description.visible = true;
                    upgrade_price_image.visible = true;
                    upgrade_price_text.visible = true;
                }

                return;
            }

            int upgrade_index = 0; // skip the sold button index
            for (int i = 0; i < upgrade_button_list.Count; i++)   // shop logic
            {
                ui_button upgrade_button = upgrade_button_list[i];
                if (upgrade_button.active == false) { continue; }

                upgrade_base upgrade = run_data.shop_upgrade_list[upgrade_index];
                upgrade_index++;

                image upgrade_image = upgrade_image_list[i];
                ui_text upgrade_name = upgrade_name_text_list[i];
                ui_text upgrade_description = upgrade_description_list[i];
                image upgrade_price_image = upgrade_price_image_list[i];
                ui_text upgrade_price_text = upgrade_price_text_list[i];
                image sold_out_image = sold_out_image_list[i];
                ui_text sold_out_text = sold_out_text_list[i];

                if (run_data.dna_temporary >= upgrade.price)  // enough money
                {
                    upgrade_price_text.text_color = Color.White;
                    if (upgrade_button.clicked && run_data.upgrade_list.Count < 4)
                    {
                        run_data.dna -= upgrade.price;

                        run_data.upgrade_list.Add(upgrade);
                        run_data.shop_upgrade_list.Remove(upgrade);
                        upgrade_button.active = false;
                        upgrade_button.mouse_hover = false;
                        upgrade_button.mouse_hold = false;
                        upgrade_button.alpha = 0.5f;
                        upgrade_image.alpha = 0.5f;
                        upgrade_name.alpha = 0.5f;
                        upgrade_description.alpha = 0.5f;
                        upgrade_price_image.alpha = 0.5f;
                        upgrade_price_text.alpha = 0.5f;

                        sold_out_image.visible = true;
                        sold_out_text.visible = true;

                        break;
                    }
                }
                else
                {
                    upgrade_price_text.text_color = Color.Red;
                }

                
            }

            if (leave_button.mouse_hover)
            {
                leave_text.text_color = Color.White;
            }
            else
            {
                leave_text.text_color = new Color(89, 89, 89);
            }


            if (current_result != result.none) { return; }

            if (leave_button.clicked)
            {
                current_result = result.exit;
            }
        }
        public void load_shop_upgrade_data()
        {
            int load_amount = Math.Min(upgrade_button_list.Count, run_data.shop_upgrade_list.Count);

            for(int i=0; i< load_amount; i++)
            {
                ui_button upgrade_button = upgrade_button_list[i];

                upgrade_base upgrade = run_data.shop_upgrade_list[i];

                image upgrade_image = upgrade_image_list[i];
                ui_text upgrade_name = upgrade_name_text_list[i];
                ui_text upgrade_description = upgrade_description_list[i];
                ui_text upgrade_price_text = upgrade_price_text_list[i];

                upgrade_button.active = true;
                upgrade_image.texture = content.Load<Texture2D>(upgrade.upgrade_texture);
                upgrade_name.text_string = upgrade.GetType().Name.Remove(0, 8).Replace('_', ' ');
                upgrade_name.origin = upgrade_name.text_font.MeasureString(upgrade_name.text_string) /2;
                upgrade_description.text_string = upgrade.description;
                upgrade_description.origin = upgrade_description.text_font.MeasureString(upgrade_description.text_string) * new Vector2(0.5f, 0);
                upgrade_price_text.text_string = upgrade.price.ToString();
            }
        }
        public override void Draw(SpriteBatch sprite_batch)
        {
        }
    }
}
