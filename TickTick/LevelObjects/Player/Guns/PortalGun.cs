using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

internal class PortalGun : Gun
{
    List<Portal> portals = new List<Portal>();
    public Portal FirstPortal { get; set; }
    public PortalGun(Vector2 basePosition, Level level) : base("Sprites/LevelObjects/Player/Guns/PortalGun", basePosition, level)
    {
        magazineSize = 2;
        fireRate = 2;
        recoilForce = 80f;
        fullAuto = false;
        barrelOffset = new Vector2(Width, Height * 0.15f);
        projectileType = ProjectileType.portal;
        shootingSoundEffectName = "Sounds/snd_portal_gun";

        Reset();
    }
    public override void Update(GameTime gameTime)
    {
        base.Update(gameTime);
        foreach (Portal portal in portals)
            portal.Update(gameTime);
    }
    public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
    {
        base.Draw(gameTime, spriteBatch);
        foreach (Portal portal in portals)
            portal.Draw(gameTime, spriteBatch);
    }
    public override void Reload()
    {
        base.Reload();
        FirstPortal = null;
        portals.Clear();
    }
    public void AddPortal(Portal portal)
    {
        portals.Add(portal);
    }
}
