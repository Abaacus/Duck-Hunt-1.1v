using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Dog))]
public class DogEditor : Editor
{
    public Dog dog;

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        if (GUILayout.Button("Play HappyDog"))
        {
            dog.HappyDog();
        }

        if (GUILayout.Button("Play WalkDog"))
        {
            dog.HuntingDog();
        }

        if (GUILayout.Button("Play JumpingDog"))
        {
            dog.JumpingDog();
        }

        if (GUILayout.Button("Start PacingDog"))
        {
            dog.StartPacingDog();
        }

        if (GUILayout.Button("Stop PacingDog"))
        {
            dog.StopPacingDog();
        }
    }

    private void OnEnable()
    {
        dog = (Dog)target;
    }
}