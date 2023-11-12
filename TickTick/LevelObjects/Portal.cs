using Engine;
using Microsoft.Xna.Framework;
internal class Portal : Collectible
{
    const float waitDuration = 2f;
    public float WaitTimeLeft { private get; set; }
    public Portal LinkedPortal { private get; set; }
    public Portal(Level level, Vector2 startPosition) : base(level, startPosition, "Sprites/LevelObjects/Portal")
    { 
    }

    public override void Update(GameTime gameTime)
    {
        base.Update(gameTime);
        WaitTimeLeft -= (float)gameTime.ElapsedGameTime.TotalSeconds;
    }

    protected override void Collect()
    {
        if (WaitTimeLeft <= 0 && LinkedPortal != null)
        {
            level.Player.LocalPosition = LinkedPortal.GlobalPosition + level.Player.Origin - level.Player.BoundingBox.Size.ToVector2() * 0.6f;
            WaitTimeLeft = waitDuration;
            LinkedPortal.WaitTimeLeft = waitDuration;
            TickTick.AssetManager.PlaySoundEffect("Sounds/snd_teleport");
        }
        Visible = true;
    }
}
 
