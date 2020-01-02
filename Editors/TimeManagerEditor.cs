using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(TimeManager))]
public class TimeMangerEditor : Editor
{
    public TimeManager timeManger;

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        if (GUILayout.Button("Brighten World"))
        {
            timeManger.BrightenWorld();
        }

        if (GUILayout.Button("Darken World"))
        {
            timeManger.DarkenWorld();
        }
    }

    private void OnEnable()
    {
        timeManger = (TimeManager)target;
    }
}