using UnityEditor;
using UnityEngine;

public class StringReadOnlyAttribute : PropertyAttribute
{
}
 

[CustomPropertyDrawer(typeof(StringReadOnlyAttribute))]
public class LabelDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        GUI.backgroundColor = Color.white;

        // D�sactiver l'�dition de GUI
        GUI.enabled = false;
        
        EditorGUI.PropertyField(position, property, label);
    }
}
