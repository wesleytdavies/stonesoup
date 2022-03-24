using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SearchVertex
{
    public SearchVertex(Vector2Int Point, SearchVertex Parent)
    {
        Point = point;
        Parent = parent;
    }
    public Vector2Int point;
    public SearchVertex parent;
}

public class WesValidatedRoom : Room
{
    private bool _hasUpExit;
    private bool _hasDownExit;
    private bool _hasLeftExit;
    private bool _hasRightExit;

    private bool _hasUpDownPath;
    private bool _hasUpLeftPath;
    private bool _hasUpRightPath;
    private bool _hasDownLeftPath;
    private bool _hasDownRightPath;
    private bool _hasLeftRightPath;

    private float _chanceToSpawnMine = 0.01f;

    private bool DoesListContainPoint(List<SearchVertex> list, Vector2Int point)
    {
        foreach(SearchVertex searchVertex in list)
        {
            if (searchVertex.point == point)
            {
                return true;
            }
        }
        return false;
    }

    private void ValidateRoom()
    {
        int[,] indexGrid = loadIndexGrid();
        Vector2Int upExit = new Vector2Int(LevelGenerator.ROOM_WIDTH / 2, LevelGenerator.ROOM_HEIGHT - 1);
        Vector2Int downExit = new Vector2Int(LevelGenerator.ROOM_WIDTH / 2, 0);
        Vector2Int leftExit = new Vector2Int(0, LevelGenerator.ROOM_HEIGHT / 2);
        Vector2Int rightExit = new Vector2Int(LevelGenerator.ROOM_WIDTH - 1, LevelGenerator.ROOM_HEIGHT / 2);

        _hasUpExit = IsPointNavigable(indexGrid, upExit);
        _hasDownExit = IsPointNavigable(indexGrid, downExit);
        _hasLeftExit = IsPointNavigable(indexGrid, leftExit);
        _hasRightExit = IsPointNavigable(indexGrid, rightExit);

        _hasUpDownPath = DoesPathExist(indexGrid, upExit, downExit);
        _hasUpLeftPath = DoesPathExist(indexGrid, upExit, leftExit);
        _hasUpRightPath = DoesPathExist(indexGrid, upExit, rightExit);
        _hasDownLeftPath = DoesPathExist(indexGrid, downExit, leftExit);
        _hasDownRightPath = DoesPathExist(indexGrid, downExit, rightExit);
        _hasLeftRightPath = DoesPathExist(indexGrid, leftExit, rightExit);

        //if (_hasUpLeftPath)
        //{
        //    Debug.Log("Has up-left path.");
        //    List<Vector2Int> path = GetPathDfs(indexGrid, upExit, leftExit);
        //    foreach(Vector2Int point in path)
        //    {
        //        Debug.Log("Point: " + point.x + ", " + point.y);
        //    }
        //}
    }

    public bool MeetsConstrants(ExitConstraint requiredExits)
    {
        ValidateRoom();

        if (requiredExits.upExitRequired && !_hasUpExit)
        {
            return false;
        }
        if (requiredExits.downExitRequired && !_hasDownExit)
        {
            return false;
        }
        if (requiredExits.leftExitRequired && !_hasLeftExit)
        {
            return false;
        }
        if (requiredExits.rightExitRequired && !_hasRightExit)
        {
            return false;
        }
        if (!_hasUpDownPath && requiredExits.upExitRequired && requiredExits.downExitRequired)
        {
            return false;
        }
        if (!_hasUpLeftPath && requiredExits.upExitRequired && requiredExits.leftExitRequired)
        {
            return false;
        }
        if (!_hasUpRightPath && requiredExits.upExitRequired && requiredExits.rightExitRequired)
        {
            return false;
        }
        if (!_hasDownLeftPath && requiredExits.downExitRequired && requiredExits.leftExitRequired)
        {
            return false;
        }
        if (!_hasDownRightPath && requiredExits.downExitRequired && requiredExits.rightExitRequired)
        {
            return false;
        }
        if (!_hasLeftRightPath && requiredExits.leftExitRequired && requiredExits.rightExitRequired)
        {
            return false;
        }
        return true;
    }

