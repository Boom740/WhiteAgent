using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace old_heart
{
    public class enemy_boss : enemy
    {
        public enum animation_name { idle , attack , tired , dizzy, died }

        // --- summon ---
        public List<enemy> summoned_enemy_list = new List<enemy> { };
        public Vector2 summoned_enemy_spawn_offset = new Vector2 (0,67);
        public bool summoned_enemy = false;
        public float summon_time = 1f;
        public float summon_timer = 0f;


        //ranged attack
        public float attack_cooldown = 5f;
        private float attack_cooldown_timer = 0f;

        public int shots_burst_count = 0;  // number of shot shoot at same time
        public int shots_per_burst = 0;  // number of time enemy will shoot
        public float shot_interval = 0.5f;
        private int shots_fired_in_burst = 0;
        private float shot_timer = 0f;
        public float bullet_spread_angle = MathHelper.ToRadians(30);   // 10 degree


        public enemy_boss(ContentManager content_set, Vector2 position) : base(content_set, position, max_hp: 3 , speed: 0, hit_box_radius: 67)
        {
            animation_player = new animation_player_leukemia(content_set);

            entity_push_enable = false;
            //animation_player_shield.visible = false;

            shadow_color = Color.White;
            shadow_texture = content.Load<Texture2D>("assets/image/other/sprite_bosshole");
            shadow_scale = Vector2.One;
            shadow_origin = new Vector2(shadow_texture.Width, shadow_texture.Height) / 2;   // center

            drop_count_min = 67;
            drop_count_max = 67;

            attack_cooldown_timer = attack_cooldown;
        }
        public override void Update(GameTime gameTime)
        {
            if (alive == false) return;
            float delta_time = (float)gameTime.ElapsedGameTime.TotalSeconds;

            summoned_enemy_list.RemoveAll(enemy => enemy.alive == false);     // clear dead minion

            switch (state)
            {
                case enemy_state.normal:
                    update_idle(delta_time);
                    break;
                case enemy_state.attack:
                    update_attack(delta_time);
                    break;
                case enemy_state.summoning:
                    update_summoning(delta_time);
                    break;
                case enemy_state.dizzy:
                    update_dizzy(delta_time);
                    break;
                case enemy_state.died:
                    break;
            }

            base.Update(gameTime); // ให้ entity จัดการ velocity/position/animation/collision ตามปกติ

            // ---------------- Normal ----------------
            void update_idle(float delta_time)
            {
                if (summoned_enemy_list.Count <= 0)
                {
                    if (summoned_enemy == false)
                    {
                        summoned_enemy = true;
                        state = enemy_state.summoning;
                    }
                    else
                    {
                        state = enemy_state.tired;
                    }
                }

                if (attack_cooldown_timer > 0)
                {
                    attack_cooldown_timer -= delta_time;
                }
                else
                {
                    enter_attack();
                }
            }
            void update_attack(float delta_time)
            {
                if (target == null) { state = enemy_state.normal; return; }

                current_direction_vector = target.position - position;

                if (shots_fired_in_burst >= shots_per_burst) // finish shooting
                {
                    attack_cooldown_timer = attack_cooldown;
                    shots_fired_in_burst = 0;
                    state = enemy_state.normal;
                    return;
                }

                shot_timer -= delta_time;
                if (shot_timer <= 0f)
                {
                    float bullet_spread_sum = bullet_spread_angle * (shots_burst_count-1);

                    for(int i = 0; i < shots_burst_count; i++)
                    {
                        Vector2 bullet_direction = current_direction_vector;
                        float rotate = (bullet_spread_sum / 2) - (i * bullet_spread_angle);
                        bullet_direction.Rotate(rotate);
                        fire_projectile(bullet_direction);
                    }

                    shots_fired_in_burst++;
                    shot_timer = shot_interval;
                }


                void fire_projectile(Vector2? override_direction = null)
                {
                    Vector2 aim_direction = override_direction ?? (target.position - position);
                    if (aim_direction == Vector2.Zero) aim_direction = current_direction_vector;

                    projectile_enemy_bullet shot = new projectile_enemy_bullet(content, position, aim_direction, projectile_speed : 150f, damage:1, knockback_speed:30f , lifetime:3f);
                    shot.owner = this;
                    global.signal.spawn_projectile(shot);
                }
            }
            void update_summoning(float delta_time)
            {
                if (summon_timer > 0)
                {
                    summon_timer -= delta_time;
                    return;
                }

                int stage = 4 - hp;
                 
                shots_burst_count++;
                shots_per_burst++;

                if (stage == 1)
                {
                    spawn_enemy(new enemy_bacteria(content, position));
                }
                else if (stage == 2)
                {
                    spawn_enemy(new enemy_bacteria(content, position));
                    spawn_enemy(new enemy_bacteria(content, position));
                }
                else if (stage == 3)
                {
                    spawn_enemy(new enemy_bacteria(content, position)); 
                    spawn_enemy(new enemy_bacteria(content, position));
                    spawn_enemy(new enemy_bacteria(content, position));
                }

                state = enemy_state.normal;
            }
            // ---------------- Dizzy ----------------

            void update_dizzy(float delta_time)
            {
                // do nothing
            }

            void enter_attack()
            {
                state = enemy_state.attack;
                shots_fired_in_burst = 0;
                shot_timer = 0f;
            }
        }

        public override void enter_dizzy(bool break_shield = true)
        {
            if (state != enemy_state.tired) { return; }  // cant dizzy if not tired
            if (! break_shield) { return; } // dont dizzy if not break shield

            base.enter_dizzy(break_shield : true);
        }

        public override void apply_knockback(Vector2 direction, float speed)
        {
            // no knock back 
        }
        public override bool take_damage(int damage_taken, node damage_dealer = null)
        {
            bool took_damage = base.take_damage(1 , damage_dealer);
            if ( ! took_damage ) { return false; }
            if (! alive) { return true; }

            shield = true;
            state = enemy_state.normal;
            attack_cooldown_timer = attack_cooldown;
            summon_timer = summon_time;
            summoned_enemy = false;

            return took_damage; 
        }
        void spawn_enemy(enemy new_enemy)
        {
            new_enemy.drop_count_min = 0;
            new_enemy.drop_count_max = 0;
            new_enemy.position += summoned_enemy_spawn_offset;
            global.signal.spawn_entity(new_enemy);
            summoned_enemy_list.Add(new_enemy);
        }
        public override void die()
        {
            global.signal.spawn_particle(particle_manager.particle_name.die_efx_green, position, high_layer: true);
            global.signal.spawn_particle(particle_manager.particle_name.blood_on_ground_efx_green, position, high_layer: false);
            base.die();
        }

        public override void update_animation(float delta_time)
        {
            if (state == enemy_state.attack)
            {
                animation_player.play(animation_player.data.data[animation_name.attack]);
            }
            if (state == enemy_state.tired)
            {
                animation_player.play(animation_player.data.data[animation_name.tired]);
            }
            else if(state == enemy_state.dizzy)
            {
                animation_player.play(animation_player.data.data[animation_name.dizzy]);
            }
            else
            {
                animation_player.play(animation_player.default_animation);
            }

            base.update_animation(delta_time);
        }



        public class animation_player_leukemia : animation_player_base
        {
            public static readonly animation_data animation_data = new animation_data();

            public animation_player_leukemia(ContentManager content) : base()
            {
                if (animation_data.data.Count == 0)
                {
                    load(content);
                }

                base.data = animation_data;

                default_animation = animation_data.data[animation_name.idle];
                current_animation = default_animation;
            }

            public void load(ContentManager content)
            {
                Texture2D texture;

                texture = content.Load<Texture2D>("assets/image/enemy/sprite_boss_idle");
                animation idle_animation = new animation(texture, frame_per_sec: 8 ,sprite_size: new Point(256,256) , one_direction_sprite_format: true , one_diretion_sprite_format_frame_count:1);
                idle_animation.sprite_origin = new Vector2(128,196);
                idle_animation.name = "boss idle";
                animation_data.data.Add(animation_name.idle, idle_animation);

                texture = content.Load<Texture2D>("assets/image/enemy/sprite_boss_idle");
                animation walk_animation = new animation(texture, frame_per_sec: 8, sprite_size: new Point(256, 256), one_direction_sprite_format: true, one_diretion_sprite_format_frame_count: 1);
                walk_animation.sprite_origin = new Vector2(128, 196);
                walk_animation.name = "boss attack";
                animation_data.data.Add(animation_name.attack, walk_animation);

                texture = content.Load<Texture2D>("assets/image/enemy/sprite_boss_idle");
                animation tired_animation = new animation(texture, frame_per_sec: 12, sprite_size: new Point(256, 256), one_direction_sprite_format: true, one_diretion_sprite_format_frame_count: 1);
                tired_animation.sprite_origin = new Vector2(128, 196);
                tired_animation.name = "boss tired"; 
                animation_data.data.Add(animation_name.tired, tired_animation);

                texture = content.Load<Texture2D>("assets/image/enemy/sprite_boss_idle");
                animation dizzy_animation = new animation(texture, frame_per_sec: 12, sprite_size: new Point(256, 256), one_direction_sprite_format: true, one_diretion_sprite_format_frame_count: 1);
                dizzy_animation.sprite_origin = new Vector2(128, 196);
                dizzy_animation.name = "boss dizzy";
                animation_data.data.Add(animation_name.dizzy, dizzy_animation);
            }
        }
    }
}