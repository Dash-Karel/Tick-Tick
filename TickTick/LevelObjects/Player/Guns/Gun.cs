using System;
using System.Collections.Generic;
using Engine;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

abstract class Gun : SpriteGameObject
{
    protected enum ProjectileType { bullet, portal }

    //make mirror accessible in order to let player face the right direction
    public bool Mirror { get { return sprite.Mirror; } }

    /// <summary>
    /// A list that contains all projectiles that have been shot and are not yet destroyed
    /// </summary>
    public List<Projectile> ActiveProjectiles { get; private set; }

    //a reference to the level to pass to the projectiles
    Level level;

    //A special kind of text game object to display the amount of ammo left
    AmmoDisplay ammoDisplay;

    /// <summary>
    /// What kind of projectile the weapon will shoot
    /// </summary>
    protected ProjectileType projectileType;

    /// <summary>
    /// The position relative to the gun where the projectile will be instantiated
    /// </summary>
    protected Vector2 barrelOffset;

    /// <summary>
    /// The amount of bullets the gun can fire until it is empty
    /// </summary>
    protected int magazineSize = 10;

    /// <summary>
    /// fire rate in rounds per second
    /// </summary>
    protected float fireRate = 1f;

    /// <summary>
    /// How far the gun moves back when fired
    /// </summary>
    protected float recoilForce = 10f;

    /// <summary>
    /// Whether you can hold the mouse button to auto fire
    /// </summary>
    protected bool fullAuto = false;

    protected string shootingSoundEffectName = "Sounds/snd_default_shooting";

    protected string magazineEmptySoundEffectName = "Sounds/snd_default_empty";

    /// <summary>
    /// The time in seconds until the next shot can be fired
    /// </summary>
    protected float fireDelayTime;

    Vector2 basePosition;

    /// <summary>
    /// The rotation of the gun in radians
    /// </summary>
    float rotation;

    /// <summary>
    /// The amount of bullets left in the magazine
    /// </summary>
    int bulletsLeft;

    const float recoilResetSpeed = 5f;

    public Gun(string spriteName, Vector2 basePosition, Level level) : base(spriteName, TickTick.Depth_LevelPlayer + 0.01f)
    {
        this.basePosition = basePosition;
        this.level = level;
        Reset();
    }

    public override void HandleInput(InputHelper inputHelper)
    {
        PointToMouse(inputHelper);
        if (fireDelayTime <= 0 && (inputHelper.MouseLeftButtonPressed() || inputHelper.MouseLeftButtonDown() && fullAuto))
        {
            fireDelayTime = 1 / fireRate;
            Shoot(inputHelper);
        }
    }

    public override void Update(GameTime gameTime)
    {
        fireDelayTime -= (float)gameTime.ElapsedGameTime.TotalSeconds;

        for(int i = 0; i < ActiveProjectiles.Count; i++)
        {
            ActiveProjectiles[i].Update(gameTime);
            if (ActiveProjectiles[i].MarkedForRemoval)
                ActiveProjectiles.RemoveAt(i--);
        }

        //make sure gun moves back after recoil
        velocity = (basePosition - localPosition) * recoilResetSpeed;

        //update the display to show the ammo left
        ammoDisplay.SyncDisplay(bulletsLeft);

        base.Update(gameTime);
    }

    //override Draw to take rotation into account
    public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
    {
        ammoDisplay.Draw(gameTime, spriteBatch);

        if (!Visible)
            return;

        // draw the sprite at its *global* position in the game world
        if (sprite != null)
            sprite.Draw(spriteBatch, GlobalPosition, Origin, rotation);

        //draw the bullets
        for (int i = 0; i < ActiveProjectiles.Count; i++)
        {
            ActiveProjectiles[i].Draw(gameTime, spriteBatch);
        }
    }

    public override void Reset()
    {
        ammoDisplay = new AmmoDisplay(magazineSize);
        ActiveProjectiles = new List<Projectile>();
        bulletsLeft = magazineSize;
    }

    protected void PointToMouse(InputHelper inputHelper)
    {
        double opposite = inputHelper.MousePositionCameraView.Y - (GlobalPosition.Y + Origin.Y);
        double adjacent = inputHelper.MousePositionCameraView.X - GlobalPosition.X;
        float actualRotation = (float)Math.Atan2(opposite, adjacent);
        HandleSpriteOrientation(actualRotation);
    }

    void HandleSpriteOrientation(float actualRotation)
    {
        if(actualRotation > MathF.PI / 2|| actualRotation < -MathF.PI / 2)
        {
            rotation = MathF.PI + actualRotation;
            sprite.Mirror = true;
            SetOriginRight();
        }
        else
        {
            rotation = actualRotation;
            sprite.Mirror = false;
            SetOriginLeft();        
        }
    }

    protected void Shoot(InputHelper inputHelper)
    {
        if (bulletsLeft > 0)
        {
            bulletsLeft--;

            TickTick.AssetManager.PlaySoundEffect(shootingSoundEffectName);

            Vector2 projectileDirection = inputHelper.MousePositionCameraView - (GlobalPosition + new Vector2(0, barrelOffset.Y));
            projectileDirection.Normalize();
            Vector2 projectilePosition = GlobalPosition + new Vector2(0, barrelOffset.Y) + projectileDirection * Width;
            Projectile projectile = NewProjectile(projectileType, projectilePosition);
            projectile.StartFlying(projectileDirection);
            ActiveProjectiles.Add(projectile);

            //apply recoil
            localPosition -= projectileDirection * recoilForce;
        }
        else if(inputHelper.MouseLeftButtonPressed())
        {
            TickTick.AssetManager.PlaySoundEffect(magazineEmptySoundEffectName);
        }
    }

    Projectile NewProjectile(ProjectileType type, Vector2 startPosition)
    {
        switch (projectileType)
        {
            case ProjectileType.bullet:
                return new Bullet(startPosition, level);
            default:
                return null;
        }
    }

    void SetOriginLeft()
    {
        Origin = new Vector2(0, barrelOffset.Y);
    }
    void SetOriginRight()
    {
        Origin = new Vector2(Width, barrelOffset.Y);
    }
}
