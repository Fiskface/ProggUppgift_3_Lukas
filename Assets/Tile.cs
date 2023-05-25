using System;
using BoardGame;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Tile : TileParent {
    
    public int lengthFromStart = int.MaxValue;
    public Tile lastTile = null;

    // 1. TileParent extends MonoBehavior, so you can add member variables here
    // to store data.
    public Material regularMaterial;
    public Material reachesMaterial;

    [SerializeField] private GameObject Blocked;
    [SerializeField] private GameObject Start;
    [SerializeField] private GameObject Obstacle;
    [SerializeField] private GameObject Checkpoint;
    [SerializeField] private GameObject Portal;

    

    // This function is called when something has changed on the board. All 
    // tiles have been created before it is called.
    public override void OnSetup(Board board) {
        // 2. Each tile has a unique 'coordinate'
        Vector2Int key = Coordinate;
        
        // 3. Tiles can have different modifiers
        if (IsBlocked) {
            Blocked.SetActive(true);
        }
        
        if (IsObstacle(out int penalty)) {
            Obstacle.SetActive(true);
        }
        
        if (IsCheckPoint) {
            Checkpoint.SetActive(true);
        }
        
        if (IsStartPoint) {
            Start.SetActive(true);
        }
        
        if (IsPortal(out Vector2Int destination)) {
            Portal.SetActive(true);
            GameObject.Find("Board").GetComponent<RenderVectors>().AddPortalVectors(new Vector3(Coordinate.x,0, Coordinate.y), new Vector3(destination.x, 0, destination.y));
        }
        
        // 4. Other tiles can be accessed through the 'board' instance
        if (board.TryGetTile(new Vector2Int(2, 1), out Tile otherTile)) {
            
        }
        
        // 5. Change the material color if this tile is blocked
        if (TryGetComponent<MeshRenderer>(out var meshRenderer))
        {
            meshRenderer.sharedMaterial = regularMaterial;

            if (lengthFromStart <= board.maxStep)
            {
                meshRenderer.sharedMaterial = reachesMaterial;
            }

        }
    }

    // This function is called during the regular 'Update' step, but also gives
    // you access to the 'board' instance.
    public override void OnUpdate(Board board) {
        
    }

    public void ResetValues()
    {
        lengthFromStart = int.MaxValue;
        lastTile = this;
    }
}