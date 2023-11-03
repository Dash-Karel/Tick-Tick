using Microsoft.Xna.Framework;

internal class SpeedUpPowerUp : PowerUp
{
    public SpeedUpPowerUp(Level level, Vector2 startposition) : base(level, "Sprites/LevelObjects/PowerUps/speedUp", startposition)
    {
        powerUpDuration = 5;
    }

    protected override void ApplyPowerup()
    {
        level.Player.walkingSpeed = level.Player.walkingSpeed * 1.5f;
    }

    protected override void RemovePowerUp()
    {
        level.Player.walkingSpeed = level.Player.walkingSpeed / 1.5f;
    }
}

