using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TaiyoValidatorRoom : Room
{
    /*
     Constraints
     0 ~ 4 required exits
     
     
     
     */

    private bool _hasUpExit;
    private bool _hasDownExit;
    private bool _hasLeftExit;
    private bool _hasRightExit;

    private bool _hasUpLeftPath;
    private bool _hasUpRightPath;
    private bool _hasUpDownPath;
    private bool _hasRightDownPath;
    private bool _hasRightLeftPath;
    private bool _hasDownLeftPath;

    private bool initialized = false;

    private void ValidateRoom()
    {
        int[,] indexGrid = loadIndexGrid();
        Vector2Int upExit = new Vector2Int(LevelGenerator.ROOM_WIDTH / 2, LevelGenerator.ROOM_HEIGHT - 1);
        Vector2Int downExit = new Vector2Int(LevelGenerator.ROOM_WIDTH / 2, 0);
        Vector2Int leftExit = new Vector2Int(0, LevelGenerator.ROOM_HEIGHT / 2);
        Vector2Int rightExit = new Vector2Int(LevelGenerator.ROOM_WIDTH - 1, LevelGenerator.ROOM_HEIGHT / 2);

        _hasUpExit = IsPointNavigable(upExit,indexGrid);
        _hasLeftExit = IsPointNavigable(leftExit, indexGrid);
        _hasRightExit = IsPointNavigable(rightExit, indexGrid);
        _hasDownExit = IsPointNavigable(downExit, indexGrid);

        _hasUpLeftPath = DoesPathExist(indexGrid, upExit, leftExit);
        _hasUpRightPath = DoesPathExist(indexGrid, rightExit, upExit);
        _hasUpDownPath = DoesPathExist(indexGrid, upExit, downExit);
        _hasRightDownPath = DoesPathExist(indexGrid, rightExit, downExit);
        _hasRightLeftPath = DoesPathExist(indexGrid, rightExit, leftExit);
        _hasDownLeftPath = DoesPathExist(indexGrid, downExit, leftExit);
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

    bool DoesPathExist(int[,] indexGrid, Vector2Int startPoint, Vector2Int endPoint)
    {

        List<Vector2Int> openSet = new List<Vector2Int>();
        List<Vector2Int> closedSet = new List<Vector2Int>();

        if(IsPointNavigable(startPoint,indexGrid))
            openSet.Add(startPoint);


        while(openSet.Count > 0)
        {
            var current = openSet[0];
            openSet.RemoveAt(0);

            if (current == endPoint)
                return true;

            Vector2Int upNeighbor = new Vector2Int(current.x, current.y + 1);

            if (!openSet.Contains(upNeighbor) && !closedSet.Contains(upNeighbor))
            {
                if(IsPointNavigable(upNeighbor,indexGrid))
                    openSet.Add(upNeighbor);
            }


            Vector2Int downNeighbor = new Vector2Int(current.x, current.y - 1);

            if (!openSet.Contains(downNeighbor) && !closedSet.Contains(downNeighbor))
            {
                if (IsPointNavigable(downNeighbor, indexGrid))
                    openSet.Add(downNeighbor);
            }


            Vector2Int leftNeighbor = new Vector2Int(current.x + 1, current.y);

            if (!openSet.Contains(leftNeighbor) && !closedSet.Contains(leftNeighbor))
            {
                if (IsPointNavigable(leftNeighbor, indexGrid))
                    openSet.Add(leftNeighbor);
            }


            Vector2Int rightNeighbor = new Vector2Int(current.x - 1, current.y);

            if (!openSet.Contains(rightNeighbor) && !closedSet.Contains(rightNeighbor))
            {
                if (IsPointNavigable(rightNeighbor, indexGrid))
                    openSet.Add(rightNeighbor);
            }

            closedSet.Add(current);

        }

        return false;
    }

    bool IsPointNavigable(Vector2Int point,int[,] indexGrid)
    {
        if (point.x < 0 || point.y < 0 ||
            point.x >= LevelGenerator.ROOM_WIDTH || point.y >= LevelGenerator.ROOM_HEIGHT)
            return false;

        if (indexGrid[point.x, point.y] == 0)
            return true;
        else
            return false;
    }

    public bool MeetsConstraints(ExitConstraint requiredExits)
    {
        if (!initialized)
        {
            initialized = true;
            ValidateRoom();
        }

        if (requiredExits.upExitRequired && !_hasUpExit)
            return false;
        if (requiredExits.downExitRequired && !_hasDownExit)
            return false;
        if (requiredExits.leftExitRequired && !_hasLeftExit)
            return false;
        if (requiredExits.rightExitRequired && !_hasRightExit)
            return false;

        if (requiredExits.leftExitRequired && requiredExits.upExitRequired
            && !_hasUpLeftPath)
            return false;
        if (requiredExits.leftExitRequired && requiredExits.downExitRequired
    && !_hasDownLeftPath)
            return false;
        if (requiredExits.leftExitRequired && requiredExits.rightExitRequired
    && !_hasRightLeftPath)
            return false;
        if (requiredExits.rightExitRequired && requiredExits.upExitRequired
    && !_hasRightLeftPath)
            return false;
        if (requiredExits.rightExitRequired && requiredExits.downExitRequired
    && !_hasRightDownPath)
            return false;
        if (requiredExits.upExitRequired && requiredExits.downExitRequired
    && !_hasUpDownPath)
            return false;


        return true;
    }
}
