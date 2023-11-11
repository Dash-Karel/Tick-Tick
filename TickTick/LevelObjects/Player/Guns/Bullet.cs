using Microsoft.Xna.Framework;

class Bullet : Projectile
{
    public Bullet(Vector2 startPosition, Level level) : base(startPosition, "Sprites/LevelObjects/Player/Guns/bullet", level)
    {
        travelSpeed = 1500;
        useGravity = false;
    }
}
