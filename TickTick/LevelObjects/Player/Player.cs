using Engine;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;

class Player : AnimatedGameObject
{
    public float walkingSpeed = 400; // Standard walking speed, in game units per second.
    const float backwardsWalkingMultiplier = 0.7f; //The multiplier that gets applied to the walkingSpeed when walking backwards
    const float jumpSpeed = 900; // Lift-off speed when the character jumps.
    const float gravity = 2300; // Strength of the gravity force that pulls the character down.
    const float maxFallSpeed = 1200; // The maximum vertical speed at which the character can fall.
    
    const float iceFriction = 1; // Friction factor that determines how slippery the ice is; closer to 0 means more slippery.
    const float normalFriction = 20; // Friction factor that determines how slippery a normal surface is.
    const float airFriction = 2; // Friction factor that determines how much (horizontal) air resistance there is.

    bool facingLeft; // Whether or not the character is currently looking to the left.

    bool isGrounded; // Whether or not the character is currently standing on something.
    bool standingOnIceTile, standingOnHotTile; // Whether or not the character is standing on an ice tile or a hot tile.
    float desiredHorizontalSpeed; // The horizontal speed at which the character would like to move.
    
    bool isOnMovingObject;
    float objectHorizontalVelocity;
    
    Level level;
    Vector2 startPosition;
    
    bool isCelebrating; // Whether or not the player is celebrating a level victory.
    bool isExploding;

    public Gun Gun { get; set; }

    public bool IsAlive { get; private set; }

    public bool CanCollideWithObjects { get { return IsAlive && !isCelebrating; } }

    public bool IsMoving { get { return velocity != Vector2.Zero; } }

    public Player(Level level, Vector2 startPosition) : base(TickTick.Depth_LevelPlayer)
    {
        this.level = level;
        this.startPosition = startPosition;

        // load all animations
        LoadAnimation("Sprites/LevelObjects/Player/spr_idle", "idle", true, 0.1f);
        LoadAnimation("Sprites/LevelObjects/Player/spr_run@13", "run", true, 0.04f);
        LoadAnimation("Sprites/LevelObjects/Player/spr_jump@14", "jump", false, 0.08f);
        LoadAnimation("Sprites/LevelObjects/Player/spr_celebrate@14", "celebrate", false, 0.05f);
        LoadAnimation("Sprites/LevelObjects/Player/spr_die@5", "die", true, 0.1f);
        LoadAnimation("Sprites/LevelObjects/Player/spr_explode@5x5", "explode", false, 0.04f);

        Reset();
    }

    public override void Reset()
    {
        //remove the gun
        Gun = null;

        // go back to the starting position
        localPosition = startPosition;
        velocity = Vector2.Zero;
        desiredHorizontalSpeed = 0;

        // start with the idle sprite
        PlayAnimation("idle", true);
        SetOriginToBottomCenter();
        facingLeft = false;
        isGrounded = true;
        standingOnIceTile = standingOnHotTile = false;

        IsAlive = true;
        isExploding = false;
        isCelebrating = false;
        IsRising = false;
        isOnMovingObject = false;
    }

    public override void HandleInput(InputHelper inputHelper)
    {
        if(Gun != null)
            Gun.HandleInput(inputHelper);

        if (!CanCollideWithObjects)
            return;

        // arrow keys: move left or right(or alternative WASD layout)
        if (inputHelper.KeyDown(Keys.Left) || inputHelper.KeyDown(Keys.A))
        {
            facingLeft = true;
            desiredHorizontalSpeed = -walkingSpeed;
            if (isGrounded)
                PlayAnimation("run");
        }
        else if (inputHelper.KeyDown(Keys.Right) || inputHelper.KeyDown(Keys.D))
        {
            facingLeft = false;
            desiredHorizontalSpeed = walkingSpeed;
            if (isGrounded)
                PlayAnimation("run");
        }

        // no arrow keys: don't move
        else
        {
            desiredHorizontalSpeed = 0;
            if (isGrounded)
                PlayAnimation("idle");
        }

        // spacebar: jump
        if (isGrounded && inputHelper.KeyPressed(Keys.Space))
            Jump();

        // falling?
        if (!isGrounded)
            PlayAnimation("jump", false, 8);

        // set the origin to the character's feet
        SetOriginToBottomCenter();

        // make sure the sprite is facing the correct direction
        if (Gun != null)
            sprite.Mirror = Gun.Mirror;
        else
            sprite.Mirror = facingLeft;

        //Move slower if not facing the way you are moving
        if (sprite.Mirror != facingLeft)
            desiredHorizontalSpeed *= backwardsWalkingMultiplier;
    }

