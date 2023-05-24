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

    private List<List<Vector3>> vectorsToDraw = new List<List<Vector3>>();

    void OnEnable() {
        vectors = GetComponent<VectorRenderer>();
    }

    void Update()
    {
        using (vectors.Begin())
        {
            foreach (var pair in vectorsToDraw)
            {
                vectors.Draw(pair[0], pair[1], new Color(0.533f, 0, 1));
            }
            //vectors.Draw(Vector3.zero, vectorA, Color.red);
        }
    }

    public void AddPortalVectors(Vector3 start, Vector3 end)
    {
        start += new Vector3(-0.2f, 0.1f, 0);
        end += new Vector3(0, 0.1f, 0);
        var diff = start - end;
        diff *= 0.5f;
        var mid = start - diff + Vector3.up;

        List<Vector3> vectorPair1 = new List<Vector3>();
        vectorPair1.Add(start);
        vectorPair1.Add(mid);
        vectorsToDraw.Add(vectorPair1);
        List<Vector3> vectorPair2 = new List<Vector3>();
        vectorPair2.Add(mid);
        vectorPair2.Add(end);
        vectorsToDraw.Add(vectorPair2);
    }

    public void ClearVectorsToDraw()
    {
        vectorsToDraw.Clear();
    }
}

/*
[CustomEditor(typeof(RenderVectors))]
public class ExampleGUI : Editor {
    void OnSceneGUI() {
        var ex = target as RenderVectors;
        if (ex == null) return;

        EditorGUI.BeginChangeCheck();
        var a = Handles.PositionHandle(ex.vectorA, Quaternion.identity);
        var b = Handles.PositionHandle(ex.vectorB, Quaternion.identity);
        var c = Handles.PositionHandle(ex.vectorC, Quaternion.identity);

        if (EditorGUI.EndChangeCheck()) {
            Undo.RecordObject(target, "Vector Positions");
            ex.vectorA = a;
            ex.vectorB = b;
            ex.vectorC = c;
            EditorUtility.SetDirty(target);
        }
    }
}
*/