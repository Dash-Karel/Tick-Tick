using Engine;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

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

    public void SyncDisplay(int bulletsLeft)
    {
        label.Text = bulletsLeft + "/" + magSize;
    }

}