    public void Jump(float speed = jumpSpeed)
    {
        IsRising = true;
        velocity.Y = -speed;
        isGrounded = false;
        // play the jump animation; always make sure that the animation restarts
        PlayAnimation("jump", true);
        // play a sound
        ExtendedGame.AssetManager.PlaySoundEffect("Sounds/snd_player_jump");
        
    }

    /// <summary>
    /// Returns whether or not the Player is currently falling down.
    /// </summary>
    public bool IsFalling
    {
        get { return velocity.Y > 0 && !isGrounded; }
    }
    bool IsRising;
    
    void SetOriginToBottomCenter()
    {
        Origin = new Vector2(sprite.Width / 2, sprite.Height);
    }
    public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
    {
        base.Draw(gameTime, spriteBatch);
        if (Gun != null)
            Gun.Draw(gameTime, spriteBatch);
    }
    public override void Update(GameTime gameTime)
    {
        if(Gun != null)
            Gun.Update(gameTime);

        Vector2 previousPosition = localPosition;

        if (isOnMovingObject)
            desiredHorizontalSpeed += objectHorizontalVelocity;

        if (CanCollideWithObjects)
            ApplyFriction(gameTime);
        else
            velocity.X = 0;

        if (!isExploding)
            if(!isGrounded)
                ApplyGravity(gameTime);

        base.Update(gameTime);
        if (isGrounded &&!IsRising) 
            velocity.Y = 0;
        if (IsFalling)
            IsRising = false;

        if (IsAlive)
        {
            // check for collisions with tiles
            HandleTileCollisions(previousPosition);
            if (isOnMovingObject)
            {
                isGrounded = true;
            }
            // check if we've fallen down through the level
            if (BoundingBox.Center.Y > level.BoundingBox.Bottom)
                Die();

            if (standingOnHotTile)
                level.Timer.Multiplier = 2;
            else
                level.Timer.Multiplier = 1;

        }
        isOnMovingObject = false;
    }

    void ApplyFriction(GameTime gameTime)
    {
        // determine the friction coefficient for the character
        float friction;
        if (standingOnIceTile)
            friction = iceFriction;
        else if (isGrounded)
            friction = normalFriction;
        else
            friction = airFriction;

        // calculate how strongly the horizontal speed should move towards the desired value
        float multiplier = MathHelper.Clamp(friction * (float)gameTime.ElapsedGameTime.TotalSeconds, 0, 1);

        // update the horizontal speed
        velocity.X += (desiredHorizontalSpeed - velocity.X) * multiplier;
        if (Math.Abs(velocity.X) < 1)
            velocity.X = 0;
    }

    void ApplyGravity(GameTime gameTime)
    {
        velocity.Y += gravity * (float)gameTime.ElapsedGameTime.TotalSeconds;
        if (velocity.Y > maxFallSpeed)
            velocity.Y = maxFallSpeed;
    }

