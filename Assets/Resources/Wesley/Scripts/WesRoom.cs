using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// I struggled with creating good room generation so I just used CA; I'll definitely keep working on this.
/// </summary>
public class WesRoom : Room
{
    private List<Vector2Int> _occupiedTiles = new List<Vector2Int>();

    public override void fillRoom(LevelGenerator ourGenerator, ExitConstraint requiredExits)
    {
        bool extraDownExitRequired = ChooseExtraExit();
        bool extraUpExitRequired = ChooseExtraExit();
        bool extraLeftExitRequired = ChooseExtraExit();
        bool extraRightExitRequired = ChooseExtraExit();

        if (Random.Range(0, 10) < 9) //only generate borders 90% of the time
        {
            #region generate borders
            for (int i = 0; i < LevelGenerator.ROOM_WIDTH; i++)
            {
                for (int j = 0; j < LevelGenerator.ROOM_HEIGHT; j++)
                {
                    if (i != 0 && i != LevelGenerator.ROOM_WIDTH - 1 && j != 0 && j != LevelGenerator.ROOM_HEIGHT - 1)
                    {
                        continue;
                    }
                    #region generate exits
                    if (requiredExits.downExitRequired || extraDownExitRequired)
                    {
                        if (j == 0)
                        {
                            if (i == 4)
                            {
                                continue;
                            }
                            if (i == 5)
                            {
                                continue;
                            }
                        }
                    }
                    if (requiredExits.upExitRequired || extraUpExitRequired)
                    {
                        if (j == LevelGenerator.ROOM_HEIGHT - 1)
                        {
                            if (i == 4)
                            {
                                continue;
                            }
                            if (i == 5)
                            {
                                continue;
                            }
                        }
                    }
                    if (requiredExits.leftExitRequired || extraLeftExitRequired)
                    {
                        if (i == 0)
                        {
                            if (j == 3)
                            {
                                continue;
                            }
                            if (j == 4)
                            {
                                continue;
                            }
                        }
                    }
                    if (requiredExits.rightExitRequired || extraRightExitRequired)
                    {
                        if (i == LevelGenerator.ROOM_WIDTH - 1)
                        {
                            if (j == 3)
                            {
                                continue;
                            }
                            if (j == 4)
                            {
                                continue;
                            }
                        }
                    }
                    #endregion
                    Tile.spawnTile(ourGenerator.normalWallPrefab, transform, i, j);
                    _occupiedTiles.Add(new Vector2Int(i, j));
                }
            }
            #endregion
        }
        #region cellular automata
        for (int i = 0; i < LevelGenerator.ROOM_WIDTH; i++)
        {
            for (int j = 0; j < LevelGenerator.ROOM_HEIGHT; j++)
            {
                if (_occupiedTiles.Contains(new Vector2Int(i, j)))
                {
                    continue;
                }
                if (Random.Range(0, 10) < 1)
                {
                    _occupiedTiles.Add(new Vector2Int(i, j));
                }
            }
        }
        bool[,] wallGrid = PerformCAStep();
        for (int i = 0; i < LevelGenerator.ROOM_WIDTH; i++)
        {
            for (int j = 0; j < LevelGenerator.ROOM_HEIGHT; j++)
            {
                if (wallGrid[i, j])
                {
                    if (_occupiedTiles.Contains(new Vector2Int(i, j)))
                    {
                        Tile.spawnTile(ourGenerator.normalWallPrefab, transform, i, j);
                        continue;
                    }
                    else
                    {
                        Tile.spawnTile(ourGenerator.normalWallPrefab, transform, i, j);
                        _occupiedTiles.Add(new Vector2Int(i, j));
                    }
                }
                if (!wallGrid[i, j])
                {
                    if (!_occupiedTiles.Contains(new Vector2Int(i, j)))
                    {
                        continue;
                    }
                    else
                    {
                        _occupiedTiles.Remove(new Vector2Int(i, j));
                    }
                }
            }
        }
        #endregion
        #region place trip mines
        for (int i = 0; i < LevelGenerator.ROOM_WIDTH; i++)
        {
            for (int j = 0; j < LevelGenerator.ROOM_HEIGHT; j++)
            {
                if(_occupiedTiles.Contains(new Vector2Int(i, j)))
                {
                    continue;
                }
                if(Random.Range(0, 50) < 1)
                {
                    Tile.spawnTile(localTilePrefabs[1], transform, i, j);
                    _occupiedTiles.Add(new Vector2Int(i, j));
                }
            }
        }
        #endregion
        #region place teleporters
        for (int i = 0; i < LevelGenerator.ROOM_WIDTH; i++)
        {
            for (int j = 0; j < LevelGenerator.ROOM_HEIGHT; j++)
            {
                if (_occupiedTiles.Contains(new Vector2Int(i, j)))
                {
                    continue;
                }
                if (Random.Range(0, 50) < 1)
                {
                    Tile.spawnTile(localTilePrefabs[0], transform, i, j);
                    _occupiedTiles.Add(new Vector2Int(i, j));
                }
            }
        }
        #endregion
    }

    bool NextCAValue(int x, int y)
    {
        int livingNeighborsCount = 0;
        for (int i = -1; i < 2; i++)
        {
            for (int j = -1; j < 2; j++)
            {
                if (i == 0 && j == 0)
                {

                }
                else if (x + i < 1 || x + i > LevelGenerator.ROOM_WIDTH - 1 || y + j < 1 || y + j > LevelGenerator.ROOM_HEIGHT - 1)
                {

                }
                else if (!_occupiedTiles.Contains(new Vector2Int(x + i, y + j)))
                {

                }
                else
                {
                    livingNeighborsCount++;
                }
            }
        }
        if (_occupiedTiles.Contains(new Vector2Int(x, y)))
        {
            return livingNeighborsCount > 2;
        }
        else
        {
            return livingNeighborsCount >= 3;
        }
    }

    private bool[,] PerformCAStep()
    {
        bool[,] nextWallGrid = new bool[LevelGenerator.ROOM_WIDTH, LevelGenerator.ROOM_HEIGHT];
        for (int x = 0; x < LevelGenerator.ROOM_WIDTH; x++)
        {
            for (int y = 0; y < LevelGenerator.ROOM_HEIGHT; y++)
            {
                nextWallGrid[x, y] = NextCAValue(x, y);
            }
        }
        return nextWallGrid;
    }

    private bool ChooseExtraExit()
    {
        if (Random.Range(0, 2) < 1)
        {
            return true;
        }
        return false;
    }
}
