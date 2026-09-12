using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media;
using MonoGame.Extended;
using MonoGame.Extended.Input;
using MonoGame.Extended.ViewportAdapters;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace old_heart
{ 
    public static class global           // This class is the important !!!!!!!!!!!!!!!!!!
    {
        public static Vector2 render_size = new Vector2(960,540);
        public static int eee = 0;     // for debug only
        public static class input
        {
            public static KeyboardStateExtended keyboard_state;
            public static MouseStateExtended mouse_state;

            public static Point scaled_mouse_position = Point.Zero;
            public static Vector2 scaled_mouse_world_position = Vector2.Zero;
            public static void update_input_state()
            {
                KeyboardExtended.Update(); // update keyboard input 
                MouseExtended.Update(); // update mouse input 

                keyboard_state = KeyboardExtended.GetState();
                mouse_state = MouseExtended.GetState();
            }

            public static void update_scaled_mouse(BoxingViewportAdapter viewport , OrthographicCamera camera) // call from camera manager
            {
                scaled_mouse_position = viewport.PointToScreen(mouse_state.Position);
                scaled_mouse_world_position = camera.ScreenToWorld(mouse_state.Position.ToVector2());
            }
        }
        public static class signal
        {
            public static event Action<projectile> signal_spawn_projectile;
            public static event Action<world_text> signal_spawn_world_text;
            public static event Action<entity> signal_spawn_entity;
            public static event Action<Enum,Vector2,bool> signal_spawn_particle;
            public static event Action<float> signal_screen_shake;
            public static void spawn_projectile(projectile projectile)
            {
                signal_spawn_projectile.Invoke(projectile);
            }
            public static void spawn_world_text(world_text world_text)
            {
                signal_spawn_world_text.Invoke(world_text);
            }
            public static void spawn_entity(entity entity)
            {
                signal_spawn_entity.Invoke(entity);
            }
            public static void spawn_particle(Enum particle_name, Vector2 position, bool high_layer = false)
            {
                signal_spawn_particle.Invoke(particle_name,position,high_layer);
            }
            public static void screen_shake(float intensity)
            {
                signal_screen_shake.Invoke(intensity);
            }
        }
        public static class theme {
            public static SpriteFont default_font;
            public static Vector2 default_font_scale = new Vector2(1, 1);
            public static void load(ContentManager content)
            {
                default_font = content.Load<SpriteFont>("assets/font/test_font");
                default_font.Spacing = 1f;
            }
        }
        public static class sound {
            public enum sound_name { test1, test2 };  
            public enum song_name { test1 }; 

            public static Dictionary<sound_name, sound_data> data = new Dictionary<sound_name, sound_data>();
            public static Dictionary<song_name, song_data> data_song = new Dictionary<song_name, song_data>();
            public static void play_sound(sound_name sound_name)
            {
                sound_data sound_data = data[sound_name];
                sound_data.sound_effect.Play(sound_data.volume, sound_data.pitch, pan: 0);
                
                Debug.WriteLine("global play sound : "+ sound_data.name);
            }
            public static void play_song(song_name song_name)  // maybe later  add sound fade out before play if there is already song playing
            {
                song_data song_data = data_song[song_name];
                MediaPlayer.Volume = song_data.volume;
                MediaPlayer.Play(song_data.song);

                Debug.WriteLine("global play song : " + song_data.name);
            }
            public static void pause_song()
            {
                MediaPlayer.Pause();
            }
            public static void resume_song()
            {
                MediaPlayer.Resume();
            }
            public static void load(ContentManager content)
            {
                load_sound();
                load_song();
                MediaPlayer.IsRepeating = true;

                void load_sound()
                {
                    sound_data sound_data = new sound_data(content.Load<SoundEffect>("Placeholder/SFX/Slash"));
                    data.Add(sound_name.test1, sound_data);
                }
                void load_song()
                {
                    //song_data song_data = new song_data(content.Load<Song>("Placeholder/"));
                    //data_song.Add(song_name.test1, song_data);
                }
            }

            public class sound_data
            {
                public string name;     // for debug 
                public SoundEffect sound_effect;
                public float volume;
                public float pitch;

                public sound_data(SoundEffect sound_effect, float volume = 1, float pitch = 0)
                {
                    this.sound_effect = sound_effect;
                    name = sound_effect.Name;
                    this.volume = volume;
                    this.pitch = pitch;
                }
            }
            public class song_data
            {
                public string name;     // for debug 
                public Song song;
                public float volume;

                public song_data(Song song, float volume = 1)
                {
                    this.song = song;
                    name = song.Name;
                    this.volume = volume;
                }
            }
        }

        public static void load(ContentManager content)
        {
            theme.load(content);
            sound.load(content);
        }
    }
}