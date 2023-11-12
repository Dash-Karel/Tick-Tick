using Engine;
using Microsoft.Xna.Framework;
internal class Portal : Collectible
{
    /// <summary>
    /// The amount of time that needs to pass before the portal works again after being used
    /// </summary>
    const float waitDuration = 2f;
    public float WaitTimeLeft { private get; set; }

    /// <summary>
    /// The portal that is linked with this portal and teleportation between is possible
    /// </summary>
    public Portal LinkedPortal { private get; set; }
    public Portal(Level level, Vector2 startPosition) : base(level, startPosition, "Sprites/LevelObjects/Portal")
    { 
    }

    public override void Update(GameTime gameTime)
    {
        base.Update(gameTime);

        //Count down the timer
        WaitTimeLeft -= (float)gameTime.ElapsedGameTime.TotalSeconds;
    }

    protected override void Collect()
    {
        if (WaitTimeLeft <= 0 && LinkedPortal != null)
        {
            //Teleport the player to the position of the linked portal
            level.Player.LocalPosition = LinkedPortal.GlobalPosition + level.Player.Origin - level.Player.BoundingBox.Size.ToVector2() * 0.6f;

            //Make sure both portals can't be used for a while to prevent being stuck infinitely teleporting
            WaitTimeLeft = waitDuration;
            LinkedPortal.WaitTimeLeft = waitDuration;

            TickTick.AssetManager.PlaySoundEffect("Sounds/snd_teleport");
        }

        //Override the dissappearing behaviour of Collectible
        Visible = true;
    }
}
 
