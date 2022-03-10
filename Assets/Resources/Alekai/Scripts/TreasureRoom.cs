using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreasureRoom : Room
{
    public GameObject rockPrefab;
    public GameObject sweetRockPrefab;
    public List<GameObject> treasureList;
    public float borderWallProbability = 0.7f;
    public float sweetRockProbability = .5f;
    public int treasureAmountMin = 1, treasureAmountMax = 4;
    public int treasureWallMinWidth = 4, treasureWallMaxWidth = 5;
    public int treasureWallMinHeight = 3, treasureWallMaxHeight = 4;

    public override void fillRoom(LevelGenerator ourGenerator, ExitConstraint requiredExits)
    {
        generateWalls(ourGenerator, requiredExits);
    }

    protected void generateWalls(LevelGenerator ourGenerator, ExitConstraint requiredExits)
    {
        bool[,] wallMap = new bool[LevelGenerator.ROOM_WIDTH, LevelGenerator.ROOM_HEIGHT];
        // wallMap = generateTreasureWalls(wallMap);
        wallMap = generateTreasures(wallMap);
        
        // required exits
        for (int x = 0; x < LevelGenerator.ROOM_WIDTH; x++)
        {
            for (int y = 0; y < LevelGenerator.ROOM_HEIGHT; y++)
            {
                if (x == 0 || x == LevelGenerator.ROOM_WIDTH - 1
                           || y == 0 || y == LevelGenerator.ROOM_HEIGHT - 1)
                {
                    if (x == LevelGenerator.ROOM_WIDTH / 2
                        && y == LevelGenerator.ROOM_HEIGHT - 1
                        && requiredExits.upExitRequired)
                    {
                        wallMap[x, y] = false;
                    }
                    else if (x == LevelGenerator.ROOM_WIDTH - 1
                             && y == LevelGenerator.ROOM_HEIGHT / 2
                             && requiredExits.rightExitRequired)
                    {
                        wallMap[x, y] = false;
                    }
                    else if (x == LevelGenerator.ROOM_WIDTH / 2
                             && y == 0
                             && requiredExits.downExitRequired)
                    {
                        wallMap[x, y] = false;
                    }
                    else if (x == 0
                             && y == LevelGenerator.ROOM_HEIGHT / 2
                             && requiredExits.leftExitRequired)
                    {
                        wallMap[x, y] = false;
                    }
                    else
                    {
                        wallMap[x, y] = Random.value <= borderWallProbability;
                    }

                    continue;
                }

                wallMap[x, y] = false;
            }
        }

        // Now actually spawn all the walls.
        for (int x = 0; x < LevelGenerator.ROOM_WIDTH; x++)
        {
            for (int y = 0; y < LevelGenerator.ROOM_HEIGHT; y++)
            {
                if (wallMap[x, y])
                {
                    if (Random.value <= sweetRockProbability)
                    {
                        Tile.spawnTile(sweetRockPrefab, transform, x, y);
                    }
                    else
                    {
                     Tile.spawnTile(ourGenerator.normalWallPrefab, transform, x, y);
                    }
                }
            }
        }
    }

    private bool[,] generateTreasures(bool[,] wallMap)
    {
        var newMap = wallMap;
        var emptyTiles = new List<Vector2Int>();

        for (int x = 0; x < LevelGenerator.ROOM_WIDTH; x++)
        {
            for (int y = 0; y < LevelGenerator.ROOM_HEIGHT; y++)
            {
                if (!wallMap[x, y])
                {
                    emptyTiles.Add(new Vector2Int(x, y));
                }
            }
        }

        var treasureAmount = Random.Range(treasureAmountMin, treasureAmountMax);

        for (int i = 0; i < treasureAmount; i++)
        {
            var tile = emptyTiles[Random.Range(0, emptyTiles.Count)];
            Tile.spawnTile(treasureList[Random.Range(0, treasureList.Count)], transform, tile.x, tile.y);
        }
        return newMap;
    }

    private bool[,] generateTreasureWalls(bool[,] wallMap)
        {
        var newMap = wallMap;
        // treasure walls
        var treasureCaveWidth = Random.Range(treasureWallMinWidth, treasureWallMaxWidth);
        var treasureCaveHeight = Random.Range(treasureWallMinHeight, treasureWallMaxHeight);
        for (int x = 0; x < LevelGenerator.ROOM_WIDTH; x++)
        {
            for (int y = 0; y < LevelGenerator.ROOM_HEIGHT; y++)
            {
                if (x == 3 || x == 3 + treasureCaveWidth)
                {
                    newMap[x, y] = true;
                    continue;
                }

                if (y == 3 || x == y + treasureCaveHeight)
                {
                    newMap[x, y] = true;
                }
            }
        }

        return newMap;
    }
}