using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;

namespace old_heart
{
    public class scene_test_anything : scene_base
    {
        public string current_map_file;

        public ui_text test_text;
        public ui_text test_text2;

        public ui_debug_upgrade ui_debug_upgrade;
        public enemy_leukemia enemy_1;
        public enemy_bacteria enemy_2;
        public enemy_boss enemy_3;
        public int frame_count = 0;
        public float fps_timer = 0;
        public int fps_last_record = 0;
        public float spawn_projectile_timer = 0;
        public int projectile_spawn_count = 0;
        public scene_test_anything(Game1 game) : base(game)
        {
            global.change_mouse_state(global.mouse_state.combat);
        }
        public override void LoadContent()
        {
            test_text = new ui_text("this text get replace in update function anyway",  new Vector2(10, 5));
            test_text.text_color = Color.DarkRed;
            test_text.text_scale = new Vector2(0.25f, 0.25f);
            game_manager.add_ui(test_text);
            test_text2 = new ui_text("press P to view hitbox testttttttt  Chess Battle Advanced\nE shake\nF player take 1 damage\nT spawn test projectile\nZ spawn test particle",  new Vector2(25f, 0f));
            test_text2.text_color = Color.DarkOrange;
            //game_manager.add_map(test_text2);


            ui_upgrade_bar = new ui_upgrade_bar(Content, game_manager, game.run_data_manager , ui_shop);
            game_manager.add_ui(ui_upgrade_bar);

            player player = new player(Content,new Vector2(200,200),game.run_data_manager);
            game_manager.add_entity(player);

            collision_shape_box wall = new collision_shape_box(BoundingBox2D.CreateFromPositionAndSize(new Vector2(500f, 100f), new Vector2(64f, 600f)));
            game_manager.add_map_collision(wall);
            collision_shape_box wall2 = new collision_shape_box(BoundingBox2D.CreateFromPositionAndSize(new Vector2(100f, 500f), new Vector2(500f, 64f)));
            game_manager.add_map_collision(wall2);
            collision_shape_box wall3 = new collision_shape_box(BoundingBox2D.CreateFromPositionAndSize(new Vector2(500f, 100f), new Vector2(500f, 64f)));
            game_manager.add_map_collision(wall3);
            //collision_shape_circle wall4 = new collision_shape_circle(new BoundingCircle2D(new Vector2(0, 0), 100));
            //game_manager.add_map_collision(wall4);
            collision_shape_box wall5 = new collision_shape_box(BoundingBox2D.CreateFromPositionAndSize(new Vector2(1000f, 100f), new Vector2(64f, 600f)));
            game_manager.add_map_collision(wall5);
            collision_shape_box wall6 = new collision_shape_box(BoundingBox2D.CreateFromPositionAndSize(new Vector2(500f, 500f), new Vector2(600f, 64f)));
            game_manager.add_map_collision(wall6);

            
            enemy_1 = new enemy_leukemia(Content, position: new Vector2(600, 400)); // ใช้ Content (ตัวใหญ่) ไม่ใช่ content
            game_manager.add_entity(enemy_1);

            enemy test_enemy;

            test_enemy = new enemy_virus(Content, position: new Vector2(400, 1300));
            game_manager.add_entity(test_enemy);

            test_enemy = new enemy_mold(Content, position: new Vector2(400, 1300)); 
            game_manager.add_entity(test_enemy);

            enemy_2 = new enemy_bacteria(Content, position: new Vector2(-400, 300)); 
            game_manager.add_entity(enemy_2);

            game_manager.add_entity(enemy_bacteria.super_form(Content, new Vector2(200, 840)));

            game_manager.add_entity(enemy_virus.super_form(Content, new Vector2(-200, 740)));

            game_manager.add_entity(enemy_mold.super_form(Content, new Vector2(0, 1140)));
            game_manager.add_entity(enemy_leukemia.super_form(Content, new Vector2(0, 1140)));

            projectile merchant = new projectile_merchant(Content, position: new Vector2(400, 400));
            game_manager.add_projectile(merchant);

            enemy_3 = new enemy_boss(Content, position: new Vector2(400, -300));
            game_manager.add_entity(enemy_3);
            

            base.LoadContent();


            ui_debug_upgrade = new ui_debug_upgrade(Content, game.run_data_manager, game_manager);
            game_manager.add_ui(ui_debug_upgrade);
        }
        public override void Update(GameTime gameTime)
        {
            if (global.input.keyboard_state.WasKeyPressed(Keys.V))
            {
                game.change_scene(new scene_main_menu(game), fade_transition);
            }
            if (global.input.keyboard_state.WasKeyPressed(Keys.G))
            {
                game.run_data_manager.dna = 1000000;
            }

            if (global.input.keyboard_state.WasKeyPressed(Keys.B))
            {
                Vector2 mouse_position = global.input.scaled_mouse_world_position;
                for (int i = 0; i < 100; i++)
                {
                    //global.signal.spawn_entity(new enemy_bacteria(Content, mouse_position));
                }
            }
            else if (global.input.keyboard_state.WasKeyPressed(Keys.N))
            {
                //projectile_spawn_count += 1000;
            }


                test_text.text_string = $"scene_test_anything current_frame_fps[{(1 / gameTime.ElapsedGameTime.TotalSeconds):F2}]                                                     V to go back to title bruh" +
                //    fps[{ (1 / gameTime.ElapsedGameTime.TotalSeconds):F2}]
                //$"\nmouse_pos : {global.input.scaled_mouse_position}\nworld_mouse_pos : {global.input.scaled_mouse_world_position}" +
                //$"\ngame state : {game_manager.current_game_state}" +
                $"";

            //this.game_manager.player.i_frame_time = 10;

            float delta_time = (float)gameTime.ElapsedGameTime.TotalMilliseconds / 1000;
            /*
            if (spawn_projectile_timer <= 0)
            {
                for (int i = 0; i < projectile_spawn_count; i++)
                {
                    global.signal.spawn_projectile(new projectile_test(Content, 1, new Vector2 (i/10f,0) , new Vector2(0,-100)));
                }
                spawn_projectile_timer = 1;
            }
            else
            {
                spawn_projectile_timer -= delta_time;
            }
            */
            if (fps_timer > 10)
            {
                fps_timer = 0;
                fps_last_record = frame_count/10;
                frame_count = 0;
            }
            else
            {
                fps_timer += delta_time;
                frame_count ++;
            }

            if (game_manager.player != null)
            {
                test_text.text_string +=
                //$"\nplayer acc : {game_manager.player.acceleration}\nvelocity : {game_manager.player.velocity.X:F2} , {game_manager.player.velocity.Y:F2}" +
                //$"\nw speed : {game_manager.player.velocity.Length():F2} \nposition : {game_manager.player.position.X:F2} , {game_manager.player.position.Y:F2}" +
                //$"\nplayer animation : {game_manager.player.animation_player.current_animation.name} [{game_manager.player.animation_player.current_frame_index}]" +
                //$"\nplayer animation_2 : {game_manager.player.animation_player_2.current_animation.name} [{game_manager.player.animation_player_2.current_frame_index}]" +
                //$"\nstate : {game_manager.player.current_state} combat_state : {game_manager.player.current_combat_state} has_head : {game_manager.player.has_head}" +
                //$"\natk [{game_manager.player.combo_count}] melee_cooldown_timer : {game_manager.player.melee_cooldown_timer:F2}  combo_reset_timer : {game_manager.player.combo_reset_timer:F2} attack_timer : {game_manager.player.attack_timer:F2}" +
                //$"\ncurrent_buffer_input : {game_manager.player.current_buffer_input}]" +
                //  $"\ndash [{game_manager.player.dash_count}] cooldown timer : {game_manager.player.dash_cooldown_timer:F2}" +
                $"\nentity_count : {game_manager.entity_list.Count}" +
                $"\nprojectile_count : {game_manager.projectile_list.Count}" +
                $"\navarage FPS (10 second): {fps_last_record}" +

                $"";
            }
            /*
            if (enemy_3 != null)
            {
                test_text.text_string +=
                $"\nenemy : {enemy_3.state}" +
                $"";
            }
            */
            update_all(gameTime);
        }
    }
}