    private bool DoesPathExist(int[,] indexGrid, Vector2Int startPoint, Vector2Int endPoint)
    {
        List<Vector2Int> openSet = new List<Vector2Int>();
        List<Vector2Int> closedSet = new List<Vector2Int>();
        if (IsPointNavigable(indexGrid, startPoint))
        {
            openSet.Add(startPoint);
        }
        while (openSet.Count > 0)
        {
            Vector2Int currentPoint = openSet[0];
            openSet.RemoveAt(0);
            if (currentPoint == endPoint)
            {
                return true;
            }
            Vector2Int upNeighbor = new Vector2Int(currentPoint.x, currentPoint.y + 1);
            if (!openSet.Contains(upNeighbor) && !closedSet.Contains(upNeighbor))
            {
                if(IsPointNavigable(indexGrid, upNeighbor))
                {
                    openSet.Add(upNeighbor);
                }
            }
            Vector2Int downNeighbor = new Vector2Int(currentPoint.x, currentPoint.y - 1);
            if (!openSet.Contains(downNeighbor) && !closedSet.Contains(downNeighbor))
            {
                if (IsPointNavigable(indexGrid, downNeighbor))
                {
                    openSet.Add(downNeighbor);
                }
            }
            Vector2Int leftNeighbor = new Vector2Int(currentPoint.x - 1, currentPoint.y);
            if (!openSet.Contains(leftNeighbor) && !closedSet.Contains(leftNeighbor))
            {
                if (IsPointNavigable(indexGrid, leftNeighbor))
                {
                    openSet.Add(leftNeighbor);
                }
            }
            Vector2Int rightNeighbor = new Vector2Int(currentPoint.x + 1, currentPoint.y);
            if (!openSet.Contains(rightNeighbor) && !closedSet.Contains(rightNeighbor))
            {
                if (IsPointNavigable(indexGrid, rightNeighbor))
                {
                    openSet.Add(rightNeighbor);
                }
            }
            closedSet.Add(currentPoint);
        }
        return false;
    }

    private List<Vector2Int> GetPathDfs(int[,] indexGrid, Vector2Int startPoint, Vector2Int endPoint)
    {
        List<SearchVertex> openSet = new List<SearchVertex>();
        List<SearchVertex> closedSet = new List<SearchVertex>();
        if (IsPointNavigable(indexGrid, startPoint))
        {
            SearchVertex startVertex = new SearchVertex(startPoint, null);
            openSet.Add(startVertex);
        }
        while (openSet.Count > 0)
        {
            int index = openSet.Count - 1;
            SearchVertex currentVertex = openSet[index];
            openSet.RemoveAt(index);
            if (currentVertex.point == endPoint)
            {
                List<Vector2Int> retVal = new List<Vector2Int>();
                while (currentVertex != null)
                {
                    retVal.Add(currentVertex.point);
                    currentVertex = currentVertex.parent;
                }
                retVal.Reverse();
                return retVal;
            }
            Vector2Int upNeighbor = new Vector2Int(currentVertex.point.x, currentVertex.point.y + 1);
            if (!DoesListContainPoint(openSet, upNeighbor) && !DoesListContainPoint(closedSet, upNeighbor))
            {
                if (IsPointNavigable(indexGrid, upNeighbor))
                {
                    SearchVertex upNeighborVertex = new SearchVertex(upNeighbor, currentVertex);
                    openSet.Add(upNeighborVertex);
                }
            }
            Vector2Int downNeighbor = new Vector2Int(currentVertex.point.x, currentVertex.point.y - 1);
            if (!DoesListContainPoint(openSet, downNeighbor) && !DoesListContainPoint(closedSet, downNeighbor))
            {
                if (IsPointNavigable(indexGrid, downNeighbor))
                {
                    SearchVertex downNeighborVertex = new SearchVertex(downNeighbor, currentVertex);
                    openSet.Add(downNeighborVertex);
                }
            }
            Vector2Int leftNeighbor = new Vector2Int(currentVertex.point.x - 1, currentVertex.point.y);
            if (!DoesListContainPoint(openSet, leftNeighbor) && !DoesListContainPoint(closedSet, leftNeighbor))
            {
                if (IsPointNavigable(indexGrid, leftNeighbor))
                {
                    SearchVertex leftNeighborVertex = new SearchVertex(leftNeighbor, currentVertex);
                    openSet.Add(leftNeighborVertex);
                }
            }
            Vector2Int rightNeighbor = new Vector2Int(currentVertex.point.x + 1, currentVertex.point.y);
            if (!DoesListContainPoint(openSet, rightNeighbor) && !DoesListContainPoint(closedSet, rightNeighbor))
            {
                if (IsPointNavigable(indexGrid, rightNeighbor))
                {
                    SearchVertex rightNeighborVertex = new SearchVertex(rightNeighbor, currentVertex);
                    openSet.Add(rightNeighborVertex);
                }
            }
            closedSet.Add(currentVertex);
        }
        return new List<Vector2Int>();
    }

