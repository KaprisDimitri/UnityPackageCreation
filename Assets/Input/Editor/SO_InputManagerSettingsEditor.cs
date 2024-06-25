using System;
using System.IO;
using System.Reflection;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(SO_InputManagerSettings))]
public class SO_InputManagerSettingsEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        GUILayout.Space(10);

        if (GUILayout.Button("Change SO_Inputs path"))
        {
            SO_InputManagerSettings sO_InputManagerSettings = target as SO_InputManagerSettings;
            Type inputManagerType = typeof(SO_InputManagerSettings);

            FieldInfo pathSOInputsField = inputManagerType.GetField("pathSOInputs", BindingFlags.NonPublic | BindingFlags.Instance);

            string path = (string)pathSOInputsField.GetValue(sO_InputManagerSettings);
            string applicationPath = Application.dataPath.Replace("Assets", "");
             
            string newPath = EditorUtility.OpenFolderPanel("Select Folder", applicationPath + path, "");

            if (Directory.Exists(newPath) && newPath.Contains(Application.dataPath))
            {
                newPath = newPath.Replace(Application.dataPath.Replace("Assets", ""), "") + '/';
                FieldInfo staticFolderPath = inputManagerType.GetField("PathSoInputs", BindingFlags.NonPublic | BindingFlags.Static);
                staticFolderPath.SetValue(null, newPath);
                pathSOInputsField?.SetValue(sO_InputManagerSettings, newPath);
                return; 
            }

            EditorUtility.SetDirty(sO_InputManagerSettings);
        }
    }
}
