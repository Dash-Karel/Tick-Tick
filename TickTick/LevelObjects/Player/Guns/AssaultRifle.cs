using Microsoft.Xna.Framework;

internal class AssaultRifle : Gun
{
    public AssaultRifle(Vector2 basePosition, Level level) : base("Sprites/LevelObjects/Player/Guns/AssaultRifle", basePosition, level)
    {
        magazineSize = 30;
        fireRate = 9;
        recoilForce = 22f;
        fullAuto = true;
        barrelOffset = new Vector2(Width, Height * 0.15f);
        projectileType = ProjectileType.bullet;
        shootingSoundEffectName = "Sounds/snd_assault_rifle";

        Reset();
    }
}
