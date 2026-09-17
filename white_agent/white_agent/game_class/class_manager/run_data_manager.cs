using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace old_heart
{
    public class run_data_manager
    {
        public Game game;
        private Random random = new Random();

        public int hp_left = 4;
        public int respawn_left = 2;

        public float shock_wave_radius = 18;

        public List<string> level_list = new List<string>();
        public int cleared_level = 0;
        public run_data_manager(Game game)
        {
            this.game = game;

            load();
        }
        public void load()
        {
            List<string> level_set_1 = new List<string>() { "level_1", "level_2", "level_3", "level_4", "level_5" };
            string level_boss_1 = "level_boss_1";
            List<string> level_set_2 = new List<string>();

            List<string> level_set_3 = new List<string>();

            random_level_order(level_set_1);

            //level_list.AddRange(level_set_1);
            //level_list.Add(level_boss_1);

            level_list.Add("test_1");
            level_list.Add("test_2");

            foreach (string level in level_list)
            {
                Debug.WriteLine("level_list " +  level);
            }  // debug

            void random_level_order(List<string> level_set) 
            {
                int i = level_set.Count;
                while (i > 0)
                {
                    i--;
                    int random_index = random.Next(i + 1);

                    string temp_level = level_set[i];       // swap
                    level_set[i] = level_set[random_index];
                    level_set[random_index] = temp_level;
                }
            }
        }
    }
}