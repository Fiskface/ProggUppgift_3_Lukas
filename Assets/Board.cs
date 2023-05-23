using BoardGame;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Board : BoardParent
{
    // This function is called whenever the board or any tile inside the board
    // is modified.
    public override void SetupBoard() {
        
        // 1. Get the size of the board
        var boardSize = BoardSize;
        
        // 2. Iterate over all tiles
        foreach (Tile tile in Tiles) {
            for (int i = 0; i < tile.transform.childCount; i++)
            {
                tile.transform.GetChild(i).GameObject().SetActive(false);
            }
        }
        
        
        // 3. Find a tile with a particular coordinate
        Vector2Int coordinate = new Vector2Int(2, 1);
        if (TryGetTile(coordinate, out Tile tile2)) {
            
        }
    }
}
