using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Graphics.PackedVector;
using System;
using System.Diagnostics;

namespace old_heart
{
    public class ui_resource : node
    {
        public run_data_manager run_data;

        public ui_text ui_life_text;
        public image ui_life_image;

        public image ui_dna_image;
        public ui_text ui_dna_text;

        public ui_text ui_dna_change_text;
        public Vector2 dna_change_text_travel_vector = new Vector2(20,0);
        public Vector2 dna_change_text_start_position;
        public float dna_change_text_time = 0.5f;
        public float dna_change_text_timer = 0;
        public int dna_change_text_change_amount = 0;

        public int last_dna_amount = 0;
        public ui_resource(ContentManager content,  game_manager game_manager , run_data_manager run_data)
        {
            this.run_data = run_data;

            ui_life_text = new ui_text("x 0", new Vector2(90, 25));
            ui_life_text.text_scale = new Vector2(0.5f, 0.5f);
            game_manager.add_ui(ui_life_text);

            ui_dna_text = new ui_text("0", new Vector2(65, 83));
            ui_dna_text.text_scale = new Vector2(0.3f, 0.3f);
            game_manager.add_ui(ui_dna_text);

            ui_dna_change_text = new ui_text("+0", new Vector2(70, 83));
            ui_dna_change_text.text_scale = new Vector2(0.2f, 0.2f);
            game_manager.add_ui(ui_dna_change_text);

            ui_life_image = new image(content, new Vector2(20, 10), file_path: "assets/image/ui/ui_life");
            game_manager.add_ui(ui_life_image);

            ui_dna_image = new image(content, new Vector2(30, 80), file_path: "assets/image/ui/ui_dna");
            ui_dna_image.scale = new Vector2(0.6f, 0.6f);
            game_manager.add_ui(ui_dna_image);

            last_dna_amount = run_data.dna;
        }
        public override void Update(GameTime gameTime)
        {
            if (run_data == null) { return; }
            float delta_time = (float)gameTime.ElapsedGameTime.TotalSeconds;
            int dna = run_data.dna_temporary;

            ui_life_text.text_string = $"x {run_data.respawn_left}";
            ui_dna_text.text_string = $"{dna}";

            if (dna != last_dna_amount)
            {
                show_dna_change_text(dna - last_dna_amount);
                last_dna_amount = run_data.dna_temporary;
            }

            if (dna_change_text_timer > 0)
            {
                dna_change_text_timer -= delta_time;

                float move_ratio = dna_change_text_timer / dna_change_text_time;  // start a 1   -> 0
                float color_ratio = 1f - move_ratio;  // start a 0  -> 1

                move_ratio *= move_ratio * move_ratio * move_ratio * move_ratio;
                color_ratio *= color_ratio * color_ratio * color_ratio * color_ratio;

                move_ratio = 1f - move_ratio;   // start a 0 -> 1
                color_ratio = 1f - color_ratio; // start a 1 -> 0

                ui_dna_change_text.position = dna_change_text_start_position + (dna_change_text_travel_vector * move_ratio);
                ui_dna_change_text.alpha = color_ratio; 
            }
            else
            {
                ui_dna_change_text.visible = false;
            }


            void show_dna_change_text(int change_amount)
            {
                if (dna_change_text_timer > 0) // add number if already show change text
                {
                    dna_change_text_change_amount += change_amount;
                }
                else
                {
                    dna_change_text_change_amount = change_amount;
                }

                if (dna_change_text_change_amount >= 0)
                {
                    ui_dna_change_text.text_string = "+";
                    ui_dna_change_text.text_color = new Color(50, 255, 50); // green
                }
                else
                {
                    ui_dna_change_text.text_string = "-";
                    ui_dna_change_text.text_color = new Color(255, 50, 50); // red
                }

                ui_dna_change_text.text_string += Math.Abs(dna_change_text_change_amount);
                dna_change_text_start_position = ui_dna_text.position + (ui_dna_text.text_font.MeasureString(ui_dna_text.text_string) * ui_dna_text.text_scale * new Vector2(1, 0f)) + new Vector2(10, 0);
                ui_dna_change_text.position = dna_change_text_start_position;
                ui_dna_change_text.visible = true;
                dna_change_text_timer = dna_change_text_time;
            }
        }
        public override void Draw(SpriteBatch sprite_batch)
        {
        }
    }
}
