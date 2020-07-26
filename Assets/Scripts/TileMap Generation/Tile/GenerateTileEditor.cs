using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor (typeof (GenerateMap))]
public class GenerateTileEditor : Editor
{
    public override void OnInspectorGUI()
    {
        GenerateMap mapGen = (GenerateMap)target;

        if (DrawDefaultInspector())
        {
            if (mapGen.autoUpdate)
            {
                mapGen.MapDestroy();
                mapGen.MapGeneration();
            }
        }

        if (GUILayout.Button("GENERATE"))
        {
            mapGen.MapDestroy();
            mapGen.MapGeneration();
            
        }
    }
}
