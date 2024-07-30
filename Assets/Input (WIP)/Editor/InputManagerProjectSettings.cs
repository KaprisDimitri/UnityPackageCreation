using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class InputManagerProjectSettings : SettingsProvider
{
    private const string NAME_WINDOW_SETTINGS = "InputManagerSettings";

    private SerializedObject _so_InputManagerSettings;

    class Styles
    {
        public static GUIContent pathInputManagerSettings = new GUIContent("Path SO_Inputs");
    }

    public InputManagerProjectSettings(string path, SettingsScope scope = SettingsScope.User)
        : base(path, scope) { }



    public override void OnGUI(string searchContext)
    {
        // Use IMGUI to display UI:
        EditorGUILayout.PropertyField(_so_InputManagerSettings.FindProperty("pathSOInputs"), Styles.pathInputManagerSettings);
        _so_InputManagerSettings.ApplyModifiedPropertiesWithoutUndo();
    }

    // Register the SettingsProvider, Call when preference / Preject Settings is open
    [SettingsProvider]
    public static SettingsProvider CreateInputManagerSettingsProvider()
    {
        SO_InputManagerSettings.GetOrCreateSettings();

        var provider = new InputManagerProjectSettings("Project/InputSettings/" + NAME_WINDOW_SETTINGS, SettingsScope.Project);

        // Automatically extract all keywords from the Styles.
        provider.keywords = GetSearchKeywordsFromGUIContentProperties<Styles>();
       
        return provider;
    }

    // This function is called when the user clicks on the MyCustom element in the Settings window.
    public override void OnActivate(string searchContext, VisualElement rootElement)
    {
        _so_InputManagerSettings = SO_InputManagerSettings.GetSerializedSettings();
    }
}