    // Checks for collisions between the character and the level's tiles, and handles these collisions when needed.
    void HandleTileCollisions(Vector2 previousPosition)
    {
        isGrounded = false;
        standingOnIceTile = false;
        standingOnHotTile = false;

        // determine the range of tiles to check
        Rectangle bbox = BoundingBoxForCollisions;
        Point topLeftTile = level.GetTileCoordinates(new Vector2(bbox.Left, bbox.Top)) - new Point(1, 1);
        Point bottomRightTile = level.GetTileCoordinates(new Vector2(bbox.Right, bbox.Bottom)) + new Point(1, 1);

        for (int y = topLeftTile.Y; y <= bottomRightTile.Y; y++)
        {
            for (int x = topLeftTile.X; x <= bottomRightTile.X; x++)
            {
                Tile.Type tileType = level.GetTileType(x, y);

                // ignore empty tiles
                if (tileType == Tile.Type.Empty)
                    continue;

                // ignore platform tiles if the player is standing below them
                Vector2 tilePosition = level.GetCellPosition(x, y);
                if (tileType == Tile.Type.Platform && localPosition.Y > tilePosition.Y && previousPosition.Y > tilePosition.Y)
                    continue;

                // if there's no intersection with the tile, ignore this tile
                Rectangle tileBounds = new Rectangle((int)tilePosition.X, (int)tilePosition.Y, Level.TileWidth, Level.TileHeight);
                if (!tileBounds.Intersects(bbox))
                    continue;

                // calculate how large the intersection is
                Rectangle overlap = CollisionDetection.CalculateIntersection(bbox, tileBounds);

                // if the x-component is smaller, treat this as a horizontal collision
                if (overlap.Width < overlap.Height)
                {
                    if ((bbox.Center.X < tileBounds.Left) || // right wall of the world
                        (bbox.Center.X > tileBounds.Right)) // left wall of the world
                    {
                        localPosition.X = previousPosition.X;
                        velocity.X = 0;
                    }
                }

                // otherwise, treat this as a vertical collision
                else
                {
                    if (velocity.Y >= 0 && bbox.Center.Y < tileBounds.Top && overlap.Width > 6) // floor
                    {
                        isGrounded = true;
                        velocity.Y = 0;
                        localPosition.Y = tileBounds.Top;

                        // check the surface type: are we standing on a hot tile or an ice tile?
                        Tile.SurfaceType surface = level.GetSurfaceType(x, y);
                        if (surface == Tile.SurfaceType.Hot)
                            standingOnHotTile = true;
                        else if (surface == Tile.SurfaceType.Ice)
                            standingOnIceTile = true;
                    }
                    else if (velocity.Y <= 0 && bbox.Center.Y > tileBounds.Bottom && overlap.Height > 2) // ceiling
                    {
                        localPosition.Y = previousPosition.Y;
                        velocity.Y = 0;
                    }
                }
            }
        }
    }

    Rectangle BoundingBoxForCollisions
    {
        get
        {
            Rectangle bbox = BoundingBox;
            // adjust the bounding box
            bbox.X += 20;
            bbox.Width -= 40;
            bbox.Height += 1;

            return bbox;
        }
    }

    public void Die()
    {
        if(Gun != null)
            Gun.Visible = false;

        IsAlive = false;
        PlayAnimation("die");
        velocity = new Vector2(0, -jumpSpeed);
        level.Timer.Running = false;

        ExtendedGame.AssetManager.PlaySoundEffect("Sounds/snd_player_die");
    }

    public void Explode()
    {
        Gun.Visible = false;

        IsAlive = false;
        isExploding = true;
        PlayAnimation("explode");
        velocity = Vector2.Zero;

        ExtendedGame.AssetManager.PlaySoundEffect("Sounds/snd_player_explode");
    }

    /// <summary>
    /// Lets this Player object start celebrating due to completing a level.
    /// The Player will show an animation, and it will stop responding to keyboard input.
    /// </summary>
    public void Celebrate()
    {
        if(Gun != null)
            Gun.Visible = false;

        isCelebrating = true;
        PlayAnimation("celebrate");
        SetOriginToBottomCenter();

        // stop moving
        velocity = Vector2.Zero;
    }
    /// <summary>
    /// Makes the player move with a different object
    /// </summary>
    /// <param name="velocity"></param>velocity of the object.
    /// <param name="gameTime"></param>
    public void MoveWithObject(float horizontalVelocity, float yPosition)
    {
        objectHorizontalVelocity = horizontalVelocity;
        if(!IsRising)
            LocalPosition = new Vector2(LocalPosition.X, yPosition);
        isOnMovingObject = true;
        
    }
}