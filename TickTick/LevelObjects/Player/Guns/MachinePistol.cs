using Microsoft.Xna.Framework;
class MachinePistol : Gun
{
    public MachinePistol(Vector2 basePosition, Level level) : base("Sprites/LevelObjects/Player/Guns/MachinePistol", basePosition, level)
    {
        fireRate = 5;
        recoilForce = 15f;
        fullAuto = true;
        barrelOffset = new Vector2(Width, Height * 0.15f);
        projectileType = ProjectileType.bullet;
        shootingSoundEffectName = "Sounds/snd_machine_pistol";
    }
}
