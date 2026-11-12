using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;

namespace old_heart
{
    public class gamepad_input
    {
        GamePadState current_state;
        GamePadState previous_state;
        public bool is_connected;

        // --- left stick: movement ---
        public float move_deadzone = 0.2f;
        public Vector2 left_stick;                  // screen space (Y ลง = บวก), หลัง deadzone, ความยาว 0..1

        // --- right stick: aim / throw head ---
        public float aim_enter_deadzone = 0.5f;     // เกินนี้ = เริ่มเล็ง
        public float aim_exit_deadzone = 0.2f;      // ต่ำกว่านี้ = ปล่อยสติ๊ก (ขว้าง)
        public float aim_cache_threshold = 0.5f;    // เก็บทิศลง cache เฉพาะตอนสติ๊กดันแรงเกินนี้
        public int aim_reverse_confirm_frames = 3;  // ทิศกลับด้าน >90° ต้องค้างกี่เฟรมถึงจะยอมรับ (กัน snapback)

        public bool aim_active;                     // กำลังเล็งอยู่ (hysteresis 0.5 / 0.2)
        public Vector2 last_valid_aim_direction;    // ทิศ normalized ล่าสุดที่เชื่อถือได้

        bool aim_suppressed;                        // ยกเลิกเล็งไปแล้ว ต้องปล่อยสติ๊กก่อนถึงจะเล็งใหม่ได้
        int reverse_frames;

        bool has_state;
        public float right_stick_magnitude;
        public bool has_activity;   // มีการขยับสติ๊ก/กดปุ่มเฟรมนี้

        public gamepad_input()
        {
            current_state = GamePad.GetState(PlayerIndex.One, GamePadDeadZone.None);
        }

        public void update()
        {
            GamePadState state = GamePad.GetState(PlayerIndex.One, GamePadDeadZone.None);
            previous_state = has_state ? current_state : state;   // เฟรมแรกไม่ให้ปุ่มที่ค้างอยู่นับเป็น "เพิ่งกด"
            current_state = state;
            has_state = true;
            is_connected = state.IsConnected;

            if (!is_connected)
            {
                left_stick = Vector2.Zero;
                right_stick_magnitude = 0f;
                aim_active = false;
                aim_suppressed = false;
                has_activity = false;
                return;
            }

            update_left_stick();
            update_right_stick();

            has_activity = left_stick != Vector2.Zero
                        || right_stick_magnitude >= aim_exit_deadzone
                        || current_state.Buttons != previous_state.Buttons;
        }

        public bool is_down(Buttons button) => is_connected && current_state.IsButtonDown(button);
        public bool was_pressed(Buttons button) =>
            is_connected && current_state.IsButtonDown(button) && previous_state.IsButtonUp(button);

        // ยกเลิกการเล็ง (ไม่ขว้าง) และบล็อกการเล็งใหม่จนกว่าจะปล่อยสติ๊ก
        public void cancel_aim_until_release()
        {
            aim_suppressed = true;
            aim_active = false;
        }

        void update_left_stick()
        {
            // ThumbSticks.Y ของ XNA/MonoGame "ขึ้น = บวก" ต้องกลับให้ตรงกับหน้าจอ
            Vector2 raw = new Vector2(current_state.ThumbSticks.Left.X, -current_state.ThumbSticks.Left.Y);
            float magnitude = raw.Length();
            if (magnitude < move_deadzone) { left_stick = Vector2.Zero; return; }

            float scaled = Math.Min(1f, (magnitude - move_deadzone) / (1f - move_deadzone));
            left_stick = raw / magnitude * scaled;
        }

        void update_right_stick()
        {
            Vector2 raw = new Vector2(current_state.ThumbSticks.Right.X, -current_state.ThumbSticks.Right.Y);
            float magnitude = raw.Length();

            right_stick_magnitude = magnitude;

            // ถูก cancel ไปแล้ว: เงียบไว้จนกว่าสติ๊กจะกลับมาที่กลาง
            if (aim_suppressed)
            {
                aim_active = false;
                if (magnitude < aim_exit_deadzone) aim_suppressed = false;
                return;
            }

            // ยังไม่เล็ง: ดันเกิน 0.5 = เข้าสู่การเล็ง และ cache ทิศทันที
            if (!aim_active)
            {
                if (magnitude >= aim_enter_deadzone)
                {
                    aim_active = true;
                    last_valid_aim_direction = raw / magnitude;
                    reverse_frames = 0;
                }
                return;
            }

            // กำลังเล็ง: ต่ำกว่า 0.2 = ปล่อยแล้ว (last_valid_aim_direction ยังค้างทิศเดิมไว้ให้ throw)
            if (magnitude < aim_exit_deadzone)
            {
                aim_active = false;
                return;
            }

            // อัปเดต cache เฉพาะตอนสติ๊กแรงพอ
            if (magnitude >= aim_cache_threshold)
            {
                Vector2 direction = raw / magnitude;

                if (Vector2.Dot(direction, last_valid_aim_direction) >= 0f)
                {
                    last_valid_aim_direction = direction;   // ทิศใกล้เคียงเดิม = ปกติ
                    reverse_frames = 0;
                }
                else if (++reverse_frames >= aim_reverse_confirm_frames)
                {
                    last_valid_aim_direction = direction;   // กลับด้านค้างหลายเฟรม = ตั้งใจสะบัดจริง
                    reverse_frames = 0;
                }
                // กลับด้านแค่ 1-2 เฟรม = snapback ทิ้งไป
            }
            else
            {
                reverse_frames = 0;
            }
        }
    }
}