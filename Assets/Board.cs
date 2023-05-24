using System.Collections.Generic;
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

    private bool AllCheckpointsReached => checkpointsReached >= checkpoints.Count;
    
    

    // This function is called whenever the board or any tile inside the board
    // is modified.
    public override void SetupBoard() {
        ResetVariables();
        
        // 1. Get the size of the board
        var boardSize = BoardSize;
        
        // 2. Iterate over all tiles
        foreach (Tile tile in Tiles) {
            tile.ResetValues();
            if (tile.IsCheckPoint)
                checkpoints.Add(tile);
            for (int i = 0; i < tile.transform.childCount; i++)
            {
                tile.transform.GetChild(i).GameObject().SetActive(false);
            }

            if (tile.IsStartPoint)
            {
                startTile = tile;
                tile.lastTile = tile;
                tile.lengthFromStart = 0;
            }
        }

        
        while (true)
        {
            CheckNeighbours(startTile);
            
            if (seen.Count == 0)
            {
                if(!AllCheckpointsReached)
                    Debug.Log("All checkpoints couldn't be reached!");
                break;
            }

            if (maxStepReached && AllCheckpointsReached)
            {
                break;
            }
            
            CheckNeighbours(ShortestInSeen());
            
            //TODO: Check which in seen that has shortest distance, make it visited, checkneighbours from it. 
            //When adding to visited, if it is a checkpoint, checkpointsreached++
            //when the seen with shortest lengthfromstart is over maxStep, set maxStepReached to true;

            //TODO: Draw vectors back to start from checkpoints
        }
        
        // 3. Find a tile with a particular coordinate
        Vector2Int coordinate = new Vector2Int(2, 1);
        if (TryGetTile(coordinate, out Tile tile2)) {
            
        }
    }

    private void CheckNeighbours(Tile tile)
    {
        Vector2Int north = new Vector2Int(tile.Coordinate.x, tile.Coordinate.y + 1);
        Vector2Int east = new Vector2Int(tile.Coordinate.x + 1, tile.Coordinate.y);
        Vector2Int south = new Vector2Int(tile.Coordinate.x, tile.Coordinate.y - 1);
        Vector2Int west = new Vector2Int(tile.Coordinate.x - 1, tile.Coordinate.y);
        HashSet<Vector2Int> positions = new HashSet<Vector2Int>() { north, east, south, west};
        
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

    private Tile ShortestInSeen()
    {
        Tile shortest = null;

        foreach (var tile in seen)
        {
            if (shortest == null)
                shortest = tile;
            if (tile.lengthFromStart < shortest.lengthFromStart)
                shortest = tile;
        }

        //if (shortest.lengthFromStart > maxStep)
        //    maxStepReached = true;

        seen.Remove(shortest);
        visited.Add(shortest);
        if (shortest.IsCheckPoint)
            checkpointsReached++;
        
        return shortest;
    }

    private void ResetVariables()
    {
        GetComponent<RenderVectors>().ClearVectorsToDraw();
        seen.Clear();
        visited.Clear();
        checkpoints.Clear();
        maxStepReached = false;
        checkpointsReached = 0;
    }
}
