using System;
using Microsoft.Xna.Framework;

internal class GunCollectible : Collectible
{
    /// <summary>
    /// The type of the object that gets instantiated on Collect()
    /// </summary>
    Type type;
    public GunCollectible(Level level, Vector2 startPosition, string spriteName, Type gunType) : base(level, startPosition, spriteName)
    {
        if (gunType.BaseType == typeof(Gun))
        {
            type = gunType;
        }
        else
        {
            throw new WrongTypeException();
        }
    }
    protected override void Collect()
    {
        object[] parameters = new object[] { new Vector2(0, -level.Player.Height / 2), level };
        Gun gun = Activator.CreateInstance(type, parameters) as Gun;
        gun.Parent = level.Player;
        level.Player.Gun = gun;

        TickTick.AssetManager.PlaySoundEffect(gun.reloadSoundEffectName);
    }

    private class WrongTypeException : Exception
    {
        public WrongTypeException() : base("The provided type was not of type Gun") { }
    }
}
