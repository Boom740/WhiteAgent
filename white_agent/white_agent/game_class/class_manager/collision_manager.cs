using Microsoft.Xna.Framework;
using MonoGame.Extended;
using MonoGame.Extended.Collisions;
using MonoGame.Extended.Collisions.Layers;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace old_heart
{
    public class collision_manager
    {
        public List<collision_shape> wall_list = new List<collision_shape>();

        Layer default_layer = new Layer(new SpatialHash(new SizeF(128f, 128f))); 
        public CollisionWorld2D collision_world ; // for collision detecting
        public collision_manager()
        {
            collision_world = new CollisionWorld2D(default_layer);
            // make collision system layer
            Layer player_layer = new Layer(new SpatialHash(new SizeF(128f, 128f))); // for player 
            collision_world.AddLayer("player",player_layer);

            Layer player_hitbox_layer = new Layer(new SpatialHash(new SizeF(128f, 128f))); // for player _hitbox
            collision_world.AddLayer("player_hitbox", player_hitbox_layer);

            Layer enemy_layer = new Layer(new SpatialHash(new SizeF(128f, 128f))); // for enemy 
            collision_world.AddLayer("enemy", enemy_layer);

            Layer enemy_hitbox_layer = new Layer(new SpatialHash(new SizeF(128f, 128f))); // for enemy _hitbox
            collision_world.AddLayer("enemy_hitbox", enemy_hitbox_layer);

            Layer wall_layer = new Layer(new SpatialHash(new SizeF(128f, 128f))); // for wall that can't move
            wall_layer.IsDynamic = false;                     // not dynamic mean it will not move
            collision_world.AddLayer("wall", wall_layer);

            Layer enemy_shield_layer = new Layer(new SpatialHash(new SizeF(128f, 128f)));
            collision_world.AddLayer("enemy_shield", enemy_shield_layer);

            collision_world.EnableCollisionBetweenLayers("player_hitbox", "enemy_shield");
            collision_world.EnableCollisionBetweenLayers("player", "enemy_shield");

            collision_world.EnableCollisionBetweenLayers("player", "wall");
            collision_world.EnableCollisionBetweenLayers("player_hitbox", "wall");

            collision_world.EnableCollisionBetweenLayers("enemy", "wall");
            collision_world.EnableCollisionBetweenLayers("enemy_hitbox", "wall");

            collision_world.EnableCollisionBetweenLayers("player_hitbox", "enemy");
            collision_world.EnableCollisionBetweenLayers("enemy_hitbox", "player");
            collision_world.EnableCollisionBetweenLayers("player", "enemy");

            collision_world.EnableCollisionBetweenLayers("player_hitbox", "enemy_hitbox");

        }
        public void add(ICollisionActor collision_object ,String collision_layer_name)
        {
            collision_world.Insert(collision_object, collision_layer_name);
            //Debug.WriteLine("add " + collision_object + " to " +  collision_layer_name);
            if (collision_layer_name == "wall" && collision_object is collision_shape collision_shape)
            {
                wall_list.Add(collision_shape);
            }
        }
        public void remove(ICollisionActor collision_object)
        {
            collision_world.Remove(collision_object);
        }
        public void update(GameTime gameTime)
        {
            float delta_time = (float)gameTime.ElapsedGameTime.TotalSeconds;

            collision_world.RebuildDynamicLayers();

            resolve_hitbox_collision("player_hitbox", "enemy");
            resolve_hitbox_collision("enemy_hitbox", "player");
            resolve_hitbox_collision("player_hitbox", "enemy_hitbox");

            resolve_entity_body_collision("player", "enemy", delta_time);
            resolve_entity_body_collision("enemy", "enemy", delta_time); 

            resolve_wall_collision("player", delta_time);
            resolve_wall_collision("player_hitbox", delta_time);

            resolve_wall_collision("enemy", delta_time);
            resolve_wall_collision("enemy_hitbox", delta_time);

            resolve_shield_collision("player_hitbox", "enemy_shield");
            //resolve_shield_block("player", "enemy_shield", delta_time);

        }

        public void resolve_wall_collision(string layer_that_collide_with_wall ,float delta_time) // use in update only
        {
            var collisionPairs = collision_world.QueryCollisionPairs(layer_that_collide_with_wall, "wall"); // when collision between default and wall layer happen
            foreach (var pair in collisionPairs)
            {
                if (pair.First is collision_shape collision_shape) // if any entity in default collision layer hit wall layer
                {
                    if (collision_shape.owner is entity entity)
                    {
                        entity.position += pair.FirstResult.MinimumTranslationVector;
                        entity.collide_wall(pair, delta_time);
                    }
                    else if (collision_shape.owner is projectile projectile)
                    {
                        projectile.collide_wall(pair, delta_time);
                    }else
                    {
                        Debug.WriteLine("collision_shape.owner is not entity nor projectile : " + collision_shape);
                    }
                }
            }
        }
        public void resolve_hitbox_collision(string hitbox_layer, string target_layer)
        {
            var collisionPairs = collision_world.QueryCollisionPairs(hitbox_layer, target_layer);
            foreach (var pair in collisionPairs)
            {
                if (pair.First is collision_shape hitbox_shape && pair.Second is collision_shape target_shape)
                {
                    if (hitbox_shape.owner is projectile hitbox_projectile)
                    {
                        if(target_shape.owner is entity target_entity)
                        {
                            hitbox_projectile.on_hit_entity(target_entity);
                        }
                        if (target_shape.owner is projectile target_projectile)
                        {
                            hitbox_projectile.on_collide_hit_box(target_projectile);
                        }
                    }
                    
                    else
                    {
                        Debug.WriteLine("hitbox collision owner is not projectile: " + hitbox_shape.owner + " / " + target_shape.owner);
                    }
                }
            }
        }
        public void resolve_entity_body_collision(string layer_a, string layer_b, float delta_time)
        {
            float push_multiplier = 1.5f;   // scale push power
            float minimul_distance = 0.01f;  // prevent too close distance push too much 


            var collisionPairs = collision_world.QueryCollisionPairs(layer_a, layer_b);
            foreach (var pair in collisionPairs)
            {
                if (pair.First is collision_shape shape_a && shape_a.owner is entity entity_a     &&     pair.Second is collision_shape shape_b && shape_b.owner is entity entity_b)
                {
                    Vector2 push_direction = entity_b.position - entity_a.position;  // vector from a to b
                    float push_distance = push_direction.Length();

                    if (push_distance < minimul_distance)
                    {
                        push_distance = minimul_distance;
                        push_direction = new Vector2(1, 0);
                    }
                    float overlapse_distance = ((entity_a.hit_box_radius + entity_b.hit_box_radius) - push_distance);

                    push_direction = Vector2.Normalize(push_direction) * overlapse_distance * push_multiplier;

                    entity_a.velocity -= (push_direction);
                    entity_b.velocity += (push_direction);

                    entity_a.collide_entity(entity_b);
                    entity_b.collide_entity(entity_a);
                }
            }
        }
        public void resolve_shield_collision(string hitbox_layer, string shield_layer)
        {
            var collisionPairs = collision_world.QueryCollisionPairs(hitbox_layer, shield_layer);
            foreach (var pair in collisionPairs)
            {
                if (pair.First is collision_shape hitbox_shape && hitbox_shape.owner is projectile hitbox_projectile
                    && pair.Second is collision_shape shield_shape && shield_shape.owner is enemy shielded_enemy)
                {
                    if (shielded_enemy.alive == false || shielded_enemy.shield == false) { continue; } // โล่พังแล้ว = hitbox หายไป
                    hitbox_projectile.on_hit_shield(shielded_enemy);
                }
            }
        }
        /*public void resolve_shield_block(string player_layer, string shield_layer, float delta_time)
        {
            float shield_eject_speed = 1000f; // ความเร็วสูงสุดที่ดันออกจากวง (px/วินาที)

            var collisionPairs = collision_world.QueryCollisionPairs(player_layer, shield_layer);
            foreach (var pair in collisionPairs)
            {
                if (pair.First is collision_shape player_shape && player_shape.owner is player target_player
                    && pair.Second is collision_shape shield_shape && shield_shape.owner is enemy shielded_enemy)
                {
                    if (target_player.alive == false) { continue; }
                    if (target_player.current_combat_state == player.combat_state.dash) { continue; }
                    if (shielded_enemy.alive == false || shielded_enemy.shield == false || shielded_enemy.shield_blocks_player == false) { continue; }

                    Vector2 to_player = target_player.position - shielded_enemy.shield_center;
                    float distance = to_player.Length();
                    float required_distance = shielded_enemy.shield_radius + target_player.hit_box_radius;
                    if (distance >= required_distance) { continue; }

                    Vector2 normal = distance > 0.001f ? to_player / distance : Vector2.UnitX; // ทิศจากโล่ไปหา player

                    // ดันตำแหน่งกลับมาที่ผิววง (แบบเดียวกับกำแพง)
                    float correction = MathF.Min(required_distance - distance, shield_eject_speed * delta_time);
                    target_player.position += normal * correction;

                    // ตัดความเร็วที่พุ่งเข้าหาโล่ทิ้ง เหลือแค่ความเร็วที่ไถลไปตามผิว
                    float into_shield = Vector2.Dot(target_player.velocity, normal);
                    if (into_shield < 0f)
                    {
                        target_player.velocity -= normal * into_shield;
                    }
                }
            }
        }*/

    }
}