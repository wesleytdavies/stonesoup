using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PillarRoom : Room
{
    public GameObject rockPrefab;
    public GameObject sweetRockPrefab;
    public GameObject enemyPrefab;
    public List<GameObject> treasurePrefabs;
    
    public float sweetRockProbability = 0.1f;
    public float borderWallProbability = 0.7f;
    
    public int minNumRocks = 1, maxNumRocks = 6;
    public int minNumEnemies = 1, maxNumEnemies = 2;
    public int minNumTreasures = 0, maxNumTreasures = 1;
    public override void fillRoom(LevelGenerator ourGenerator, ExitConstraint requiredExits)
    {
        roomGeneration(ourGenerator, requiredExits);
    }

    protected void roomGeneration(LevelGenerator ourGenerator, ExitConstraint requiredExits)
    {
        // In this version of room generation, I generate walls and then other stuff.
        generateWalls(ourGenerator, requiredExits);
        // Inside the borders I make some rocks and enemies.
        int numRocks = Random.Range(minNumRocks, maxNumRocks + 1);
        int numEnemies = Random.Range(minNumEnemies, maxNumEnemies + 1);
        int numTreasures = Random.Range(minNumTreasures, maxNumTreasures + 1);

        // First, let's make an array keeping track of where we've spawned objects already.
        bool[,] occupiedPositions = new bool[LevelGenerator.ROOM_WIDTH, LevelGenerator.ROOM_HEIGHT];
        for (int x = 0; x < LevelGenerator.ROOM_WIDTH; x++)
        {
            for (int y = 0; y < LevelGenerator.ROOM_HEIGHT; y++)
            {
                if (x == 0 || x == LevelGenerator.ROOM_WIDTH - 1
                           || y == 0 || y == LevelGenerator.ROOM_HEIGHT - 1)
                {
                    // All border zones are occupied.
                    occupiedPositions[x, y] = true;
                }
                else
                {
                    occupiedPositions[x, y] = false;
                }
            }
        }

        // Now we spawn rocks and enemies in random locations
        List<Vector2> possibleSpawnPositions =
            new List<Vector2>(LevelGenerator.ROOM_WIDTH * LevelGenerator.ROOM_HEIGHT);
        for (int i = 0; i < numRocks; i++)
        {
            possibleSpawnPositions.Clear();
            for (int x = 0; x < LevelGenerator.ROOM_WIDTH; x++)
            {
                for (int y = 0; y < LevelGenerator.ROOM_HEIGHT; y++)
                {
                    if (occupiedPositions[x, y])
                    {
                        continue;
                    }

                    possibleSpawnPositions.Add(new Vector2(x, y));
                }
            }

            if (possibleSpawnPositions.Count > 0)
            {
                Vector2 spawnPos = GlobalFuncs.randElem(possibleSpawnPositions);
                if (Random.value <= sweetRockProbability)
                {
                    Tile.spawnTile(sweetRockPrefab, transform, (int) spawnPos.x, (int) spawnPos.y);
                }
                else
                {
                    Tile.spawnTile(rockPrefab, transform, (int) spawnPos.x, (int) spawnPos.y);
                }

                occupiedPositions[(int) spawnPos.x, (int) spawnPos.y] = true;
            }
        }
        

        for (int i = 0; i < numEnemies; i++)
        {
            possibleSpawnPositions.Clear();
            for (int x = 0; x < LevelGenerator.ROOM_WIDTH; x++)
            {
                for (int y = 0; y < LevelGenerator.ROOM_HEIGHT; y++)
                {
                    if (occupiedPositions[x, y])
                    {
                        continue;
                    }

                    possibleSpawnPositions.Add(new Vector2(x, y));
                }
            }

            if (possibleSpawnPositions.Count > 0)
            {
                Vector2 spawnPos = GlobalFuncs.randElem(possibleSpawnPositions);
                Tile.spawnTile(enemyPrefab, transform, (int) spawnPos.x, (int) spawnPos.y);
                occupiedPositions[(int) spawnPos.x, (int) spawnPos.y] = true;
            }
        }
        
        for (int i = 0; i < numTreasures; i++)
        {
            possibleSpawnPositions.Clear();
            for (int x = 0; x < LevelGenerator.ROOM_WIDTH; x++)
            {
                for (int y = 0; y < LevelGenerator.ROOM_HEIGHT; y++)
                {
                    if (occupiedPositions[x, y])
                    {
                        continue;
                    }

                    possibleSpawnPositions.Add(new Vector2(x, y));
                }
            }

            if (possibleSpawnPositions.Count > 0)
            {
                Vector2 spawnPos = GlobalFuncs.randElem(possibleSpawnPositions);
                Tile.spawnTile(treasurePrefabs[Random.Range(0, treasurePrefabs.Count)], transform, (int) spawnPos.x, (int) spawnPos.y);
                occupiedPositions[(int) spawnPos.x, (int) spawnPos.y] = true;
            }
        }
    }

    protected void generateWalls(LevelGenerator ourGenerator, ExitConstraint requiredExits)
    {
        // Basically we go over the border and determining where to spawn walls.
        bool[,] wallMap = new bool[LevelGenerator.ROOM_WIDTH, LevelGenerator.ROOM_HEIGHT];
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
}