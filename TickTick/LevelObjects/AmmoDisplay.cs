using Engine;
using Microsoft.Xna.Framework;

internal class AmmoDisplay : GameObjectList
{
    TextGameObject label;
    int magSize;

    public AmmoDisplay(int magazineSize)
    {
        magSize = magazineSize;

        localPosition = TickTick.Camera.Size.ToVector2() - new Vector2(10, 10);

        SpriteGameObject ammoIcon = new SpriteGameObject("Sprites/UI/spr_ammo", TickTick.Depth_UIBackground);
        ammoIcon.LocalPosition = new Vector2(-ammoIcon.Width, -ammoIcon.Height);
        AddChild(ammoIcon);

        label = new TextGameObject("Fonts/MainFont", TickTick.Depth_UIForeground, Color.LimeGreen, TextGameObject.Alignment.Center);
        label.LocalPosition = new Vector2(-ammoIcon.Width / 4, -ammoIcon.Height * 0.7f);
        AddChild(label);
    }

    /// <summary>
    /// Updates the text
    /// </summary>
    /// <param name="bulletsLeft">The number that gets shown to the left side of the slash</param>
    public void SyncDisplay(int bulletsLeft)
    {
        label.Text = bulletsLeft + "/" + magSize;
    }

}
