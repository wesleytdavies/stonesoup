using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JadenValidatedRoom : Room
{
    bool _hasExitUp, _hasExitDown, _hasExitLeft, _hasExitRight;

    bool _pathUpLeft, _pathUpRight, _pathDownLeft, _pathDownRight, _pathRightLeft, _pathUpDown;

    void Start() {
        ValidateRoom();
    }

    private void ValidateRoom() {

        int[,] indexGrid = loadIndexGrid();
        Vector2Int upExit = new Vector2Int(LevelGenerator.ROOM_WIDTH / 2, LevelGenerator.ROOM_HEIGHT);
        Vector2Int downExit = new Vector2Int(LevelGenerator.ROOM_WIDTH / 2, 0);
        Vector2Int leftExit = new Vector2Int(0, LevelGenerator.ROOM_HEIGHT / 2);
        Vector2Int rightExit = new Vector2Int(LevelGenerator.ROOM_WIDTH, LevelGenerator.ROOM_HEIGHT / 2);

        _hasExitUp = IsPointNavigable(indexGrid, upExit);
        _hasExitDown = IsPointNavigable(indexGrid, downExit);
        _hasExitLeft = IsPointNavigable(indexGrid, leftExit);
        _hasExitRight = IsPointNavigable(indexGrid, rightExit);

        _pathUpLeft = DoesPathExist(indexGrid, upExit, leftExit);
        _pathUpRight = DoesPathExist(indexGrid, upExit, rightExit);
        _pathDownLeft = DoesPathExist(indexGrid, downExit, leftExit);
        _pathDownRight = DoesPathExist(indexGrid, downExit, rightExit);
        _pathRightLeft = DoesPathExist(indexGrid, rightExit, leftExit);
        _pathUpDown = DoesPathExist(indexGrid, upExit, downExit);

    }

    public bool MeetsConstraints(ExitConstraint requiredExits) {

        if((requiredExits.upExitRequired && !_hasExitUp) ||
            (requiredExits.downExitRequired && !_hasExitDown) ||
            (requiredExits.leftExitRequired && !_hasExitLeft) ||
            (requiredExits.rightExitRequired && !_hasExitRight)) {
            return false;
        }
        
        if((!_pathUpLeft && requiredExits.upExitRequired && requiredExits.leftExitRequired) ||
            (!_pathDownLeft && requiredExits.downExitRequired && requiredExits.leftExitRequired) ||
            (!_pathUpRight && requiredExits.upExitRequired && requiredExits.rightExitRequired) ||
            (!_pathDownRight && requiredExits.downExitRequired && requiredExits.rightExitRequired) ||
            (!_pathUpDown && requiredExits.upExitRequired && requiredExits.downExitRequired) ||
            (!_pathRightLeft && requiredExits.rightExitRequired && requiredExits.leftExitRequired)) {
            return false;
        }

        return true;
        
    }

    //takes designed room text file and parses
    public int[,] loadIndexGrid() {
        string initialGridString = designedRoomFile.text;
        string[] rows = initialGridString.Trim().Split('\n');
        int width = rows[0].Trim().Split(',').Length;
        int height = rows.Length;
        if (height != LevelGenerator.ROOM_HEIGHT) {
            throw new UnityException(string.Format("Error in room by {0}. Wrong height, Expected: {1}, Got: {2}", roomAuthor, LevelGenerator.ROOM_HEIGHT, height));
        }
        if (width != LevelGenerator.ROOM_WIDTH) {
            throw new UnityException(string.Format("Error in room by {0}. Wrong width, Expected: {1}, Got: {2}", roomAuthor, LevelGenerator.ROOM_WIDTH, width));
        }
        int[,] indexGrid = new int[width, height];
        for (int r = 0; r < height; r++) {
            string row = rows[height - r - 1];
            string[] cols = row.Trim().Split(',');
            for (int c = 0; c < width; c++) {
                indexGrid[c, r] = int.Parse(cols[c]);
            }
        }
        return indexGrid;
    }

//breadth first search 
    bool DoesPathExist(int[,] indexGrid, Vector2Int startPoint, Vector2Int endPoint) {
        List<Vector2Int> openSet = new List<Vector2Int>();
        List<Vector2Int> closedSet = new List<Vector2Int>();

        if(IsPointNavigable(indexGrid, startPoint)) {
            openSet.Add(startPoint);
            //if not nav, skips entire loop 
        }

        openSet.Add(startPoint);

        while(openSet.Count > 0) {
            Vector2Int currentPoint = openSet[0];
            openSet.RemoveAt(0);

            if(currentPoint == endPoint)
                return true;
            
            
            //make sure this isn't already in 
            //alternate implementation: other data structure, flag as visited- higher performance
            //cause, Contains is high overhead 
            //this implementation is clarity > performance 
            Vector2Int upNeighbor = new Vector2Int(currentPoint.x, currentPoint.y + 1);
            tryAdd(upNeighbor, openSet, closedSet, indexGrid);
            Vector2Int downNeighbor = new Vector2Int(currentPoint.x, currentPoint.y - 1);
            tryAdd(downNeighbor, openSet, closedSet, indexGrid);
            Vector2Int leftNeighbor = new Vector2Int(currentPoint.x - 1, currentPoint.y);
            tryAdd(leftNeighbor, openSet, closedSet, indexGrid);
            Vector2Int rightNeighbor = new Vector2Int(currentPoint.x + 1, currentPoint.y);
            tryAdd(rightNeighbor, openSet, closedSet, indexGrid);

            closedSet.Add(currentPoint);

        }
        return false; //no path 
    }
    void tryAdd(Vector2Int neighbor, List<Vector2Int> openSet, List<Vector2Int> closedSet, int[,] indexGrid) {
        if(openSet.Contains(neighbor) == false && closedSet.Contains(neighbor) == false) {
                if(IsPointNavigable(indexGrid, neighbor))
                    openSet.Add(neighbor);
        }
    }

    bool IsPointNavigable(int[,] indexGrid, Vector2Int point) {

        if(!IsPointInGrid(point)) {
            return false;
        }

        if(indexGrid[point.x, point.y] == 1) {
            return false;
        } else {
            return true;
        }

    }

    bool IsPointInGrid(Vector2Int point) {
        if((point.x < 0 || point.x >= LevelGenerator.ROOM_WIDTH) ||
        (point.y < 0 || point.y >= LevelGenerator.ROOM_HEIGHT))
            return false;
        return true;
    }





    class SearchVertex {
        public Vector2Int point;
        public SearchVertex parent;
    }

    //depth first search  and  returns the path it found 
    //for depth first, it's the same agorithm but the list of nodes is a stack instead of a queue. 
    //returns the path it found. 
    //"get depth first search path" 
    List<Vector2Int> GetDFSPath(int[,] indexGrid, Vector2Int startPoint, Vector2Int endPoint) {
        List<SearchVertex> openSet = new List<SearchVertex>();
        List<SearchVertex> closedSet = new List<SearchVertex>();
        
        if(IsPointNavigable(indexGrid, startPoint)) {
            SearchVertex startVertex = new SearchVertex();
            startVertex.point = startPoint;
            startVertex.parent = null;
            openSet.Add(startVertex);
            //if not nav, skips entire loop 
        }

        while(openSet.Count > 0) {
            
            //literally the only change to make it depth first is the index used in this chunk of code. 
            int index = openSet.Count - 1;
            SearchVertex currentVertex = openSet[index];
            openSet.RemoveAt(index);

            if(currentVertex.point == endPoint) {
                List<Vector2Int> path = new List<Vector2Int>();
                while(currentVertex != null) {
                    path.Add(currentVertex.point);
                }
                path.Reverse();
                return path; 
            }
            
            
            //make sure this isn't already in 
            //alternate implementation: other data structure, flag as visited- higher performance
            //cause, Contains is high overhead 
            //this implementation is clarity > performance 
            Vector2Int upNeighbor = new Vector2Int(currentVertex.point.x, currentVertex.point.y + 1);
            tryAdd(upNeighbor, openSet, closedSet, indexGrid, currentVertex);
            Vector2Int downNeighbor = new Vector2Int(currentVertex.point.x, currentVertex.point.y - 1);
            tryAdd(downNeighbor, openSet, closedSet, indexGrid, currentVertex);
            Vector2Int leftNeighbor = new Vector2Int(currentVertex.point.x - 1, currentVertex.point.y);
            tryAdd(leftNeighbor, openSet, closedSet, indexGrid, currentVertex);
            Vector2Int rightNeighbor = new Vector2Int(currentVertex.point.x + 1, currentVertex.point.y);
            tryAdd(rightNeighbor, openSet, closedSet, indexGrid, currentVertex);

            closedSet.Add(currentVertex);

        }

        return new List<Vector2Int>(); //no path. could return null or an empty list 
            //returning an empty list means anything can work with the output w/o checking null 
    }

    bool DoesListContainPoint(List<SearchVertex> list, Vector2Int point) {
        //lambda way: 
        //openSet.Exists(x=>x.point==upNeighbor)
        //"for each value of x, treat it as x.point, do this comparison" 
        //so comparing against child data instead of data itself 
        //getting at data inside of class 

        //conventional way 
        foreach(SearchVertex searchVertex in list) {
            if(searchVertex.point == point) {
                return true;
            }
        }

        return false;

    }

    void tryAdd(Vector2Int neighbor, List<SearchVertex> openSet, List<SearchVertex> closedSet, int[,] indexGrid, SearchVertex currentVertex) {
        if(DoesListContainPoint(openSet, neighbor) == false && DoesListContainPoint(closedSet, neighbor) == false) {
            if(IsPointNavigable(indexGrid, neighbor)) {
                SearchVertex neighborVertex = new SearchVertex();
                neighborVertex.point = neighbor;
                neighborVertex.parent = currentVertex;
                openSet.Add(neighborVertex);
            }                
        }
    }

}
