using Engine;
using Microsoft.Xna.Framework;
using System.Diagnostics;

internal abstract class PowerUp : SpriteGameObject
{
    protected Level level;
    Vector2 startPosition;
    protected float powerUpDuration;
    float timer;
    protected bool isApllied;
    public PowerUp(Level level, string spritename, Vector2 startposition)
        : base(spritename, TickTick.Depth_LevelObjects)
    {
        this.level = level;
        this.startPosition = startposition;
        isApllied = false;

        SetOriginToCenter();

        Reset();
    }
    public override void Update(GameTime gameTime)
    {
        base.Update(gameTime);

        if (Visible && level.Player.CanCollideWithObjects && HasPixelPreciseCollision(level.Player))
        {
            ApplyPowerup();
        }
        if (timer > 0)
        {
            timer -= (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (timer <= 0) 
                RemovePowerUp();
        }
    }
    protected virtual void ApplyPowerup()
    {
        Visible = false;
        isApllied = true;
        Debug.WriteLine("speeeeeeeeeeed");
        if (timer <= 0)
            timer = powerUpDuration;
    }

    protected virtual void RemovePowerUp()
    {
        isApllied = false;
    }

    public override void Reset()
    {
        base.Reset();
        if (isApllied)
            RemovePowerUp();

        localPosition = startPosition;
        Visible = true;
    }
}
