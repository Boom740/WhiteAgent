using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using MonoGame.Extended.Collisions;
using MonoGame.Extended.Screens;
using MonoGame.Extended.Screens.Transitions;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace old_heart
{
    public class game_manager  // manage everything in game scene
    {
        public ContentManager content;

        public ui_manager ui_manager;
        public map_manager map_manager;
        public entity_manager entity_manager;
        public camera_manager camera_manager;
        public particle_manager particle_manager;
        public projectile_manager projectile_manager;
        public collision_manager collision_manager;
        public level_manager level_manager;

        public debug_manager debug_manager;

        Queue<projectile> signal_spawn_projectile_queue = new Queue<projectile>();
        Queue<entity> signal_spawn_entity_queue = new Queue<entity>();

        public player player;

        public bool pause = false;
        public bool game_over = false;
        public bool level_clear = false;

        public game_manager(ContentManager content,GameWindow window,GraphicsDevice graphics_device)
        {
            this.content = content;

            ui_manager = new ui_manager();
            map_manager = new map_manager();
            entity_manager = new entity_manager();
            camera_manager = new camera_manager(window,graphics_device);
            particle_manager = new particle_manager(content); 
            projectile_manager = new projectile_manager();
            collision_manager = new collision_manager();
            level_manager = new level_manager(this);

            debug_manager = new debug_manager();

            global.signal.signal_spawn_projectile += handle_signal_add_projectile;
            global.signal.signal_spawn_entity += handle_signal_add_entity;
            global.signal.signal_spawn_particle += add_particle;
        }
        public void add_ui(node node)
        {
            ui_manager.add(node);
        }
        public void add_map(node node, bool high_ground = false)
        {
            map_manager.add(node, high_ground);
        }
        public void add_map_collision(collision_shape collision_object)
        {
            collision_manager.add(collision_object,"wall");
            debug_manager.add(collision_object);
        }
        public void add_particle(Enum particle_name , Vector2 position ,bool high_layer)
        {
            particle_manager.add(particle_name ,position ,high_layer);
        }
        public void add_entity(entity entity)
        {
            if (entity_manager.entity_list.Count >= entity_manager.limit)
            {
                Debug.WriteLine("cant spawn entity at limit count : " + entity_manager.entity_list.Count);
                return;
            }
            entity_manager.add(entity);
            if (entity is player player)
            {
                this.player = player;
                collision_manager.add(player.collision, "player");
            }
            else
            {
                collision_manager.add(entity.collision, "enemy");
                if (entity is enemy_leukemia enemy_leukemia)
                {
                    enemy_leukemia.target = this.player; // ให้ enemy รู้จัก player เพื่อเช็คระยะ dangerous_rad/safe_rad
                }
            }
            debug_manager.add(entity.collision);
        }
        public void add_projectile(projectile projectile)
        {
            if (projectile_manager.projectile_list.Count >= projectile_manager.limit)
            {
                Debug.WriteLine("cant spawn projectile at limit count : " + projectile_manager.projectile_list.Count);
                return;
            }
            projectile_manager.add(projectile);
            if (projectile.owner == player)
            {
                //Debug.WriteLine("add projectile from player");  
                collision_manager.add(projectile.collision, "player_hitbox");
            }
            else
            {
                collision_manager.add(projectile.collision, "enemy_hitbox");
            }
            debug_manager.add(projectile.collision);
        }
        public void update(GameTime gameTime)
        {
            ui_manager.update(gameTime);
            debug_manager.update(gameTime);

            

            if (pause || level_clear)
            {
                camera_manager.update_global_mouse_position();
                return;
            }

            if (entity_manager.entity_list.Any(entity => entity is enemy) == false && level_manager.next_level_file != null)   // if no enemy is left in entity_list
            {
                Debug.WriteLine("level clear !");
                level_clear = true;
            }
            else
            {
                //Debug.WriteLine("level not clea  " + level_manager.next_level_file);
            }

            //map_manager.update(gameTime);  map don't update lol


            entity_manager.update(gameTime);
            camera_manager.update(gameTime, player);
            particle_manager.update(gameTime);
            projectile_manager.update(gameTime);
            collision_manager.update(gameTime);

            clear_inactive_node();
            spawn_queue_signal();
        }
        public void clear_inactive_node()
        {

            clear_inactive_node_in_entity();
            clear_inactive_node_in_projectile();


            void clear_inactive_node_in_entity() 
            {
                List<entity> inactive_entity = entity_manager.entity_list.Where(node => node.active == false).ToList();
                foreach (entity entity in inactive_entity)
                {
                    entity_manager.remove(entity);
                    if (entity.collision is ICollisionActor actor) {
                        collision_manager.remove(actor); // remove it from collision manager
                        //Debug.WriteLine("game_manager test collision entity removed " + entity);
                    }
                    if (entity.collision is node node)
                    {
                        debug_manager.remove(node);
                    }
                    if (entity is player)
                    {
                        player = null;
                    }
                }
            }
            void clear_inactive_node_in_projectile()
            {
                List<projectile> inactive_projectile = projectile_manager.projectile_list.Where(node => node.active == false).ToList();
                foreach (projectile projectile in inactive_projectile)
                {
                    projectile_manager.remove(projectile);
                    if (projectile.collision is ICollisionActor actor)
                    {
                        collision_manager.remove(actor); // remove it from collision manager 
                        //Debug.WriteLine("game_manager test collision projectile removed " + projectile);
                    }
                    if (projectile.collision is node node)
                    {
                        debug_manager.remove(node);
                    }
                }
            }
        }
        public void spawn_queue_signal()
        {
            while (signal_spawn_entity_queue.Count > 0)
            {
                entity signal_entity = signal_spawn_entity_queue.Dequeue();
                add_entity(signal_entity);
            }
            while (signal_spawn_projectile_queue.Count > 0)
            {
                projectile signal_projectile = signal_spawn_projectile_queue.Dequeue();
                add_projectile(signal_projectile);
            }
        }
        public void handle_signal_add_projectile(projectile projectile)
        {
            signal_spawn_projectile_queue.Enqueue(projectile);
        }
        public void handle_signal_add_entity(entity entity)
        {
            signal_spawn_entity_queue.Enqueue(entity);
        }
        public void draw(SpriteBatch sprite_batch)
        {
            Matrix ui_camera_matrix = camera_manager.ui_camera.GetViewMatrix();
            Matrix camera_matrix = camera_manager.camera.GetViewMatrix();

            sprite_batch.Begin(samplerState: SamplerState.PointClamp , transformMatrix: camera_matrix);    // low layer

            RectangleF blue_backgound_rectangle = new RectangleF(camera_manager.camera.Position.X, camera_manager.camera.Position.Y, camera_manager.viewport_adapter.VirtualWidth, camera_manager.viewport_adapter.VirtualHeight);
            sprite_batch.FillRectangle(blue_backgound_rectangle, Color.CornflowerBlue);  // blue_background

            map_manager.draw_low(sprite_batch);
            particle_manager.draw_low(sprite_batch);
            sprite_batch.End();

            sprite_batch.Begin(samplerState: SamplerState.PointClamp , sortMode: SpriteSortMode.FrontToBack, transformMatrix: camera_matrix); // sorted entity and projectile layer
            entity_manager.draw(sprite_batch);
            projectile_manager.draw(sprite_batch);
            sprite_batch.End();

            sprite_batch.Begin(samplerState: SamplerState.PointClamp, transformMatrix: camera_matrix);  // high layer
            particle_manager.draw_high(sprite_batch);
            map_manager.draw_high(sprite_batch);
            debug_manager.draw(sprite_batch); // debug 
            sprite_batch.End();

            sprite_batch.Begin(samplerState: SamplerState.PointClamp, transformMatrix: ui_camera_matrix); // ui layer
            ui_manager.draw(sprite_batch);
            sprite_batch.End();


        }
        public void unload()
        {
            global.signal.signal_spawn_projectile -= handle_signal_add_projectile;
            global.signal.signal_spawn_entity -= handle_signal_add_entity;
            global.signal.signal_spawn_particle -= add_particle;
        }
    }
}