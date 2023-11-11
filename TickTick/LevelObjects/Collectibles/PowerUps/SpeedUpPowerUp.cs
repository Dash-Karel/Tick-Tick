using Microsoft.Xna.Framework;

internal class SpeedUpPowerUp : PowerUp
{
    public SpeedUpPowerUp(Level level, Vector2 startPosition) : base(level, startPosition, "Sprites/LevelObjects/PowerUps/speedUp")
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

