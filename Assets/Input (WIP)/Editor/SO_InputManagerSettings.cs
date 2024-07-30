using UnityEditor;
using UnityEngine;

public class SO_InputManagerSettings : ScriptableObject
{
    private const string DEFAULT_PATH_INPUTMANAGER_SETTINGS = "Assets/Input/Editor/SO_InputManagerSettings.asset";

    [SerializeField] private static string PathSoInputs = "";
    [SerializeField, StringReadOnly] private string pathSOInputs = "Assets/Input/SO_Bridge_Input/SO_Input_Data/"; 

    public static string GetPathSOInputs => PathSoInputs;

    internal static SO_InputManagerSettings GetOrCreateSettings()
    {
        var settings = AssetDatabase.LoadAssetAtPath<SO_InputManagerSettings>(DEFAULT_PATH_INPUTMANAGER_SETTINGS);
        PathSoInputs = settings.pathSOInputs;

        if (settings == null)
        {
            settings = CreateInstance<SO_InputManagerSettings>();
            AssetDatabase.CreateAsset(settings, DEFAULT_PATH_INPUTMANAGER_SETTINGS);
            AssetDatabase.SaveAssets();
        }
        return settings;
    }

    internal static SerializedObject GetSerializedSettings()
    {
        return new SerializedObject(GetOrCreateSettings());
    }
}
