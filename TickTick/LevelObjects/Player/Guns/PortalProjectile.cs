using Microsoft.Xna.Framework;
internal class PortalProjectile : Projectile
{
    PortalGun gun;
    public PortalProjectile(Vector2 startPosition, Level level, PortalGun portalGun) : base(startPosition, "Sprites/LevelObjects/Portal", level)
    {
        travelSpeed = 1100;
        useGravity = true;
        gun = portalGun;
        //set origin to bottom
        Origin = new Vector2(Origin.X, Height);
    }
    protected override void CollideWithTile()
    {
        base.CollideWithTile();
        Portal portal = new Portal(level, GlobalPosition - new Vector2(0, Height / 2));
        gun.AddPortal(portal);
        if(gun.FirstPortal == null)
            gun.FirstPortal = portal;
        else
        {
            gun.FirstPortal.LinkedPortal = portal;
            portal.LinkedPortal = gun.FirstPortal;
        }
    }
    public override void CollideWithEnemy(Enemy enemy)
    {
        CollideWithTile();
    }
}
