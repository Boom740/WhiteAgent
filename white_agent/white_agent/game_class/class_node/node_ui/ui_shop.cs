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
        public run_data_manager run_data;
        public enum result { none , exit };
        public result current_result = result.none;

        public image black_background;
        public ui_text upgrade_text;

        public ui_button leave_button;
        public ui_text leave_text;

        public ui_text shop_text;
        public image shop_background_image;

        public List<ui_button> upgrade_button_list = new List<ui_button> { };
        public List<image> upgrade_image_list = new List<image> { };
        public List<ui_text> upgrade_name_text_list = new List<ui_text> { };
        public List<ui_text> upgrade_description_list = new List<ui_text> { };

        public List<image> upgrade_price_image_list = new List<image> { };
        public List<ui_text> upgrade_price_text_list = new List<ui_text> { };
        public ui_shop(ContentManager content, game_manager game_manager , run_data_manager run_data)
        {
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
            shop_text.position = render_size * new Vector2(0.5f, 0.1f) - shop_text.text_font.MeasureString(shop_text.text_string) * shop_text.text_scale / 2;
            game_manager.add_ui(shop_text);


            Texture2D upgrade_button_unselect_texture = content.Load<Texture2D>("assets/image/ui/ui_upgrade_slot_unselect");
            Texture2D upgrade_button_selected_texture = content.Load<Texture2D>("assets/image/ui/ui_upgrade_slot_selected");
            Texture2D upgrade_texture = content.Load<Texture2D>("assets/image/ui/ui_upgrade_double_dash");  // place holder texture before load

            Vector2 shop_button_spacing = render_size * new Vector2(0.2f, 0f); // position of center of button spacing
            for (int i = 0; i < 3; i++)
            {
                Point upgrade_position = (render_size * new Vector2(0.5f,0.5f) + ((i-1) * shop_button_spacing)).ToPoint(); // center of button

                ui_button upgrade_button = new ui_button(content, new Rectangle(upgrade_position - upgrade_button_unselect_texture.Bounds.Size / 2, upgrade_button_unselect_texture.Bounds.Size));
                upgrade_button.button_texture = upgrade_button_unselect_texture;
                upgrade_button.hover_texture = upgrade_button_selected_texture;
                upgrade_button.hold_texture = upgrade_button_selected_texture;
                upgrade_button.visible = false;
                //upgrade_button.active = false;
                upgrade_button_list.Add(upgrade_button);
                game_manager.add_ui(upgrade_button);

                image upgrade_image = new image(content, upgrade_position.ToVector2() , texture: upgrade_texture);
                upgrade_image.scale = new Vector2(2, 2);
                upgrade_image.origin = upgrade_image.texture.Bounds.Center.ToVector2();
                upgrade_image.visible = false;
                upgrade_image_list.Add(upgrade_image);
                game_manager.add_ui(upgrade_image);
            }

            //load_shop_upgrade_data();
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

                for (int i=0;i < upgrade_button_list.Count; i++)
                {
                    ui_button upgrade_button = upgrade_button_list[i];
                    if (upgrade_button.active == false) { continue; }

                    image upgrade_image = upgrade_image_list[i];

                    upgrade_button.visible = true;
                    upgrade_image.visible = true;
                }

                return;
            }
            else
            {
                if (leave_button.mouse_hover)
                {
                    leave_text.text_color = Color.White;
                }
                else
                {
                    leave_text.text_color = new Color(89, 89, 89);
                }
            }


            if (current_result != result.none) { return; }

            if (leave_button.clicked)
            {
                current_result = result.exit;
            }
        }
        public void load_upgrade_data()
        {
            
        }
        public override void Draw(SpriteBatch sprite_batch)
        {
        }
    }
}
