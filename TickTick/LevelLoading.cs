﻿using Engine;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.IO;

partial class Level : GameObjectList
{
    void LoadLevelFromFile(string filename)
    {
        // open the file
        StreamReader reader = new StreamReader(filename);

        // read the description
        string description = reader.ReadLine();

        // read the rows of the grid; keep track of the longest row
        int gridWidth = 0;

        List<string> gridRows = new List<string>();
        string line = reader.ReadLine();
        while (line != null && line[0] != 't')
        {
            if (line.Length > gridWidth)
                gridWidth = line.Length;

            gridRows.Add(line);
            line = reader.ReadLine();
        }
        
        //read the timer length, if it is present
        double timerLength = 30;
        if (line != null) 
        {
            line = line.Remove(0, 1);
            timerLength = double.Parse(line);
        }

        // add the timer
        timer = new BombTimer(timerLength);
        AddChild(timer);
        
        // stop reading the file
        reader.Close();
        
        // create all game objects for the grid
        AddPlayingField(gridRows, gridWidth, gridRows.Count);

        // add game objects to show that general level info
        AddLevelInfoObjects(description);

        // make sure the world size matches the size of the playing field
        game.WorldSize = new Point(BoundingBox.Width, BoundingBox.Height);
    }

    void AddLevelInfoObjects(string description)
    {
        // - background box
        SpriteGameObject frame = new SpriteGameObject("Sprites/UI/spr_frame_hint", TickTick.Depth_UIBackground);
        frame.SetOriginToCenter();
        frame.LocalPosition = new Vector2(720, 50);
        AddChild(frame);

        // - text
        TextGameObject hintText = new TextGameObject("Fonts/HintFont", TickTick.Depth_UIForeground, Color.Black, TextGameObject.Alignment.Left);
        hintText.Text = description;
        hintText.LocalPosition = new Vector2(510, 40);
        AddChild(hintText);
    }

    void AddPlayingField(List<string> gridRows, int gridWidth, int gridHeight)
    {
        // create a parent object for everything
        GameObjectList playingField = new GameObjectList();

        // initialize the list of water drops
        waterDrops = new List<WaterDrop>();

        // prepare the grid arrays
        tiles = new Tile[gridWidth, gridHeight];

        // load the tiles
        for (int y = 0; y < gridHeight; y++)
        {
            string row = gridRows[y];
            for (int x = 0; x < gridWidth; x++)
            {
                // the row could be too short; if so, pretend there is an empty tile
                char symbol = '.';
                if (x < row.Length)
                    symbol = row[x];

                // load the tile
                AddTile(x, y, symbol);
            }
        }
    }

    void AddTile(int x, int y, char symbol)
    {
        // load the static part of the tile
        Tile tile = CharToStaticTile(symbol);
        tile.LocalPosition = GetCellPosition(x, y);
        AddChild(tile);

        // store a reference to that tile in the grid
        tiles[x, y] = tile;

        // load the dynamic part of the tile
        if (symbol == '1')
            LoadCharacter(x, y);
        else if (symbol == 'X')
            LoadGoal(x, y);
        else if (symbol == 'W')
            LoadWaterDrop(x, y);
        else if (symbol == 'R')
            LoadRocketEnemy(x, y);
        else if (symbol == 'T')
            LoadTurtleEnemy(x, y);
        else if (symbol == 'S')
            LoadSparkyEnemy(x, y);
        else if (symbol == 'A' || symbol == 'B' || symbol == 'C')
            LoadFlameEnemy(x, y, symbol);
        else if (symbol == 's')
            LoadSpeedPowerUp(x, y);
        else if (symbol == 'a' || symbol == 'b' || symbol == 'm' || symbol == 'p' || symbol == 'P')
            LoadGunCollectible(x, y, symbol);
        else if (symbol == 'M')
            LoadAmmoCollectible(x, y);

    }

    Tile CharToStaticTile(char symbol)
    {
        switch (symbol)
        {
            case '-':
                return new Tile(Tile.Type.Platform, Tile.SurfaceType.Normal);
            case '#':
                return new Tile(Tile.Type.Wall, Tile.SurfaceType.Normal);
            case 'h':
                return new Tile(Tile.Type.Platform, Tile.SurfaceType.Hot);
            case 'H':
                return new Tile(Tile.Type.Wall, Tile.SurfaceType.Hot);
            case 'i':
                return new Tile(Tile.Type.Platform, Tile.SurfaceType.Ice);
            case 'I':
                return new Tile(Tile.Type.Wall, Tile.SurfaceType.Ice);
            default:
                return new Tile(Tile.Type.Empty, Tile.SurfaceType.Normal);
        }
    }

