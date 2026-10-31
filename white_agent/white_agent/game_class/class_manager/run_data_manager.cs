using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace old_heart
{
    public class run_data_manager
    {
        private Random random = new Random();

        public int max_hp = 1;
        public int hp_left = 1;
        public int respawn_left = 3;

        public List<string> level_list = new List<string>();
        public int cleared_level = 0;

        public List<upgrade_base> upgrade_list = new List<upgrade_base> {};

        public List<upgrade_base> shop_upgrade_pool_list = new List<upgrade_base> {
            new upgrade_ricochet_head(),
            new upgrade_explosive_impact(),
            new upgrade_double_dash(),
            new upgrade_lethal_dash(),
            new upgrade_projectile_deflection(),
            new upgrade_adrenaline_rush(),
            new upgrade_emergency_knockback(),
            new upgrade_magenet()
        };

        public List<upgrade_base> shop_upgrade_list = new List<upgrade_base> {}; // upgrade that sell in shop

        public int dna = 0;
        public int dna_temporary = 0;   // will save on dna only when clear level
        public run_data_manager()
        {
            load();
        }
        public void load()
        {
            List<string> level_set_1 = new List<string>() { "level_1_easy" , "level_2_easy" , "level_3_easy" , "level_4_easy", "level_5_easy", "level_6_easy" }; 
            List<string> level_set_2 = new List<string>() { "level_1_mid", "level_2_mid", "level_3_mid", "level_4_mid", "level_5_mid", "level_6_mid" }; ;
            List<string> level_set_3 = new List<string>() { "level_1_hard", "level_2_hard", "level_3_hard", "level_4_hard", "level_5_hard", "level_6_hard" }; ;

            random_list_order(level_set_1);
            random_list_order(level_set_2);
            random_list_order(level_set_3);

            level_list.Add("level_0_tutorial");
            level_list_add_from_list(level_set_1.GetRange(0, 3));
            level_list.Add("level_0_shop");
            level_list_add_from_list(level_set_2.GetRange(0, 3));
            level_list.Add("level_0_shop");
            level_list_add_from_list(level_set_3.GetRange(0, 3));
            level_list.Add("level_0_shop");
            level_list.Add("level_boss_cutscene");
            level_list.Add("level_boss");


            foreach (string level in level_list)
            {
                Debug.WriteLine("level_list " +  level);
            }  // debug

            void level_list_add_from_list(List<string> level_add_list)
            {
                foreach (string level in level_add_list)
                {
                    level_list.Add(level);
                }
            }

            random_shop_item();
        }
        void random_list_order<T>(List<T> list)
        {
            int i = list.Count;
            while (i > 0)
            {
                i--;
                int random_index = random.Next(i + 1);

                T temp_level = list[i];       // swap
                list[i] = list[random_index];
                list[random_index] = temp_level;
            }
        }
        public void random_shop_item()
        {
            shop_upgrade_list.Clear(); 
            List<string> owned_upgrade_name_list = upgrade_list.Select(upgrade => upgrade.GetType().Name).ToList();
            List<upgrade_base> not_owned_upgrade_list = shop_upgrade_pool_list.Where(poolItem => !owned_upgrade_name_list.Contains(poolItem.GetType().Name)).ToList();
            random_list_order(not_owned_upgrade_list);

            for (int i = 0; i < (Math.Min(3 , not_owned_upgrade_list.Count)); i++)
            {
                upgrade_base upgrade = not_owned_upgrade_list[i];
                shop_upgrade_list.Add(upgrade);

                //Debug.WriteLine("random shop item [" + i + "] : " + upgrade);
            }

        }
        public void apply_upgrade(player player)
        {
            foreach (upgrade_base upgrade in upgrade_list)
            {
                upgrade.apply_upgrade(player);
            }
        }
    }
}