using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(WFCOBJController))]
public class WFCScanEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        if(GUILayout.Button("Update Tiles"))
        {
            (target as WFCOBJController).UpdateWFC();
        }
    }
}
