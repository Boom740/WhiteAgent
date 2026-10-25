using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace old_heart
{
    public class ui_pause : node
    {
        public run_data_manager run_data;
        public enum result { none , return_to_main_menu , un_pause};
        public result current_result = result.none;

        public ui_button ui_pause_button;
        public ui_text ui_pause_text;

        public ui_button ui_return_button;
        public ui_text ui_return_text;
        public ui_pause(ContentManager content  , game_manager game_manager, run_data_manager run_data)
        {
            this.run_data = run_data;
            active = false;

            ui_pause_button = new ui_button(content, new Rectangle(new Point(540, 290), new Point(360, 230)));
            ui_pause_button.visible = false;
            ui_pause_button.active = false;
            game_manager.add_ui(ui_pause_button);

            ui_pause_text = new ui_text(" [Esc]\nPause", new Vector2(550, 300));
            ui_pause_text.text_scale = new Vector2(1.5f, 1.5f);
            ui_pause_text.text_color *= 0.5f; // transparent
            ui_pause_text.visible = false;
            game_manager.add_ui(ui_pause_text);

            ui_return_button = new ui_button(content, new Rectangle(new Point(10, 475), new Point(270,50)));
            ui_return_button.visible = false;
            ui_return_button.active = false;
            game_manager.add_ui(ui_return_button);

            ui_return_text = new ui_text("Return to main menu", Vector2.Zero, text_scale: new Vector2(0.3f, 0.3f), text_color: Color.White * 0.5f);
            ui_return_text.position = ui_return_button.button_rectangle.Center.ToVector2() - ui_return_text.text_font.MeasureString(ui_return_text.text_string) * ui_return_text.text_scale / 2;
            ui_return_text.visible = false;
            game_manager.add_ui(ui_return_text);
        }

        public override void Update(GameTime gameTime)
        {
            if (!active) return; // make it not work when it is inactive

            if (ui_pause_text.visible == false)
            {
                set_ui_visible(true);
                return;
            }

            if (ui_return_button.mouse_hover)
            {
                ui_return_text.text_color = Color.White;
            }
            else
            {
                ui_return_text.text_color = Color.White * 0.5f;
            }

            if (ui_pause_button.mouse_hover)
            {
                ui_pause_text.text_color = Color.White;
            }
            else
            {
                ui_pause_text.text_color = Color.White * 0.5f;
            }

            if (current_result != result.none) { return; }

            if (ui_return_button.clicked)
            {
                ui_return_button.clicked = false;
                current_result = result.return_to_main_menu;
            }
            else if (global.input.keyboard_state.WasKeyPressed(Microsoft.Xna.Framework.Input.Keys.Escape) || ui_pause_button.clicked)
            {
                ui_pause_button.clicked = false;
                current_result = result.un_pause;
                set_ui_visible(false);
            }

            void set_ui_visible(bool visible)
            {
                ui_pause_button.active = visible;
                //ui_pause_button.visible = visible;
                ui_pause_text.visible = visible;
                ui_return_button.active = visible;
                //ui_return_button.visible = visible;
                ui_return_text.visible = visible;
            }
        }

        public override void Draw(SpriteBatch sprite_batch)
        {
            if (!visible) return; // make it not work when it is invisible
        }

    }
}
