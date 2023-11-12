using Microsoft.Xna.Framework;

internal class Pistol : Gun
{
    public Pistol(Vector2 basePosition, Level level) : base("Sprites/LevelObjects/Player/Guns/Pistol", basePosition, level)
    {
        magazineSize = 12;
        fireRate = 10;
        recoilForce = 20f;
        fullAuto = false;
        barrelOffset = new Vector2(Width, Height * 0.15f);
        projectileType = ProjectileType.bullet;
        shootingSoundEffectName = "Sounds/snd_machine_pistol";

        Reset();
    }
}