    private bool IsPointNavigable(int[,] indexGrid, Vector2Int point)
    {
        if (!IsPointInGrid(point))
        {
            return false;
        }
        if(indexGrid[point.x, point.y] == 0)
        {
            return true;
        }
        return false;
    }

    private bool IsPointInGrid(Vector2Int point)
    {
        if (point.x < 0 || point.x > LevelGenerator.ROOM_WIDTH - 1)
        {
            return false;
        }
        if (point.y < 0 || point.y > LevelGenerator.ROOM_HEIGHT - 1)
        {
            return false;
        }
        return true;
    }

    //override to allow for spawning mines
    public override void fillRoom(LevelGenerator ourGenerator, ExitConstraint requiredExits)
    {
        string initialGridString = designedRoomFile.text;
        string[] rows = initialGridString.Trim().Split('\n');
        int width = rows[0].Trim().Split(',').Length;
        int height = rows.Length;
        if (height != LevelGenerator.ROOM_HEIGHT)
        {
            throw new UnityException(string.Format("Error in room by {0}. Wrong height, Expected: {1}, Got: {2}", roomAuthor, LevelGenerator.ROOM_HEIGHT, height));
        }
        if (width != LevelGenerator.ROOM_WIDTH)
        {
            throw new UnityException(string.Format("Error in room by {0}. Wrong width, Expected: {1}, Got: {2}", roomAuthor, LevelGenerator.ROOM_WIDTH, width));
        }
        int[,] indexGrid = new int[width, height];
        for (int r = 0; r < height; r++)
        {
            string row = rows[height - r - 1];
            string[] cols = row.Trim().Split(',');
            for (int c = 0; c < width; c++)
            {
                indexGrid[c, r] = int.Parse(cols[c]);
            }
        }

        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                int tileIndex = indexGrid[i, j];
                if (tileIndex == 0)
                {
                    //the only difference: try to spawn a mine
                    if (Random.Range(0f, 1f) <= _chanceToSpawnMine)
                    {
                        if (localTilePrefabs?.Length > 0)
                        {
                            Debug.Log("Mine spawned.");
                            Tile.spawnTile(localTilePrefabs[0], transform, i, j);
                        }
                    }
                    continue; // 0 is nothing.
                }
                GameObject tileToSpawn;
                if (tileIndex < LevelGenerator.LOCAL_START_INDEX)
                {
                    tileToSpawn = ourGenerator.globalTilePrefabs[tileIndex - 1];
                }
                else
                {
                    tileToSpawn = localTilePrefabs[tileIndex - LevelGenerator.LOCAL_START_INDEX];
                }
                Tile.spawnTile(tileToSpawn, transform, i, j);
            }
        }
    }

    public int[,] loadIndexGrid()
    {
        string initialGridString = designedRoomFile.text;
        string[] rows = initialGridString.Trim().Split('\n');
        int width = rows[0].Trim().Split(',').Length;
        int height = rows.Length;
        if (height != LevelGenerator.ROOM_HEIGHT)
        {
            throw new UnityException(string.Format("Error in room by {0}. Wrong height, Expected: {1}, Got: {2}", roomAuthor, LevelGenerator.ROOM_HEIGHT, height));
        }
        if (width != LevelGenerator.ROOM_WIDTH)
        {
            throw new UnityException(string.Format("Error in room by {0}. Wrong width, Expected: {1}, Got: {2}", roomAuthor, LevelGenerator.ROOM_WIDTH, width));
        }
        int[,] indexGrid = new int[width, height];
        for (int r = 0; r < height; r++)
        {
            string row = rows[height - r - 1];
            string[] cols = row.Trim().Split(',');
            for (int c = 0; c < width; c++)
            {
                indexGrid[c, r] = int.Parse(cols[c]);
            }
        }
        return indexGrid;
    }
}
