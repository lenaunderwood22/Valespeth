using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(PathVisualGenerator))]
public class PathVisualEditor : Editor {

    public override void OnInspectorGUI () {
        base.OnInspectorGUI();

        if (GUILayout.Button("Generate Line")) {
            var tar = (PathVisualGenerator) target;

            tar.GenerateLine();
        }
    }

}