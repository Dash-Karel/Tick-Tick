using Engine;
using Microsoft.Xna.Framework;

class Cloud : SpriteGameObject
{
    Level level;
    float parallaxMultiplier;

    public Cloud(Level level) : base(null, TickTick.Depth_Background)
    {
        this.level = level;
        Reset();
    }

    public override void Reset()
    {
        Randomize();

        // give the cloud a starting position inside the level
        localPosition.X = ExtendedGame.Random.Next(-sprite.Width, (int)(TickTick.Camera.Size.X + (level.BoundingBox.Width - TickTick.Camera.Size.X) * parallaxMultiplier));
    }

    void Randomize()
    {
        // set a random sprite and depth
        float depth = TickTick.ParallaxEndDepth / 2 - TickTick.ParallaxEndDepth / 2 * ExtendedGame.Random.NextSingle();
        sprite = new SpriteSheet("Sprites/Backgrounds/spr_cloud_" + ExtendedGame.Random.Next(1, 6), depth);

        // set a random y-coordinate and speed
        parallaxMultiplier = depth / ExtendedGame.ParallaxEndDepth;
        float y = ExtendedGame.Random.Next((int)(100 * parallaxMultiplier), (int)(TickTick.Camera.Size.Y + (BoundingBox.Height - TickTick.Camera.Size.Y) * parallaxMultiplier)) - Height;
        float speed = ExtendedGame.Random.Next(10, 50) * parallaxMultiplier;

        if (ExtendedGame.Random.Next(2) == 0)
        {
            // go from right to left
            localPosition = new Vector2(TickTick.Camera.Size.X + (level.BoundingBox.Width - TickTick.Camera.Size.X) * parallaxMultiplier, y);
            velocity = new Vector2(-speed, 0);
        }
        else
        {
            // go from left to right
            localPosition = new Vector2(-sprite.Width, y);
            velocity = new Vector2(speed, 0);
        }
    }

    public override void Update(GameTime gameTime)
    {
        base.Update(gameTime);

        if (velocity.X < 0 && localPosition.X < -sprite.Width)
            Randomize();
        else if (velocity.X > 0 && localPosition.X > TickTick.Camera.Size.X + (level.BoundingBox.Width - TickTick.Camera.Size.X) * parallaxMultiplier)
            Randomize();
    }
}
