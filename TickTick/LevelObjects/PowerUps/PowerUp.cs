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
        

        SetOriginToCenter();

        Reset();
    }
    public override void Update(GameTime gameTime)
    {
        base.Update(gameTime);

        if (Visible && level.Player.CanCollideWithObjects && HasPixelPreciseCollision(level.Player))
        {
            ApplyPowerup();
            Visible = false;
            isApllied = true;
            if (timer <= 0)
                timer = powerUpDuration;
        }
        if (timer > 0)
        {
            timer -= (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (timer <= 0)
            {
                RemovePowerUp();
                isApllied = false;
            }
                
        }
    }
    protected abstract void ApplyPowerup();


    protected abstract void RemovePowerUp();
    

    public override void Reset()
    {
        base.Reset();
        if (isApllied)
            RemovePowerUp();

        localPosition = startPosition;
        Visible = true;
        isApllied = false;
    }
}
