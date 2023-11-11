using Microsoft.Xna.Framework;
using Engine;

class WaterDrop : Collectible
{

    public WaterDrop(Level level, Vector2 startPosition) : base(level, startPosition, "Sprites/LevelObjects/spr_water") { }

    protected override void Collect()
    {
        ExtendedGame.AssetManager.PlaySoundEffect("Sounds/snd_watercollected");
    }
}