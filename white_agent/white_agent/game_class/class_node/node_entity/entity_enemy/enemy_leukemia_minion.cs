using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System.Diagnostics;

namespace old_heart
{
    public class enemy_leukemia_minion : enemy
    {
        public enum animation_name { idle, walk, died , dizzy }

        public int contact_damage = 1;

        public enemy_leukemia_minion(ContentManager content_set, Vector2 position)
            : base(content_set, position, max_hp: 1, speed: 1000, hit_box_radius: 8)
        {
            animation_player = new animation_player_minion(content_set);
            state = enemy_state.chase; // minion ไล่ล่าทันทีที่เกิด ไม่มี patrol เหมือน leukemia
        }

        public override void Update(GameTime gameTime)
        {
            if (alive == false) return;
            float delta_time = (float)gameTime.ElapsedGameTime.TotalSeconds;

            // shield regen ทำงานอิสระจาก state 
            if (shield == false && state != enemy_state.died)
            {
                shield_timer_current -= delta_time;
                if (shield_timer_current <= 0f)
                {
                    shield = true;
                }
            }

            switch (state)
            {
                case enemy_state.chase:
                    chase_target();
                    break;

                case enemy_state.dizzy:
                    update_dizzy(delta_time);
                    break;

                case enemy_state.died:
                    break;
            }

            base.Update(gameTime);

            void chase_target()
            {
                if (target == null) return;
                Vector2 to_target = target.position - position;
                if (to_target != Vector2.Zero)
                {
                    acceleration = Vector2.Normalize(to_target) * speed;
                    current_direction_vector = acceleration;
                }
            }

            void update_dizzy(float dt)
            {
                acceleration = Vector2.Zero;
                dizzy_timer_current -= dt; 
                if (dizzy_timer_current <= 0f)
                {
                    state = enemy_state.chase; // มึนหมดเวลา กลับมาไล่ต่อ (ถ้ายังไม่โดนจัดการก่อน)
                }
            }
        }

        // ชนกับ entity อื่น 
        public override void collide_entity(entity entity)
        {
            if (alive == false) return;
            if (state != enemy_state.chase) return; // ตอนติด dizzyไม่ระเบิดใส่ผู้เล่นตอนโดนชน

            if (entity is player)
            {
                bool deal_damage = entity.take_damage(contact_damage, damage_dealer: this);
                if (deal_damage == false) { return; }
                die();
            }
        }
        public override void die()
        {
            global.signal.spawn_particle(particle_manager.particle_name.die_efx_purple, position, high_layer: true);
            global.signal.spawn_particle(particle_manager.particle_name.blood_on_ground_efx_purple, position, high_layer: false);
            base.die();
        }
        public override void update_animation(float delta_time)
        {
            if (state == enemy_state.dizzy)
            {
                animation_player.play(animation_player.data.data[animation_name.dizzy]);
            }
            else if (velocity.Length() > 10f)
            {
                animation_player.play(animation_player.data.data[animation_name.walk]);
            }
            else
            {
                animation_player.play(animation_player.default_animation);
            }

            base.update_animation(delta_time);
        }

        public class animation_player_minion : animation_player_base
        {
            public static readonly animation_data animation_data = new animation_data();

            public animation_player_minion(ContentManager content) : base()
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


                texture = content.Load<Texture2D>("assets/image/enemy/sprite_minion_idle");
                animation idle_animation = new animation(texture, frame_per_sec: 2, sprite_size: new Point(64, 64));
                idle_animation.sprite_scale = new Vector2(1f, 1f);
                idle_animation.sprite_origin = new Vector2(32, 48);
                idle_animation.name = "minion idle";
                animation_data.data.Add(animation_name.idle, idle_animation);

                texture = content.Load<Texture2D>("assets/image/enemy/sprite_minion_walk");
                animation walk_animation = new animation(texture, frame_per_sec: 10, sprite_size: new Point(64, 64));
                walk_animation.sprite_scale = new Vector2(1f, 1f);
                walk_animation.sprite_origin = new Vector2(32, 48);
                walk_animation.name = "minion walk";
                animation_data.data.Add(animation_name.walk, walk_animation);

                texture = content.Load<Texture2D>("assets/image/enemy/sprite_minion_dizzy");
                animation dizzy_animation = new animation(texture, frame_per_sec: 10, sprite_size: new Point(64, 64));
                dizzy_animation.sprite_scale = new Vector2(1, 1);
                dizzy_animation.sprite_origin = new Vector2(32, 48);
                dizzy_animation.name = "leukemia dizzy";
                animation_data.data.Add(animation_name.dizzy, dizzy_animation);
            }
        }
    }
}