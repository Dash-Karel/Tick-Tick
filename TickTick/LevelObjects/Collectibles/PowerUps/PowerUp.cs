using Engine;
using Microsoft.Xna.Framework;

internal abstract class PowerUp : Collectible
{
    protected float powerUpDuration;
    float timer;
    protected bool isApllied;
    public PowerUp(Level level, Vector2 startPosition, string spriteName) : base(level, startPosition, spriteName) { }

    public override void Update(GameTime gameTime)
    {
        base.Update(gameTime);

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

    protected override void Collect()
    {
        ApplyPowerup();
        isApllied = true;
        if (timer <= 0)
            timer = powerUpDuration;
    }

    protected abstract void ApplyPowerup();


    protected abstract void RemovePowerUp();
    

    public override void Reset()
    {
        base.Reset();
        if (isApllied)
            RemovePowerUp();

        isApllied = false;
        timer = 0;
    }
}
