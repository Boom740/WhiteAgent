using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using MonoGame.Extended.Tilemaps;
using System.Collections.Generic;
using System.Diagnostics;

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
            this.content = game_manager.content;
            this.game_manager = game_manager;

            entity_list = game_manager.entity_manager.entity_list;
            wall_collision_list = game_manager.collision_manager.wall_list;
            map_node_list = game_manager.map_manager.map_node_list;
            high_map_node_list = game_manager.map_manager.high_map_node_list;


        }
        public void set_level_file(string level_file,run_data_manager run_data)
        {
            current_level_file = level_file;

            load_level(run_data);
        }

        public void load_level(run_data_manager run_data)
        {
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
                    game_manager.add_map(new image(content, new Vector2(image_layer.Position.X, image_layer.Position.X), texture: texture), high_ground: false);
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
                            game_manager.add_entity(new player(content, position, run_data));

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
                            game_manager.add_entity(new enemy_leukemia(content, position));
                        }
                        else if (tile_map_object.Class == "enemy_bacteria")
                        {
                            game_manager.add_entity(new enemy_bacteria(content, position));
                        }
                        else if (tile_map_object.Class == "enemy_virus")
                        {
                            game_manager.add_entity(new enemy_virus(content, position));
                        }
                        else if (tile_map_object.Class == "enemy_mold")
                        {
                            game_manager.add_entity(new enemy_mold(content, position));
                        }
                        else if (tile_map_object.Class == "enemy_leukemia_super")
                        {
                            game_manager.add_entity(enemy_leukemia.super_form(content,position));
                        }
                        else if (tile_map_object.Class == "enemy_bacteria_super")
                        {
                            game_manager.add_entity(enemy_bacteria.super_form(content, position));
                        }
                        else if (tile_map_object.Class == "enemy_virus_super")
                        {
                            game_manager.add_entity(enemy_virus.super_form(content, position));
                        }
                        else if (tile_map_object.Class == "enemy_mold_super")
                        {
                            game_manager.add_entity(enemy_mold.super_form(content, position));
                        }
                        else if (tile_map_object.Class == "enemy_boss")
                        {
                            game_manager.add_entity(new enemy_boss(content, position));
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
                else if (layer is TilemapObjectLayer other_layer)
                {
                    foreach (TilemapObject tile_map_object in other_layer.Objects)   // load other object (like next_level_portal)
                    {
                        Vector2 position = tile_map_object.Position + new Vector2(tile_map_object.Bounds.Width / 2, -tile_map_object.Bounds.Height / 2);
                        if (tile_map_object.Class == "next_level_portal")
                        {
                            game_manager.next_level_portal = new projectile_next_level_portal(content, position);
                        }
                        if (tile_map_object.Class == "merchant")
                        {
                            game_manager.add_projectile(new projectile_merchant(content, position));
                        }
                    }
                }
            }
            Debug.WriteLine("------- CBA -------- \nLoaded level : " + current_level_file);
        }
    }
}

