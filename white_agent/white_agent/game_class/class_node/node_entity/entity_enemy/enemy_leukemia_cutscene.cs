using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using static old_heart.player;

namespace old_heart
{
    public class enemy_leukemia_cutscene : enemy
    {
        public enum animation_name { idle, walk }
        public Vector2 initial_position;
        public float walk_distance = 800f;
        public float die_radius = 200;
        public bool finish_walk = false;
        public float walk_wait_time = 2;
        public float walk_wait_timer = 0;

        public float die_animation_timer = 0;   // for logic after die   time
        public float die_screen_shake_interval = 0.67f;
        public float die_screen_shake_timer = 0;
        public enemy_leukemia_cutscene(ContentManager content_set, Vector2 position) : base(content_set, position, max_hp: 40 , speed: 3600, hit_box_radius: 15)
        {
            animation_player = new animation_player_leukemia_cutscene(content_set);
            initial_position = position;

            entity_push_enable = false;
            can_be_dizzied = false;

            this.drop_count_min = 0;
            this.drop_count_max = 0;

        }
        public override void Update(GameTime gameTime)
        {
            float delta_time = (float)gameTime.ElapsedGameTime.TotalSeconds;

            switch (state)
            {
                case enemy_state.normal:
                    update_idle();
                    break;

                case enemy_state.frightened:
                    update_frightened(delta_time);
                    break;

                case enemy_state.dizzy:
                    update_dizzy(delta_time);
                    break;

                case enemy_state.died:
                    update_died(delta_time);
                    break;
            }

            base.Update(gameTime); // ให้ entity จัดการ velocity/position/animation/collision ตามปกติ

            // ---------------- Normal ----------------


            void update_idle()
            {
                if (target == null) return;
                float distance = Vector2.Distance(position, target.position);
                if (finish_walk == false)
                {
                    if (walk_wait_timer < walk_wait_time)
                    {
                        walk_wait_timer += delta_time;
                    }
                    else
                    {
                        enter_frightened();
                    }
                }
                else
                {
                    if (distance <= die_radius)
                    {
                        die();
                    }
                }
               
            }

            // ---------------- Frightened ----------------

            void enter_frightened()
            {
                state = enemy_state.frightened;
            }

            void update_frightened(float delta_time)
            {
                if (target == null) return;

                Vector2 away_direction = position - initial_position;
                float distance = away_direction.Length();


                if (distance >= walk_distance)
                {
                    state = enemy_state.normal;
                    finish_walk = true;
                    acceleration = Vector2.Zero;   // stop when return to normal state
                    current_direction_vector = new Vector2(0, 1);
                }
                else
                {
                    acceleration = new Vector2(0, -1) * speed;
                    current_direction_vector = acceleration;
                }
            }

            

          
            // ---------------- Dizzy ----------------

            void update_dizzy(float delta_time)
            {
                acceleration = Vector2.Zero;

                dizzy_timer_current -= delta_time;
                if (dizzy_timer_current <= 0f)
                {
                    state = enemy_state.normal;
                }
            }
            void update_died(float delta_time)
            {
                if (die_screen_shake_timer > 0f)
                {
                    die_screen_shake_timer -= delta_time;
                }
                else
                {
                    global.signal.screen_shake(1.0f);
                    die_screen_shake_timer = die_screen_shake_interval;
                }

                die_animation_timer += delta_time;
                if (die_animation_timer > 5)
                {
                    active = false;
                }
                else if (die_animation_timer > 3)
                {
                    die_screen_shake_interval = 0.3f;
                }
            }
        }
        public override bool take_damage(int damage_taken, node damage_dealer = null)
        {
            return false;  // cant take_damage
        }
        public override void apply_knockback(Vector2 direction, float speed)
        {
            // cant knock back
        }
        public override void die()
        {
            if (!alive ) { return; }
            if (target == null) return;
            player player  = target as player;
            state = enemy_state.died;
            global.signal.spawn_particle(particle_manager.particle_name.die_efx_purple, position, high_layer: true);
            global.signal.spawn_particle(particle_manager.particle_name.blood_on_ground_efx_purple, position, high_layer: false);
            global.sound.play_sound(global.sound.sound_name.enemy_die);
            visible = false;
            alive = false;

            player.change_scene_fall_animation = false;
            player.current_combat_state = player.combat_state.change_scene;
            player.animation_player.play(player.animation_player.data.data[animation_player_player.animation_name.idle]);
        }

        public override void update_animation(float delta_time)
        {
            if (velocity.Length() > 10f)
            {
                animation_player.play(animation_player.data.data[animation_name.walk]);
            }
            else
            {
                animation_player.play(animation_player.default_animation);
            }

            base.update_animation(delta_time);
        }

        public class animation_player_leukemia_cutscene : animation_player_base
        {
            public static readonly animation_data animation_data = new animation_data();

            public animation_player_leukemia_cutscene(ContentManager content) : base()
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
                //Point placeholder_sprite_size = new Point(16, 16); // sprite sheet ของ Beast.png คือ 4x4 ช่อง ช่องละ 16x16

                Texture2D texture;

                texture = content.Load<Texture2D>("assets/image/enemy/sprite_leukemia_idle");
                animation idle_animation = new animation(texture, frame_per_sec: 8 ,sprite_size: new Point(64,64));
                idle_animation.sprite_scale = new Vector2(1,1);
                idle_animation.sprite_origin = new Vector2(32,57);
                idle_animation.name = "leukemia idle";
                animation_data.data.Add(animation_name.idle, idle_animation);

                texture = content.Load<Texture2D>("assets/image/enemy/sprite_leukemia_Walk");
                animation walk_animation = new animation(texture, frame_per_sec: 8, sprite_size: new Point(64, 64));
                walk_animation.sprite_scale = new Vector2(1, 1);
                walk_animation.sprite_origin = new Vector2(32, 57);
                walk_animation.name = "leukemia walk";
                animation_data.data.Add(animation_name.walk, walk_animation);

            }
        }
    }
}