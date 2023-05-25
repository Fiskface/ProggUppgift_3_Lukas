using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Vectors;

[ExecuteAlways]
[RequireComponent(typeof(VectorRenderer))]
public class RenderVectors : MonoBehaviour {
    
    [NonSerialized] 
    private VectorRenderer vectors;

    private List<List<Vector3>> portalVectorsToDraw = new List<List<Vector3>>();
    private List<List<Vector3>> tileVectorsToDraw = new List<List<Vector3>>();

    void OnEnable() {
        vectors = GetComponent<VectorRenderer>();
    }

    void Update()
    {
        using (vectors.Begin())
        {
            foreach (var pair in portalVectorsToDraw)
            {
                vectors.Draw(pair[0], pair[1], new Color(0.533f, 0, 1));
            }
            foreach (var pair in tileVectorsToDraw)
            {
                vectors.Draw(pair[0], pair[1], Color.green);
            }
        }
    }

    public void AddPortalVectors(Vector3 start, Vector3 end)
    {
        start += new Vector3(-0.03f, 0.1f, 0);
        end += new Vector3(0, 0.1f, 0);
        var diff = start - end;
        diff *= 0.5f;
        var mid = start - diff + Vector3.up;

        List<Vector3> vectorPair1 = new List<Vector3>();
        vectorPair1.Add(start);
        vectorPair1.Add(mid);
        portalVectorsToDraw.Add(vectorPair1);
        List<Vector3> vectorPair2 = new List<Vector3>();
        vectorPair2.Add(mid);
        vectorPair2.Add(end);
        portalVectorsToDraw.Add(vectorPair2);
    }

    public void AddTileVectors(Tile tile)
    {
        List<Vector3> vectorPair = new List<Vector3>();
        vectorPair.Add(new Vector3(tile.lastTile.Coordinate.x, 0.1f, tile.lastTile.Coordinate.y));
        vectorPair.Add(new Vector3(tile.Coordinate.x, 0.1f, tile.Coordinate.y));
        tileVectorsToDraw.Add(vectorPair);
    }

    public void ClearVectorsToDraw()
    {
        portalVectorsToDraw.Clear();
        tileVectorsToDraw.Clear();
    }
}
