using System.Collections;
using System.Collections.Generic;

using UnityEngine;
public class TaiyoRoom : Room
{

    [Header("DONT USE designedRoomFiles")] public TextAsset[] roomFiles;

    private int _roomCount = 0;

    [Header("Percentage of creating an item")] public float spawnItem = 0.02f;

    [Header("Items to spawn randomly")] public GameObject[] itemPrefabs;


    //MAP FOR DFS
    private int[,] dfsMap;

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
        if(_initialSetup == false)
        {
            _initialSetup = true;
            SetupDatas();
        }


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
            if(count > 200)
            {
                Debug.Log("l : " + l);
                Debug.Log("r : " + r);
                Debug.Log("d : " + d);
                Debug.Log("u : " + u);
                Debug.Log("ñ≥å¿ÉãÅ[Év");
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

        dfsMap = indexGrid;

        recDFS(startPos);

        return (dfsMap[endPos.x,endPos.y] == -1);
    }

    private void recDFS(Vector2Int posNow)
    {
        //If we visit, we put -1 in the map
        dfsMap[posNow.x, posNow.y] = -1;

        //Visit UP
        if (posNow.y + 1 < LevelGenerator.ROOM_HEIGHT && dfsMap[posNow.x, posNow.y + 1] != -1 && dfsMap[posNow.x, posNow.y + 1] != 1)
        {
            recDFS(new Vector2Int(posNow.x, posNow.y + 1));
        }

        //Visit DOWN
        if (posNow.y - 1 >= 0 && dfsMap[posNow.x, posNow.y - 1] != -1 && dfsMap[posNow.x, posNow.y - 1] != 1)
        {
            recDFS(new Vector2Int(posNow.x, posNow.y - 1));
        }

        //Visit LEFT
        if (posNow.x - 1 >= 0 && dfsMap[posNow.x-1, posNow.y] != -1 && dfsMap[posNow.x-1, posNow.y] != 1)
        {
            recDFS(new Vector2Int(posNow.x-1, posNow.y));
        }

        //Visit RIGHT
        if (posNow.x + 1 < LevelGenerator.ROOM_WIDTH && dfsMap[posNow.x + 1, posNow.y] != -1 && dfsMap[posNow.x + 1, posNow.y] != 1)
        {
            recDFS(new Vector2Int(posNow.x + 1, posNow.y));
        }
        
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
                if(indexGrid[x,y] != 0)
                    Tile.spawnTile(this.localTilePrefabs[indexGrid[x, y] - 1], transform, x, y);
                else
                {
                    //Randomly spawn an item
                    if(Random.value < spawnItem)
                    {
                        Tile.spawnTile(itemPrefabs[Random.Range(0,itemPrefabs.Length)], transform, x, y);
                    }
                }
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


    public int[,] GetRandomRoom()
    {
        //Get a random room from the roomFiles
        string initialGridString = roomFiles[UnityEngine.Random.Range(0, roomFiles.Length)].text;
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



/*
 bool createWall = true;

                if (x != 0 && x != LevelGenerator.ROOM_WIDTH - 1
                    && y != 0 && y != LevelGenerator.ROOM_HEIGHT - 1)
                    createWall = false;

                if (requiredExits.downExitRequired && (x == 4 || x == 5) && y == 0)
                    createWall = false;

                if (requiredExits.leftExitRequired && (y == 4 || y == 5) && x == 0)
                    createWall = false;

                if (requiredExits.rightExitRequired && (y == 4 || y == 5) && x == LevelGenerator.ROOM_WIDTH-1)
                    createWall = false;

                if (requiredExits.upExitRequired && (x == 4 || x == 5) && y == LevelGenerator.ROOM_HEIGHT-1)
                    createWall = false;

                if (!createWall && indexGrid[x,y] != 0)
                {
                    if (UnityEngine.Random.value < spawnItem)
                    {
                        //Spawn Item
                        Tile.spawnTile(this.itemPrefabs[UnityEngine.Random.Range(0, itemPrefabs.Length)], transform, x, y);
                    }
                    else
                    {
                        //Create tile based off the text file
                        

                        if (o.GetComponent<BasicAICreature>() != null)
                            o.GetComponent<BasicAICreature>().init();
                    }
                }

                if (dontGenerateLeft && x == 0)
                    createWall = false;
                if (dontGenerateRight && x == LevelGenerator.ROOM_WIDTH - 1)
                    createWall = false;
                if (dontGenerateUp && y == LevelGenerator.ROOM_HEIGHT - 1)
                    createWall = false;
                if (dontGenerateDown && y == 0)
                    createWall = false;

                if (createWall && UnityEngine.Random.value > wallBroken)
                    Tile.spawnTile(ourGenerator.normalWallPrefab, transform, x, y);

     */
