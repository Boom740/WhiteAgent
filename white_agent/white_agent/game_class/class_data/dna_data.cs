using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace old_heart
{
    public class dna_data
    {
        public int amount { get; private set; } = 0;
        public int total_collected { get; private set; } = 0; // ยอดที่เก็บได้ทั้งหมด (ไว้ทำสถิติ)

        public event Action<int> on_changed; // UI subscribe ได้ (ต้อง unsubscribe ตอน unload scene) หรือจะอ่าน amount ทุกเฟรมแทนก็ได้

        public void add(int value)
        {
            if (value <= 0) return;
            amount += value;
            total_collected += value;
            on_changed?.Invoke(amount);
        }

        public bool can_afford(int cost)
        {
            return amount >= cost;
        }

        public bool try_spend(int cost)
        {
            if (cost <= 0 || amount < cost) return false;
            amount -= cost;
            on_changed?.Invoke(amount);
            return true;
        }

        public void reset()
        {
            amount = 0;
            total_collected = 0;
            on_changed?.Invoke(amount);
        }
    }
}