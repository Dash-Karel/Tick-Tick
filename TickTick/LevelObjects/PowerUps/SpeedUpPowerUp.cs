using Microsoft.Xna.Framework;

internal class SpeedUpPowerUp : PowerUp
{
    public SpeedUpPowerUp(Level level, Vector2 startposition) : base(level, "Sprites/LevelObjects/PowerUps/speedUp", startposition)
    {
        powerUpDuration = 5;
    }

    protected override void ApplyPowerup()
    {
        base.ApplyPowerup();
        level.Player.walkingSpeed = level.Player.walkingSpeed * 1.5f;
    }

    protected override void RemovePowerUp()
    {
        base.RemovePowerUp();
        level.Player.walkingSpeed = level.Player.walkingSpeed / 1.5f;
    }
}