    void LoadCharacter(int x, int y)
    {
        // create the bomb character
        Player = new Player(this, GetCellBottomCenter(x, y));
        AddChild(Player);

        //make sure the camera follows the player
        TickTick.Camera.Subject = Player;
    }

    void LoadGoal(int x, int y)
    {
        // create the exit object
        goal = new SpriteGameObject("Sprites/LevelObjects/spr_goal", TickTick.Depth_LevelObjects);
        // make sure it's standing exactly on the tile below
        goal.LocalPosition = GetCellPosition(x, y+1);
        goal.Origin = new Vector2(0, goal.Height);
        AddChild(goal);
    }

    void LoadWaterDrop(int x, int y)
    {
        // create the water drop object;  place it around the center of the tile
        Vector2 pos = GetCellPosition(x, y) + new Vector2(TileWidth / 2, TileHeight / 3);
        WaterDrop w = new WaterDrop(this, pos);
        // add it to the game world
        AddChild(w);
        // store an extra reference to it
        waterDrops.Add(w);
    }
    void LoadSpeedPowerUp(int x, int y)
    {
        // create the speed powerup object;  place it around the center of the tile
        Vector2 pos = GetCellPosition(x, y) + new Vector2(TileWidth / 2, TileHeight / 3);
        SpeedUpPowerUp s = new SpeedUpPowerUp(this, pos);
        // add it to the game world
        AddChild(s);
    }
    void LoadGunCollectible(int x, int y, char symbol)
    {
        // create the GunCollectible object;  place it around the center of the tile
        Vector2 pos = GetCellPosition(x, y) + new Vector2(TileWidth / 2, TileHeight / 3);
        Type type;
        string spriteName;
        switch(symbol)
        {
            case 'a':
                type = typeof(AssaultRifle);
                spriteName = "Sprites/LevelObjects/Player/Guns/AssaultRifle";
                break;
            case 'b':
                type = typeof(BurstRifle);
                spriteName = "Sprites/LevelObjects/Player/Guns/BurstRifle";
                break;
            case 'm':
                type = typeof(MachinePistol);
                spriteName = "Sprites/LevelObjects/Player/Guns/MachinePistol";
                break;
            case 'p':
                type = typeof(Pistol);
                spriteName = "Sprites/LevelObjects/Player/Guns/Pistol";
                break;
            case 'P':
                type = typeof(PortalGun);
                spriteName = "Sprites/LevelObjects/Player/Guns/PortalGun";
                break;
            default:
                throw new Exception("Invalid char used with symbol parameter of LoadGunCollectible");
        }
        GunCollectible s = new GunCollectible(this, pos, spriteName, type);
        // add it to the game world
        AddChild(s);
    }

    private void LoadAmmoCollectible(int x, int y)
    {
        // create the AmmoCollectible object;  place it around the center of the tile
        Vector2 pos = GetCellPosition(x, y) + new Vector2(TileWidth / 2, TileHeight / 3);
        AmmoCollectible s = new AmmoCollectible(this, pos);
        // add it to the game world
        AddChild(s);
    }


    void LoadRocketEnemy(int x, int y)
    {
        Rocket r = new Rocket(this, GetCellPosition(x, y), x != 0);
        AddChild(r);
    }

    void LoadTurtleEnemy(int x, int y)
    {
        Turtle t = new Turtle(this);
        t.LocalPosition = GetCellBottomCenter(x, y);
        AddChild(t);
    }

    void LoadSparkyEnemy(int x, int y)
    {
        Sparky s = new Sparky(this, GetCellBottomCenter(x, y));
        AddChild(s);
    }

    void LoadFlameEnemy(int x, int y, char symbol)
    {
        Vector2 pos = GetCellBottomCenter(x, y);

        PatrollingEnemy enemy;
        if (symbol == 'A')
            enemy = new PatrollingEnemy(this, pos);
        else if (symbol == 'B')
            enemy = new PlayerFollowingEnemy(this, pos);
        else
            enemy = new UnpredictableEnemy(this, pos);

        AddChild(enemy);
    }

    Vector2 GetCellBottomCenter(int x, int y)
    {
        return GetCellPosition(x, y + 1) + new Vector2(TileWidth / 2, 0);
    }
}