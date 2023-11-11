using Engine;
using Microsoft.Xna.Framework;

internal class BurstRifle : Gun
{
    int burstTokens;

    public BurstRifle(Vector2 basePosition, Level level) : base("Sprites/LevelObjects/Player/Guns/BurstRifle", basePosition, level)
    {
        magazineSize = 21;
        fireRate = 9;
        recoilForce = 25f;
        barrelOffset = new Vector2(Width, Height * 0.2f);
        projectileType = ProjectileType.bullet;
        shootingSoundEffectName = "Sounds/snd_big_gun";

        Reset();
    }
    public override void HandleInput(InputHelper inputHelper)
    {
        PointToMouse(inputHelper);

        if(inputHelper.MouseLeftButtonPressed() && burstTokens < 1) 
        {
            burstTokens += 3;
        }

        if (fireDelayTime <= 0 && burstTokens > 0)
        {
            burstTokens--;
            fireDelayTime = 1 / fireRate;
            Shoot(inputHelper);
        }
    }
}
