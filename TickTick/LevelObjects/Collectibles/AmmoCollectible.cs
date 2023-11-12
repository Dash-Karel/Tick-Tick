using System.ComponentModel.Design;
using Microsoft.Xna.Framework;

internal class AmmoCollectible : Collectible
{
    public AmmoCollectible(Level level, Vector2 startPosition) : base(level, startPosition, "Sprites/LevelObjects/Player/Guns/Ammo")
    {
    }

    protected override void Collect()
    {
        //Reload the players gun if it exists and its magizine is not full
        Gun gun = level.Player.Gun;
        if (gun != null && !gun.MagazineIsFull)
            gun.Reload();
        else
            //if the ammo was not picked up it should not disappear
            Visible = true;
    }

}

