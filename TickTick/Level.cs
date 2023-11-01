using Engine;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;

partial class Level : GameObjectList
{
    public const int TileWidth = 72;
    public const int TileHeight = 55;

    Tile[,] tiles;
    List<WaterDrop> waterDrops;

    public Player Player { get; private set; }
    public int LevelIndex { get; private set; }

    //a reference to TickTick to be able to update the world size
    TickTick game;

    SpriteGameObject goal;
    BombTimer timer;

    bool completionDetected;

    public Level(int levelIndex, string filename, TickTick game)
    {
        //save the reference to the Game 
        this.game = game;

        LevelIndex = levelIndex;

        // load the rest of the level
        LoadLevelFromFile(filename);

        // load the backgrounds
        GameObjectList backgrounds = new GameObjectList();
        SpriteGameObject backgroundSky = new SpriteGameObject("Sprites/Backgrounds/spr_sky", TickTick.Depth_Background);
        backgroundSky.LocalPosition = new Vector2(0f, 825 - backgroundSky.Height);
        backgrounds.AddChild(backgroundSky);
        AddChild(backgrounds);

        int AmountOfStandardWorldWidths = (int)MathF.Ceiling(BoundingBox.Width / 2048f);

        // add mountains in the background
        int mountainsAmount = AmountOfStandardWorldWidths * 2;
        for (int i = 0; i < mountainsAmount; i++)
        {
            float mountainDepth = TickTick.ParallaxEndDepth / 4 - TickTick.ParallaxEndDepth / 4 * ExtendedGame.Random.NextSingle();
            SpriteGameObject mountain = new SpriteGameObject(
                "Sprites/Backgrounds/spr_mountain_" + (ExtendedGame.Random.Next(2) + 1), mountainDepth);

            float parallaxMultiplier = mountainDepth / ExtendedGame.ParallaxEndDepth;
            mountain.LocalPosition = new Vector2(mountain.Width * (i-1) * 0.21f, 
                TickTick.Camera.Size.Y + (BoundingBox.Height - TickTick.Camera.Size.Y) * parallaxMultiplier - mountain.Height);

            backgrounds.AddChild(mountain);
        }

        // add clouds
        int cloudsAmount = AmountOfStandardWorldWidths * 6;
       for (int i = 0; i < cloudsAmount; i++)
            backgrounds.AddChild(new Cloud(this));
    }

    public Rectangle BoundingBox
    {
        get
        {
            return new Rectangle(0, 0,
                tiles.GetLength(0) * TileWidth,
                tiles.GetLength(1) * TileHeight);
        }
    }

    public BombTimer Timer { get { return timer; } }

    public Vector2 GetCellPosition(int x, int y)
    {
        return new Vector2(x * TileWidth, y * TileHeight);
    }

    public Point GetTileCoordinates(Vector2 position)
    {
        return new Point((int)Math.Floor(position.X / TileWidth), (int)Math.Floor(position.Y / TileHeight));
    }

    public Tile.Type GetTileType(int x, int y)
    {
        // If the x-coordinate is out of range, treat the coordinates as a wall tile.
        // This will prevent the character from walking outside the level.
        if (x < 0 || x >= tiles.GetLength(0))
            return Tile.Type.Wall;

        // If the y-coordinate is out of range, treat the coordinates as an empty tile.
        // This will allow the character to still make a full jump near the top of the level.
        if (y < 0 || y >= tiles.GetLength(1))
            return Tile.Type.Empty;

        return tiles[x, y].TileType;
    }

    public Tile.SurfaceType GetSurfaceType(int x, int y)
    {
        // If the tile with these coordinates doesn't exist, return the normal surface type.
        if (x < 0 || x >= tiles.GetLength(0) || y < 0 || y >= tiles.GetLength(1))
            return Tile.SurfaceType.Normal;

        // Otherwise, return the actual surface type of the tile.
        return tiles[x, y].Surface;
    }

    public override void Update(GameTime gameTime)
    {
        base.Update(gameTime);

        // check if we've finished the level
        if (!completionDetected && AllDropsCollected && Player.HasPixelPreciseCollision(goal))
        {
            completionDetected = true;
            ExtendedGameWithLevels.GetPlayingState().LevelCompleted(LevelIndex);
            Player.Celebrate();

            // stop the timer
            timer.Running = false;
        }

        // check if the timer has passed
        else if (Player.IsAlive && timer.HasPassed)
        {
            Player.Explode();
        }
    }

    /// <summary>
    /// Checks and returns whether the player has collected all water drops in this level.
    /// </summary>
    bool AllDropsCollected
    {
        get
        {
            foreach (WaterDrop drop in waterDrops)
                if (drop.Visible)
                    return false;
            return true;
        }
    }

    public override void Reset()
    {
        base.Reset();
        completionDetected = false;
    }
}

