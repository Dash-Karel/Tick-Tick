using Microsoft.Xna.Framework;
using Engine;
using System;
using Microsoft.Xna.Framework.Graphics;

abstract class Projectile : SpriteGameObject
{
    public bool MarkedForRemoval { get; private set; } = false;

    protected float timeToLive = 3f;
    protected float travelSpeed;
    protected float gravityStrength = 1000f;
    protected bool useGravity;

    protected Level level;

    float rotation;

    protected Projectile(Vector2 startPosition, string spriteName,Level level) : base(spriteName, TickTick.Depth_LevelPlayer)
    {
        SetOriginToCenter();
        localPosition = startPosition;
        this.level = level;
    }

    /// <summary>
    /// Counts down timeToLive, handles collisions and adds gravity to the projectile
    /// </summary>
    /// <param name="gameTime"></param>
    public override void Update(GameTime gameTime)
    {
        float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
        timeToLive -= deltaTime;
        //If the timeToLive is depleted the projectile should be removed
        if (timeToLive < 0)
        {
            MarkedForRemoval = true;
            return;
        }

        Vector2 previousPosition = localPosition;

        //Add gravity if desired
        if (useGravity)
        {
            velocity.Y += gravityStrength * deltaTime;
            rotation = MathF.Atan2(velocity.Y, velocity.X);
        }
        base.Update(gameTime);

        HandleTileCollisions(previousPosition);
    }

    /// <summary>
    /// Launch the projectile in the direction specified at the standard speed of the projectile
    /// </summary>
    /// <param name="direction"></param>
    public void StartFlying(Vector2 direction)
    {
        direction.Normalize();
        velocity = direction * travelSpeed;
        rotation = MathF.Atan2(velocity.Y, velocity.X);
    }

    public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
    {
        if (!Visible)
            return;

        // draw the sprite at its *global* position in the game world
        if (sprite != null)
            sprite.Draw(spriteBatch, GlobalPosition, Origin, rotation);
    }

    // Checks for collisions between the projectile and the level's tiles, and handles these collisions when needed.
    void HandleTileCollisions(Vector2 previousPosition)
    {
        // determine the range of tiles to check
        Rectangle bbox = BoundingBox;
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

                // ignore platform tiles if the bullet is flying below them
                Vector2 tilePosition = level.GetCellPosition(x, y);
                if (tileType == Tile.Type.Platform && localPosition.Y > tilePosition.Y && previousPosition.Y > tilePosition.Y)
                    continue;

                // if there's no intersection with the tile, ignore this tile
                Rectangle tileBounds = new Rectangle((int)tilePosition.X, (int)tilePosition.Y, Level.TileWidth, Level.TileHeight);
                if (!tileBounds.Intersects(bbox))
                    continue;

                CollideWithTile();
                return;
                
            }
        }
    }

    /// <summary>
    /// Called when colliding with a tile
    /// </summary>
    protected virtual void CollideWithTile()
    {
        MarkedForRemoval = true;
    }

    /// <summary>
    /// Called by the enemy that collides with the projectile
    /// </summary>
    /// <param name="enemy"></param>
    public virtual void CollideWithEnemy(Enemy enemy)
    {
        MarkedForRemoval = true;
        enemy.Die();
    }
}
