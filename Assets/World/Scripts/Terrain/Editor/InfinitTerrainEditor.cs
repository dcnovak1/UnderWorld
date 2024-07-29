using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor (typeof (InfinitTerrain))]

public class InfinitTerrainEditor : Editor
{
    public override void OnInspectorGUI()
    {
        InfinitTerrain iT = (InfinitTerrain)target;

        base.DrawDefaultInspector();

        if (GUILayout.Button("Regenerate Chunks"))
        {
            iT.EdiorGenerateChunks();
        }
    }
}
