using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using MonoGame.Extended.Collisions.Layers;
using MonoGame.Extended.Tilemaps;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection.Metadata;
using System.Text.Json;

namespace old_heart
{
    public class level_manager
    {
        public game_manager game_manager;
        public ContentManager content;

        public string current_level_file;

        public List<entity> entity_list; //  all entity in this level
        public List<collision_shape> wall_collision_list;
        public List<node> map_node_list; //  world object (wall)
        public List<node> high_map_node_list; //  world object but draw above (player enemy and all particle)
        public level_manager(game_manager game_manager)
        {
            this.game_manager = game_manager;

            entity_list = game_manager.entity_manager.entity_list;
            wall_collision_list = game_manager.collision_manager.wall_list;
            map_node_list = game_manager.map_manager.map_node_list;
            high_map_node_list = game_manager.map_manager.high_map_node_list;

            this.content = game_manager.content;

        }
        public void set_level_file(string level_file,run_data_manager run_data)
        {
            current_level_file = level_file;

            load_level(run_data);
        }
        public void clear_level()  //not true clear all  (still have collision in collision world)            use for level editor only
        {
            entity_list.Clear();
            wall_collision_list.Clear();
            map_node_list.Clear();
            high_map_node_list.Clear();
            game_manager.debug_manager.debug_node_list.Clear();

            Debug.WriteLine("-------- Cleared level -------");
        }

        public void load_level(run_data_manager run_data)
        {
            clear_level();

            if (current_level_file == null)
            {
                Debug.WriteLine("level file is null error");
            }

            string tile_map_file_path = "level/" + current_level_file;
            Tilemap tile_map = content.Load<Tilemap>(tile_map_file_path);

            foreach (TilemapLayer layer in tile_map.Layers)
            {
                Debug.WriteLine("Loading layer : " + layer.Name.ToString());
                if (layer is TilemapImageLayer image_layer)
                {
                    Texture2D texture = image_layer.Texture;
                    game_manager.add_map(new image(game_manager.content, new Vector2(image_layer.Position.X, image_layer.Position.X), texture: texture), high_ground: false);
                }
                else if (layer is TilemapObjectLayer object_layer && object_layer.Name == "wall_collision")
                {
                    foreach (TilemapObject tile_map_object in object_layer.Objects)
                    {
                        Vector2 position = tile_map_object.Position;
                        Vector2 size = tile_map_object.Bounds.Size;
                        if (tile_map_object.Rotation == 0)
                        {
                            game_manager.add_map_collision(new collision_shape_box(BoundingBox2D.CreateFromPositionAndSize(position, size)));
                        }
                        else
                        {
                            Debug.WriteLine("Error load collision rotation : " + tile_map_object.Rotation);
                        }
                    }
                }
                else if (layer is TilemapObjectLayer entity_layer && entity_layer.Name == "entity")
                {

                    foreach (TilemapObject tile_map_object in entity_layer.Objects)   // load player
                    {
                        if (tile_map_object.Class == "player")
                        {
                            Vector2 position = tile_map_object.Position + new Vector2(tile_map_object.Bounds.Width / 2, -tile_map_object.Bounds.Height * 0.2f);
                            game_manager.add_entity(new player(game_manager.content, position, run_data));

                            Debug.WriteLine("loaded player : " + tile_map_object.Class);
                        }
                    }

                    foreach (TilemapObject tile_map_object in entity_layer.Objects)  // load enemy
                    {
                        Vector2 position = tile_map_object.Position + new Vector2(tile_map_object.Bounds.Width / 2, -tile_map_object.Bounds.Height * 0.2f);
                        if (tile_map_object.Class == "player")
                        {

                        }
                        else if (tile_map_object.Class == "enemy_leukemia")
                        {
                            game_manager.add_entity(new enemy_leukemia(game_manager.content, position));
                        }
                        else if (tile_map_object.Class == "enemy_bacteria")
                        {
                            game_manager.add_entity(new enemy_bacteria(game_manager.content, position));
                        }
                        else if (tile_map_object.Class == "enemy_virus")
                        {
                            game_manager.add_entity(new enemy_virus(game_manager.content, position));
                        }
                        else if (tile_map_object.Class == "enemy_mole")
                        {
                            game_manager.add_entity(new enemy_mole(game_manager.content, position));
                        }
                        else
                        {
                            Debug.WriteLine("Error load entity : " + tile_map_object.Class);
                        }

                        if (tile_map_object.Class != "player")
                        {
                            Debug.WriteLine("loaded enemy : " + tile_map_object.Class);
                        }
                    }
                }
            }
            Debug.WriteLine("------- CBA -------- \nLoaded level : " + current_level_file);
        }
    }
}


