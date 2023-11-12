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

    //returns whether the magazine is at full capacity
    public bool MagazineIsFull { get { return bulletsLeft == magazineSize; } }

    /// <summary>
    /// A list that contains all projectiles that have been shot and are not yet destroyed
    /// </summary>
    public List<Projectile> ActiveProjectiles { get; private set; }

    /// <summary>
    /// a reference to the level to pass to the projectiles
    /// </summary>
    Level level;

    /// <summary>
    /// A special kind of text game object to display the amount of ammo left
    /// </summary>
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
    /// The duration of the reload in seconds
    /// </summary>
    protected float reloadDuration = 1.5f;

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

    /// <summary>
    /// The name of the shooting soundEffect
    /// </summary>
    protected string shootingSoundEffectName = "Sounds/snd_default_shooting";

    /// <summary>
    /// The name of the soundEffect that gets played when a player attempts to shoot when their magazine is empty
    /// </summary>
    protected string magazineEmptySoundEffectName = "Sounds/snd_default_empty";

    /// <summary>
    /// The name of the soundeEffect that gets played when the player reloads
    /// </summary>
    public string reloadSoundEffectName { get; private set; } = "Sounds/snd_default_reload";

    /// <summary>
    /// The time in seconds until the next shot can be fired
    /// </summary>
    protected float fireDelayTime;

    /// <summary>
    /// The position the gun returns to after recoiling
    /// </summary>
    Vector2 basePosition;

    /// <summary>
    /// The rotation of the gun in radians
    /// </summary>
    float rotation;

    /// <summary>
    /// The amount of bullets left in the magazine
    /// </summary>
    int bulletsLeft;

    /// <summary>
    /// Whether the gun is currently reloading
    /// </summary>
    bool reloading;

    /// <summary>
    /// The time in seconds until the reload is finished
    /// </summary>
    float reloadTimeLeft;

    /// <summary>
    /// The speed at which the gun moves back after recoiling
    /// </summary>
    const float recoilResetSpeed = 5f;

    /// <summary>
    /// The constructor of Gun
    /// </summary>
    /// <param name="spriteName">The name of the sprite which is used to display the gun</param>
    /// <param name="basePosition">The position the gun returns to after recoiling</param>t
    /// <param name="level">A reference to the level</param>
    public Gun(string spriteName, Vector2 basePosition, Level level) : base(spriteName, TickTick.Depth_LevelPlayer + 0.01f)
    {
        this.basePosition = basePosition;
        this.level = level;
        Reset();
    }

    /// <summary>
    /// Handles input by pointing the gun towards the mouse and checking to shoot
    /// </summary>
    /// <param name="inputHelper"></param>
    public override void HandleInput(InputHelper inputHelper)
    {
        PointToMouse(inputHelper);
        if (fireDelayTime <= 0 && (inputHelper.MouseLeftButtonPressed() || inputHelper.MouseLeftButtonDown() && fullAuto))
        {
            fireDelayTime = 1 / fireRate;
            Shoot(inputHelper);
        }
    }

    /// <summary>
    /// Contains the code for reloading, recoil, and syncing the ammo dispay
    /// </summary>
    /// <param name="gameTime"></param>
    public override void Update(GameTime gameTime)
    {
        if (reloading)
        {
            float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
            rotation += 2 * MathF.PI / reloadDuration * deltaTime;
            reloadTimeLeft -= deltaTime;
            if(reloadTimeLeft <= 0)
            {
                reloading = false;
            }
        }

        fireDelayTime -= (float)gameTime.ElapsedGameTime.TotalSeconds;

        //Check if any of the projectiles should be removed from the list
        for (int i = 0; i < ActiveProjectiles.Count; i++)
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
        //make sure the ammoDisplay is drawn as well
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

    public virtual void Reload()
    {
        bulletsLeft = magazineSize;
        TickTick.AssetManager.PlaySoundEffect(reloadSoundEffectName);
        reloading = true;
        reloadTimeLeft = reloadDuration;

    }

    /// <summary>
    /// Rotates the barrel towards the mouse pointer
    /// </summary>
    /// <param name="inputHelper"></param>
    protected void PointToMouse(InputHelper inputHelper)
    {
        //When reloading the gun needs to be spinning and the position of the mouse should be disregarded
        if (reloading)
            return;

        double opposite = inputHelper.MousePositionCameraView.Y - (GlobalPosition.Y + Origin.Y);
        double adjacent = inputHelper.MousePositionCameraView.X - GlobalPosition.X;
        float actualRotation = (float)Math.Atan2(opposite, adjacent);
        HandleSpriteOrientation(actualRotation);
    }

    /// <summary>
    /// Makes sure the orientation of the sprite is the right way(not upside down)
    /// </summary>
    /// <param name="actualRotation"></param>
    void HandleSpriteOrientation(float actualRotation)
    {
        //If the gun is rotated beyond directly up the sprite should be mirrored
        if(actualRotation > MathF.PI / 2|| actualRotation < -MathF.PI / 2)
        {
            //Add one PI to the rotation as after mirroring the gun is now turned the other way
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
    /// <summary>
    /// Checks whether the barrel is within a tile, returns true when 'colliding'
    /// </summary>
    /// <param name="inputHelper"></param>
    /// <returns></returns>
    bool BarrelHasTileCollision(InputHelper inputHelper)
    {
        Vector2 barrelDirection = inputHelper.MousePositionCameraView - (GlobalPosition + new Vector2(0, barrelOffset.Y));
        barrelDirection.Normalize();
        //Check five points along the line towards the point the bullet is fired from
        for (int i = 0; i < 5; i++)
        {
            Point tile = level.GetTileCoordinates(GlobalPosition + new Vector2(0, barrelOffset.Y) + barrelDirection * Width / (i + 1));
            Tile.Type tileType = level.GetTileType(tile.X, tile.Y);

            // ignore empty tiles
            if (tileType == Tile.Type.Empty)
                continue;

            return true;
        }
        return false;
    }

    /// <summary>
    /// Shoots a projectile if not reloading and when there are bullets left
    /// </summary>
    /// <param name="inputHelper"></param>
    protected void Shoot(InputHelper inputHelper)
    {
        //Make sure you can't shoot when reloading or when the barrel is within a tile
        if (reloading || BarrelHasTileCollision(inputHelper))
            return;

        //Make sure there are bullets left in the magazine
        if (bulletsLeft > 0)
        {
            bulletsLeft--;

            TickTick.AssetManager.PlaySoundEffect(shootingSoundEffectName);

            //calculate the position the bullet should spawn
            Vector2 projectileDirection = inputHelper.MousePositionCameraView - (GlobalPosition + new Vector2(0, barrelOffset.Y));
            projectileDirection.Normalize();
            Vector2 projectilePosition = GlobalPosition + new Vector2(0, barrelOffset.Y) + projectileDirection * Width;

            //Create and shoot the projectile
            Projectile projectile = NewProjectile(projectileType, projectilePosition);
            projectile.StartFlying(projectileDirection);

            //Add the projectile to the list to make sure it gets updated
            ActiveProjectiles.Add(projectile);

            //apply recoil
            localPosition -= projectileDirection * recoilForce;
        }
        //check for pressed to make sure only one magazine empty click is made(not multiple for burst or multiple when holding button with a full auto weapon)
        else if(inputHelper.MouseLeftButtonPressed())
        {
            TickTick.AssetManager.PlaySoundEffect(magazineEmptySoundEffectName);
        }
    }

    /// <summary>
    /// Returns a new Projectile based on the ProjectileType
    /// </summary>
    /// <param name="type">The ProjectileType that corresponds to the desired Projectile</param>
    /// <param name="startPosition">The position at which the projectile will spawn</param>
    /// <returns></returns>
    /// <exception cref="Exception"></exception>
    Projectile NewProjectile(ProjectileType type, Vector2 startPosition)
    {
        switch (projectileType)
        {
            case ProjectileType.bullet:
                return new Bullet(startPosition, level);
            case ProjectileType.portal:
                return new PortalProjectile(startPosition, level, this as PortalGun);
            default:
                throw new Exception("ProjectileType '" + projectileType + "' is not yet implemented");
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
