using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Collisions;
using System;

namespace old_heart;

public abstract class item_base : projectile
{
    public float friction = 6f;

    // --- pickup ---
   // public float pickup_radius = 3f;        // เก็บได้เมื่อห่าง player น้อยกว่า pickup_radius + hit_box_radius ของ player
    public float magnet_radius = 30f;       // เข้าใกล้กว่านี้แล้วไอเทมเริ่มลอยเข้าหา player
    public float magnet_acceleration = 1800f;
    public float magnet_max_speed = 700f;
    public float pickup_delay = 0.5f;        // กันเก็บทันทีตอนเพิ่งเด้งออกมา
    private float age = 0f;

    // --- lifetime ---
    public float blink_time = 3f;            // กระพริบช่วงท้ายก่อนหาย
    public float blink_interval = 0.1f;

    // --- visual ---
    public float float_height = 8f;          // ลอยเหนือพื้นกี่ px
    public float bob_height = 2f;
    public float bob_speed = 4f;
    public float bob_offset = 0f;

    Random random = new Random();

    protected item_base(ContentManager content_set, Vector2 position , float hit_box_radius = 7)
        : base(content_set, position, time_left: 30f, owner: null, hit_box_radius: hit_box_radius)
    {
        bob_offset = random.Next(-10,10)/10f;
    }

    public override void Update(GameTime gameTime)
    {
        float delta_time = (float)gameTime.ElapsedGameTime.TotalSeconds;

        age += delta_time;

        if (velocity != Vector2.Zero)
        {
            velocity -= velocity * friction * delta_time;
            if (velocity.LengthSquared() < 4f)
            {
                velocity = Vector2.Zero;
            }
        }

        base.Update(gameTime);
    }

    public override void collide_wall(CollisionPair2D pair, float delta_time)
    {
        velocity += pair.FirstResult.MinimumTranslationVector / delta_time;
    }

    public override void Draw(SpriteBatch sprite_batch)
    {
        float alpha = 1f;
        if (time_left <= blink_time) 
        {
            bool visible_phase = (int)(time_left / blink_interval) % 2 == 0;
            alpha = visible_phase ? 1f : 0.3f;
        }

        float bob = MathF.Sin(age * bob_speed + bob_offset) * bob_height;
        sprite_origin = new Vector2(texture.Width / 2f, texture.Height) + new Vector2(0, float_height - bob);

        base.Draw(sprite_batch,alpha);
    }
}