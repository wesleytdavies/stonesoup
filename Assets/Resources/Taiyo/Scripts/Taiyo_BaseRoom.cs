using System.Collections;
using System.Collections.Generic;

using UnityEngine;
public class Taiyo_BaseRoom : Room
{

    [Header("PUT IN ROOM FILES HERE")] public TextAsset[] roomFiles;

    private int[,] _dfsMap;

    private bool _initialSetup = false;

    private roomData[] _roomDatas;

    struct roomData
    {
        public bool hasLeftRightPath;
        public bool hasLeftUpPath;
        public bool hasLeftDownPath;
        public bool hasRightUpPath;
        public bool hasRightDownPath;
        public bool hasUpDownPath;
        public int[,] grid;
    }


    public void SetupDatas()
    {
        _initialSetup = true;

        //SET UP THE DATAS
        _roomDatas = new roomData[roomFiles.Length];

        var l = new Vector2Int(0, LevelGenerator.ROOM_HEIGHT / 2);
        var r = new Vector2Int(LevelGenerator.ROOM_WIDTH - 1, LevelGenerator.ROOM_HEIGHT / 2);
        var d = new Vector2Int(LevelGenerator.ROOM_WIDTH / 2, 0);
        var u = new Vector2Int(LevelGenerator.ROOM_WIDTH / 2, LevelGenerator.ROOM_HEIGHT - 1);

        for (int i = 0; i < _roomDatas.Length; i++)
        {
            _roomDatas[i].hasLeftRightPath = performDFS(GetRoom(i), l, r);
            _roomDatas[i].hasLeftUpPath = performDFS(GetRoom(i), l, u);
            _roomDatas[i].hasLeftDownPath = performDFS(GetRoom(i), l, d);
            _roomDatas[i].hasRightUpPath = performDFS(GetRoom(i), r, u);
            _roomDatas[i].hasRightDownPath = performDFS(GetRoom(i), r, d);
            _roomDatas[i].hasUpDownPath = performDFS(GetRoom(i), u, d);
            _roomDatas[i].grid = GetRoom(i);
        }
    }

    public int[,] GetRandomRoomWithConstraints(ExitConstraint requiredExits)
    {
        if (!_initialSetup)
            SetupDatas();

        int result = 0;
        bool l = requiredExits.leftExitRequired;
        bool r = requiredExits.rightExitRequired;
        bool u = requiredExits.upExitRequired;
        bool d = requiredExits.downExitRequired;

        //return a random grid when there is no required exits
        if (!l && !r && !u && !d)
            return _roomDatas[Random.Range(0, _roomDatas.Length)].grid;

        int count = 0;
        while (true)
        {
            count++;
            if (count > 200)
            {
                Debug.Log("CANNOT FIND A SUFFICIENT ROOMDATA");
                break;
            }
            result = Random.Range(0, _roomDatas.Length);

            if (l && u && _roomDatas[result].hasLeftUpPath)
                break;
            if (l && d && _roomDatas[result].hasLeftDownPath)
                break;
            if (l && r && _roomDatas[result].hasLeftRightPath)
                break;
            if (r && u && _roomDatas[result].hasRightUpPath)
                break;
            if (r && d && _roomDatas[result].hasRightDownPath)
                break;
            if (d && u && _roomDatas[result].hasUpDownPath)
                break;
        }

        return _roomDatas[result].grid;
    }

    private bool performDFS(int[,] indexGrid, Vector2Int startPos, Vector2Int endPos)
    {
        //If the start position is a wall then it is false no matter what
        if (indexGrid[startPos.x, startPos.y] == 1)
            return false;

        //If the end position is a wall then it is false no matter what
        if (indexGrid[endPos.x, endPos.y] == 1)
            return false;

        _dfsMap = indexGrid;

        recDFS(startPos);

        return (_dfsMap[endPos.x, endPos.y] == -1);
    }


    private void recDFS(Vector2Int posNow)
    {
        //If we visit, we put -1 in the map
        _dfsMap[posNow.x, posNow.y] = -1;

        //Visit UP
        if (posNow.y + 1 < LevelGenerator.ROOM_HEIGHT && _dfsMap[posNow.x, posNow.y + 1] != -1 && _dfsMap[posNow.x, posNow.y + 1] != 1)
            recDFS(new Vector2Int(posNow.x, posNow.y + 1));
        //Visit DOWN
        if (posNow.y - 1 >= 0 && _dfsMap[posNow.x, posNow.y - 1] != -1 && _dfsMap[posNow.x, posNow.y - 1] != 1)
            recDFS(new Vector2Int(posNow.x, posNow.y - 1));

        //Visit LEFT
        if (posNow.x - 1 >= 0 && _dfsMap[posNow.x - 1, posNow.y] != -1 && _dfsMap[posNow.x - 1, posNow.y] != 1)
            recDFS(new Vector2Int(posNow.x - 1, posNow.y));

        //Visit RIGHT
        if (posNow.x + 1 < LevelGenerator.ROOM_WIDTH && _dfsMap[posNow.x + 1, posNow.y] != -1 && _dfsMap[posNow.x + 1, posNow.y] != 1)
            recDFS(new Vector2Int(posNow.x + 1, posNow.y));
    }



    public override void fillRoom(LevelGenerator ourGenerator, ExitConstraint requiredExits)
    {

        //Get a random room from the roomFiles meeting the constraints
        int[,] indexGrid = GetRandomRoomWithConstraints(requiredExits);

        //Generate stuff
        for (int x = 0; x < LevelGenerator.ROOM_WIDTH; x++)
        {
            for (int y = 0; y < LevelGenerator.ROOM_HEIGHT; y++)
            {
                if (indexGrid[x, y] != 0)
                    Tile.spawnTile(this.localTilePrefabs[indexGrid[x, y] - 1], transform, x, y);
            }
        }
    }


    public int[,] GetRoom(int roomNum)
    {
        //Get a random room from the roomFiles
        string initialGridString = roomFiles[roomNum].text;
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

