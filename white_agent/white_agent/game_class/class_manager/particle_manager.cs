using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended.Graphics;
using MonoGame.Extended.Input;
using MonoGame.Extended.Particles;
using MonoGame.Extended.Particles.Data;
using MonoGame.Extended.Particles.Modifiers;
using MonoGame.Extended.Particles.Modifiers.Interpolators;
using MonoGame.Extended.Particles.Profiles;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace old_heart
{
    public class particle_manager
    {
        static public Dictionary<Enum, ParticleEmitter> data = new Dictionary<Enum, ParticleEmitter>();
        public enum particle_name { die_efx_red , die_efx_green, die_efx_purple , die_efx_white , blood_on_ground_efx_red , blood_on_ground_efx_green , blood_on_ground_efx_purple , blood_on_ground_efx_white
        , ground_smash_efx , enemy_summon_efx
        }

        public ParticleEffect low_particle_effect;
        public ParticleEffect high_particle_effect;

        public ContentManager content;
        public Texture2D default_particle_texture;


        public int particle_limit = 670;

        public particle_manager(ContentManager content)
        {
            this.content = content;
            default_particle_texture = content.Load<Texture2D>("assets/image/other/white_pixel");
            low_particle_effect = new ParticleEffect("low_particle_effect")
            {
                Position = new Vector2(0, 0),
                AutoTrigger = false
            };
            high_particle_effect = new ParticleEffect("high_particle_effect")
            {
                Position = new Vector2(0, 0),
                AutoTrigger = false
            };
            if(data.Count == 0)
            {
                load();
            }
        }

        public void add(Enum particle_name, Vector2 position, bool high_layer)
        {
            ParticleEmitter saved_particle = data[particle_name];
            ParticleEmitter new_particle = new ParticleEmitter(saved_particle.Capacity)
            {
                Name = saved_particle.Name,
                LifeSpan = saved_particle.LifeSpan,
                TextureRegion = saved_particle.TextureRegion,
                Profile = saved_particle.Profile,
                ModifierExecutionStrategy = ModifierExecutionStrategy.Serial,
                Offset = saved_particle.Offset,

                Parameters = new ParticleReleaseParameters
                {
                    Quantity = saved_particle.Parameters.Quantity,
                    Speed = saved_particle.Parameters.Speed,
                    Color = saved_particle.Parameters.Color,
                    Scale = saved_particle.Parameters.Scale,
                    Opacity = saved_particle.Parameters.Opacity,
                    Rotation = saved_particle.Parameters.Rotation
                }
            };
            foreach (Modifier modifier in saved_particle.Modifiers)
            {
                new_particle.Modifiers.Add(modifier);
            }

            if (high_layer) 
            { 
                high_particle_effect.Emitters.Add(new_particle);
            }
            else 
            {
                low_particle_effect.Emitters.Add(new_particle);
            }

            new_particle.Trigger(position);
        }
        public void update(GameTime gameTime)
        {
            float delta_time = (float)gameTime.ElapsedGameTime.TotalMilliseconds / 1000;

            KeyboardStateExtended keyboard_state = global.input.keyboard_state;

            if (keyboard_state.WasKeyReleased(Keys.Z))
            {
                global.signal.spawn_particle(particle_name.enemy_summon_efx, global.input.scaled_mouse_world_position, high_layer: true);
            }

            low_particle_effect.Update(delta_time);
            high_particle_effect.Update(delta_time);
        }
        public void draw_low(SpriteBatch sprite_batch)
        {
            sprite_batch.Draw(low_particle_effect);
        }
        public void draw_high(SpriteBatch sprite_batch)
        {
            sprite_batch.Draw(high_particle_effect);
        }

        public void load()
        {
            load_die_efx_color(Color.Red);
            load_die_efx_color(Color.Green);
            load_die_efx_color(Color.Purple);
            load_die_efx_color(Color.White);

            load_blood_on_ground_efx_color(Color.Red);
            load_blood_on_ground_efx_color(Color.Green);
            load_blood_on_ground_efx_color(Color.Purple);
            load_blood_on_ground_efx_color(Color.White);

            load_ground_smash_efx();
            load_enemy_summon_efx();

            void load_die_efx_color(Color color)
            {
                ParticleEmitter emitter;
                string this_particle_name = "die_efx_";
                string color_name = "";

                float color_hue;
                float color_saturation;
                float color_lightness;
                color.ToHSL(out color_hue,out color_saturation,out color_lightness);

                if (color == Color.Red)
                {
                    color_name = "red";
                }
                else if (color == Color.Green)
                {
                    color_name = "green";
                }
                else if (color == Color.Purple)
                {
                    color_name = "purple";
                }
                else if (color == Color.White)
                {
                    color_name = "white";
                }

                emitter = new ParticleEmitter(20)
                {
                    Name = this_particle_name + color_name ,
                    LifeSpan = 1.0f,
                    TextureRegion = new Texture2DRegion(default_particle_texture),
                    Profile = Profile.Spray(-Vector2.UnitY, 4.0f),
                    Parameters = new ParticleReleaseParameters
                    {
                        Quantity = new ParticleInt32Parameter(10, 20),
                        Speed = new ParticleFloatParameter(50f, 700f),
                        Color = new ParticleColorParameter(new Vector3(color_hue , color_saturation / 100f, color_lightness / 100f )),
                        Scale = new ParticleVector2Parameter(new Vector2(10f, 10f))
                    }
                };

                emitter.Modifiers.Add(new LinearGravityModifier
                {
                    Direction = Vector2.UnitY,
                    Strength = 100f
                });
                emitter.Modifiers.Add(new OpacityFastFadeModifier());
                emitter.Modifiers.Add(new DragModifier
                {
                    Density = 10f
                });

                add_into_data(this_particle_name + color_name , emitter);
            }
            void load_blood_on_ground_efx_color(Color color)
            {
                ParticleEmitter emitter;
                string this_particle_name = "blood_on_ground_efx_";
                string color_name = "";
                float color_hue;
                float color_saturation;
                float color_lightness;
                color.ToHSL(out color_hue, out color_saturation, out color_lightness);

                if (color == Color.Red)
                {
                    color_name = "red";
                }
                else if (color == Color.Green)
                {
                    color_name = "green";
                }
                else if (color == Color.Purple)
                {
                    color_name = "purple";
                }
                else if (color == Color.White)
                {
                    color_name = "white";
                }

                emitter = new ParticleEmitter(20)
                {
                    Name = this_particle_name + color_name,
                    LifeSpan = 5.0f,
                    TextureRegion = new Texture2DRegion(default_particle_texture),
                    Profile = Profile.Circle(0,CircleRadiation.Out),
                    Parameters = new ParticleReleaseParameters
                    {
                        Quantity = new ParticleInt32Parameter(10, 20),
                        Speed = new ParticleFloatParameter(300f, 0f),
                        Color = new ParticleColorParameter(new Vector3(color_hue, color_saturation/100f, color_lightness / 100f )),
                        Scale = new ParticleVector2Parameter(new Vector2(3f, 3f))
                    }
                };

                emitter.Modifiers.Add(new OpacityFastFadeModifier());
                emitter.Modifiers.Add(new AgeModifier
                {
                    Interpolators = { new ScaleInterpolator { StartValue = new Vector2(10,10), EndValue = new Vector2(20, 20) } }
                });
                emitter.Modifiers.Add(new DragModifier
                {
                    Density = 30f
                });
                add_into_data(this_particle_name + color_name, emitter);
            }
            
            void load_ground_smash_efx()
            {
                ParticleEmitter emitter = new ParticleEmitter(20)
                {
                    Name = "ground_smash_efx",
                    LifeSpan = 1f,
                    TextureRegion = new Texture2DRegion(default_particle_texture),
                    Profile = Profile.Spray(new Vector2(0,-1), MathHelper.PiOver2),
                    Parameters = new ParticleReleaseParameters
                    {
                        Quantity = new ParticleInt32Parameter(10, 20),
                        Speed = new ParticleFloatParameter(200f, 400f),
                        Color = new ParticleColorParameter(new Vector3(15f, 0.5f, 0.3f)),
                        Scale = new ParticleVector2Parameter(new Vector2(10f, 10f))
                    }
                };
                emitter.Modifiers.Add(new DragModifier
                {
                    Density = 5f
                });
                emitter.Modifiers.Add(new LinearGravityModifier
                {
                    Direction = Vector2.UnitY,
                    Strength = 400f
                });
                emitter.Modifiers.Add(new OpacityFastFadeModifier());


                data.Add(particle_name.ground_smash_efx, emitter);
            }

            void load_enemy_summon_efx()
            {
                ParticleEmitter emitter = new ParticleEmitter(20)
                {
                    Name = "enemy_summon_efx",
                    LifeSpan = 0.7f,
                    TextureRegion = new Texture2DRegion(default_particle_texture),
                    Profile = Profile.Line(new Vector2(1, 0), 50, LineRadiation.PerpendicularUp),
                    Parameters = new ParticleReleaseParameters
                    {
                        Quantity = new ParticleInt32Parameter(10, 20),
                        Speed = new ParticleFloatParameter(100f, 400f),
                        Color = new ParticleColorParameter(new Vector3(67f, 0f, 0.9f)),
                        Scale = new ParticleVector2Parameter(new Vector2(15f, 15f)),
                        Opacity = new ParticleFloatParameter(0.5f)
                    }
                };
                emitter.Modifiers.Add(new DragModifier
                {
                    Density = 15f
                });
                emitter.Modifiers.Add(new LinearGravityModifier
                {
                    Direction = Vector2.UnitY,
                    Strength = 100f
                });
                emitter.Modifiers.Add(new OpacityFastFadeModifier());


                data.Add(particle_name.enemy_summon_efx, emitter);
            }

            void add_into_data(string this_particle_name , ParticleEmitter emitter)
            {
                if (Enum.TryParse<particle_name>(this_particle_name, out particle_name particle_enum))
                {

                    data.Add(particle_enum, emitter);
                    //Debug.WriteLine("  prase efx name : " + this_particle_name);
                }
                else
                {
                    Debug.WriteLine("cant prase efx name : " + this_particle_name);
                }
            }
        }

    }
}