/*

if (level_object.type == "player")
                {
                    float position_x = level_object.position_x;
                    float position_y = level_object.position_y;

                    game_manager.add_entity(new player(game_manager.content,new Vector2(position_x,position_y),run_data));
                }
                else if(level_object.type == "enemy_leukemia")
                {
                    float position_x = level_object.position_x;
                    float position_y = level_object.position_y;

                    game_manager.add_entity(new enemy_leukemia(game_manager.content, new Vector2(position_x, position_y)));
                }
                else if (level_object.type == "enemy_bacteria")
                {
                    float position_x = level_object.position_x;
                    float position_y = level_object.position_y;

                    game_manager.add_entity(new enemy_bacteria(game_manager.content, new Vector2(position_x, position_y)));
                }
                else if (level_object.type == "enemy_virus")
                {
                    float position_x = level_object.position_x;
                    float position_y = level_object.position_y;

                    game_manager.add_entity(new enemy_virus(game_manager.content, new Vector2(position_x, position_y)));
                }
                else if (level_object.type == "wall_collision_rectangle")
                {
                    float position_x = level_object.position_x;
                    float position_y = level_object.position_y;
                    float size_x = 0;
                    float size_y = 0;

                    float.TryParse(level_object.data["size_x"], out size_x);
                    float.TryParse(level_object.data["size_y"], out size_y);

                    game_manager.add_map_collision(new collision_shape_box((BoundingBox2D.CreateFromPositionAndSize(new Vector2(position_x, position_y), new Vector2(size_x, size_y)))));
                }
                else if (level_object.type == "image")
                {
                    float position_x = level_object.position_x;
                    float position_y = level_object.position_y;
                    string file_path = level_object.data["texture"];
                    bool high = false;

                    bool.TryParse(level_object.data["high"], out high);

                    game_manager.add_map(new image(game_manager.content, new Vector2(position_x, position_y), file_path), high);
                }
                else
                {
                    Debug.WriteLine("ERROR cant load type : " + level_object.type);
                }
*/

//public void save_game_data() // ฟังก์ชันใหม่สำหรับเซฟ High Score หรือข้อมูลการเล่น
//{
//    string file_directory = AppDomain.CurrentDomain.BaseDirectory;
//    string save_folder = Path.Combine(file_directory, "Saves");

//    if (!Directory.Exists(save_folder))
//    {
//        Directory.CreateDirectory(save_folder);
//    }

//    string save_file = Path.Combine(save_folder, "high_score.json");

//    // ตัวอย่างการสร้างข้อมูลและเซฟเป็น JSON (คุณสามารถเปลี่ยนไปใช้ run_data แทนได้)
//    var saveData = new { HighScore = 9999, LastPlayed = DateTime.Now };
//    string json_string = JsonSerializer.Serialize(saveData, new JsonSerializerOptions { WriteIndented = true });

//    File.WriteAllText(save_file, json_string);
//    Debug.WriteLine("Saved game data to : " + save_file);
//}

//public void load_game_data() // ฟังก์ชันสำหรับโหลดข้อมูลกลับมา
//{
//    string file_directory = AppDomain.CurrentDomain.BaseDirectory;
//    string save_file = Path.Combine(file_directory, "Saves", "high_score.json");

//    if (File.Exists(save_file))
//    {
//        string json_string = File.ReadAllText(save_file);
//        // นำ json_string ไปแปลงกลับเป็นข้อมูลเกมของคุณ
//        Debug.WriteLine("Loaded game data from : " + save_file);
//    }
//    else
//    {
//        Debug.WriteLine("No save file found.");
//    }
//}