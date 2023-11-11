using System;
using Microsoft.Xna.Framework;
using Engine;

abstract internal class Collectible : SpriteGameObject
{
    protected Level level;
    protected float bounce;
    Vector2 startPosition;

    public Collectible(Level level, Vector2 startPosition, string spriteName) : base(spriteName, TickTick.Depth_LevelObjects)
    {
        this.level = level;
        this.startPosition = startPosition;

        SetOriginToCenter();

        Reset();
    }

    public override void Update(GameTime gameTime)
    {
        base.Update(gameTime);

        double t = gameTime.TotalGameTime.TotalSeconds * 3.0f + LocalPosition.X;
        bounce = (float)Math.Sin(t) * 0.2f;
        localPosition.Y += bounce;

        // check if the player collects this collectible
        if (Visible && level.Player.CanCollideWithObjects && HasPixelPreciseCollision(level.Player))
        {
            Visible = false;
            Collect();
        }
    }

    public override void Reset()
    {
        localPosition = startPosition;
        Visible = true;
    }

    /// <summary>
    /// The method that gets called when the player collects the collectable
    /// </summary>
    protected abstract void Collect();
}
