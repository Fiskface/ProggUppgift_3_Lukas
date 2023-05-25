using System.Collections.Generic;
using System.Reflection;
using BoardGame;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Board : BoardParent
{
    public Tile startTile;
    private HashSet<Tile> seen = new HashSet<Tile>();
    private HashSet<Tile> visited = new HashSet<Tile>();
    private HashSet<Tile> checkpoints = new HashSet<Tile>();
    public int maxStep = 5;
    private bool maxStepReached = false;
    private int checkpointsReached = 0;
    private int lastMaxStep = 5;

    private RenderVectors RV;

    private bool AllCheckpointsReached => checkpointsReached == checkpoints.Count;

    private void Update()
    {
        if (lastMaxStep != maxStep)
        {
            SetupBoard();
            foreach (var tile in Tiles) {
                tile.OnSetup(this);
            }

            lastMaxStep = maxStep;
        }
    }

    // This function is called whenever the board or any tile inside the board
    // is modified.
    public override void SetupBoard()
    {
        RV = GetComponent<RenderVectors>();
        ResetVariables();
        
        
        //Clears the console, taken from https://stackoverflow.com/questions/40577412/clear-editor-console-logs-from-script
        var assembly = Assembly.GetAssembly(typeof(UnityEditor.Editor));
        var type = assembly.GetType("UnityEditor.LogEntries");
        var method = type.GetMethod("Clear");
        method.Invoke(new object(), null);
        
        
        // 1. Get the size of the board
        var boardSize = BoardSize;
        
        // 2. Iterate over all tiles
        foreach (Tile tile in Tiles) {
            tile.ResetValues();     
            if (tile.IsCheckPoint)
                checkpoints.Add(tile);
            
            //Sets all children which indicates which states they are to inactive, tile fixes which should be seen later
            for (int i = 0; i < tile.transform.childCount; i++)
            {
                tile.transform.GetChild(i).GameObject().SetActive(false);
            }

            //Sets values for the startpoint
            if (tile.IsStartPoint)
            {
                startTile = tile;
                tile.lastTile = tile;
                tile.lengthFromStart = 0;
            }
            
        }
        
        if(startTile != null)
            CheckNeighbours(startTile);
        
        while (true)
        {
            //Break conditions for the while loop
            if (seen.Count == 0)
            {
                //If there are checkpoints that aren't reached when all tiles have been looked through it states that
                //To see which you have to look at which checkpoints have arrows to them and which have not. 
                if(!AllCheckpointsReached)
                    Debug.Log("All checkpoints couldn't be reached!");
                break;
            }

            //Doesn't need to check more tiles. 
            if (maxStepReached && AllCheckpointsReached)
            {
                break;
            }
            
            //Keeps checking neighbours to the tile in seen that has shortest lenght from start
            CheckNeighbours(ShortestInSeen());
            
        }

        //Draws arrows from start to checkpoints that can be reached
        foreach (var tile in checkpoints)
        {
            DrawFromStart(tile);
        }
        
    }

    //Checks all neighbours to the tile, if they aren't blocked it fixes values such as lengthFromStart
    //Portaldestinations are neighbours, if they are valid and haven't been locked (put in visited) they are
    //put in seen to be used later.
    private void CheckNeighbours(Tile tile)
    {
        Vector2Int north = new Vector2Int(tile.Coordinate.x, tile.Coordinate.y + 1);
        Vector2Int east = new Vector2Int(tile.Coordinate.x + 1, tile.Coordinate.y);
        Vector2Int south = new Vector2Int(tile.Coordinate.x, tile.Coordinate.y - 1);
        Vector2Int west = new Vector2Int(tile.Coordinate.x - 1, tile.Coordinate.y);
        HashSet<Vector2Int> positions = new HashSet<Vector2Int>() {north, east, south, west};
        
        if (tile.IsPortal(out Vector2Int destination))
        {
            positions.Add(destination);
        }

        foreach (var pos in positions)
        {
            if (TryGetTile(pos, out Tile tile2))
            {
                if (!tile2.IsBlocked && !visited.Contains(tile2))
                {
                    int length;
                    if (tile.IsObstacle(out int penalty))
                    {
                         length = penalty + tile.lengthFromStart;
                    }
                    else
                    {
                        length = tile.lengthFromStart + 1;
                    }

                    if (length < tile2.lengthFromStart)
                    {
                        tile2.lengthFromStart = length;
                        tile2.lastTile = tile;
                    }

                    seen.Add(tile2);
                }
            }
        }
    }

     //Looks through all tiles in the seen hashset and returns the first one it finds with the lowest value on lengthFromStart
     //This function also assumes that that tile will be used in combination with CheckNeighbours and therefore updates seen and visited hashsets
     //Should only be called if seen has at least 1 tile in it. 
     private Tile ShortestInSeen()
    {
        Tile shortest = null;
        
        foreach (var tile in seen)
        {
            if (shortest == null)
                shortest = tile;
            else if (tile.lengthFromStart < shortest.lengthFromStart)
                shortest = tile;
        }

        if (shortest.lengthFromStart > maxStep)
            maxStepReached = true;

        seen.Remove(shortest);
        visited.Add(shortest);
        if (shortest.IsCheckPoint)
            checkpointsReached++;
        
        return shortest;
    }

    //Draws vectors for each step in the shortest path from start to the tile
    private void DrawFromStart(Tile tile)
    {
        if (tile.lastTile != tile)
        {
            RV.AddTileVectors(tile);
            DrawFromStart(tile.lastTile);
        }
    }

    private void ResetVariables()
    {
        RV.ClearVectorsToDraw();
        seen.Clear();
        visited.Clear();
        checkpoints.Clear();
        maxStepReached = false;
        checkpointsReached = 0;
        startTile = null;
    }
}
