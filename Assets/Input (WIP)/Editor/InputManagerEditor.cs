using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEditor;
using System.IO;
using static GameActionsMapsNames;
using System.Linq;
using System.Text.RegularExpressions;

[CustomEditor(typeof(InputManager))]
public class InputManagerEditor : Editor
{
    InputManager inputManager;
    private const string ALINEA = "    ";
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        GUILayout.Space(10);

        if(inputManager == null)
        {
            inputManager = target as InputManager;
            //Verifier si l enum n est pas la bonne
            GenerateEnumInputActionV2();
        }

         
        if (GUILayout.Button("Find / categorize / Initialize SO_Input_Base assets"))
        {
            GenerateScriptableObjectV4();
        }
    }

    private void GetInsertsLine (List<string> fileLine, out int indexToInsetAction,out int indexToInsetActionMap)
    {
        indexToInsetAction = -1;
        indexToInsetActionMap = -1;

        for (int l = 0; l < fileLine.Count; l++)
        {
            if (fileLine[l].Contains("GameActionInsertLine")) { indexToInsetAction = l; }
            if (fileLine[l].Contains("GameActionMapInsertLine")) { indexToInsetActionMap = l; }
            if (indexToInsetAction != -1 && indexToInsetActionMap != -1) { break; }
        }

    }

    private bool GetFileLines (string filePath, out List<string> lines)
    {
        lines = null;
        if (!File.Exists(filePath))
        {
            Debug.Log("The file GameActionsMapsNames does not exist it s will be create at this path: " + filePath);
            return false;
        }

        lines = File.ReadAllLines(filePath).ToList();
        return true;
    }

    private void GetAllEnumValue (out List<string> gameActionMapsEnumString, out List<string> gameActionEnumString) 
    {
        gameActionMapsEnumString = new List<string>();

        for (int i = 0; i < Enum.GetValues(typeof(GameActionsMapsEnum)).Length; i++)
        {
            gameActionMapsEnumString.Add(((GameActionsMapsEnum)i).ToString());
        }

        gameActionEnumString = new List<string>();

        for (int i = 0; i < Enum.GetValues(typeof(GameActionsEnum)).Length; i++)
        {
            gameActionEnumString.Add(((GameActionsEnum)i).ToString());
        }
    }

    private bool CheckIfEnumExist(ref List<string> gameEnumString, string actionID)
    {
        if (gameEnumString.Contains(actionID))
        {
            gameEnumString.Remove(actionID);
            return true;
        }
        Debug.Log(actionID + " n est pas contenue dans les enum");
        return false;
    }

    private void RemoveActionsEnumLines (ref List<string> fileLines, List<string> actionEnumLeft)
    {
        List<string> actionEnumLeftCopy = new List<string>(actionEnumLeft);

        for (int i = 0;  i < fileLines.Count;i++)
        {
            for(int y = 0; y < actionEnumLeftCopy.Count; y++)
            {
                if(fileLines[i].Contains(actionEnumLeftCopy[y]))
                {
                    actionEnumLeftCopy.RemoveAt(y);
                    fileLines.RemoveAt(i);
                    i--;
                    break;
                }
            }
        }
    }

    private void WriteLineInFile (string filePath, List<string> fileLines)
    {
        using (StreamWriter writer = new StreamWriter(filePath))
        {
            foreach (string line in fileLines)
            {
                writer.WriteLine(line);
            }
        }
    }

    private void RenameSOWithRemovedID (List<string> gameActionEnumStringLeft)
    {
        string directoryPath = SO_InputManagerSettings.GetPathSOInputs;
        string directoryPathIO = SO_InputManagerSettings.GetPathSOInputs;
        if (String.IsNullOrEmpty(directoryPath))
        {
            SO_InputManagerSettings.GetOrCreateSettings();
            directoryPath = SO_InputManagerSettings.GetPathSOInputs;
            directoryPathIO = directoryPath;
        }

        directoryPathIO = Application.dataPath + directoryPathIO.Replace("Assets", "");

        string[] files = Directory.GetFiles(directoryPathIO, "*.asset", SearchOption.AllDirectories);

        for(int i = 0; i < files.Length; i++)
        {
            for(int y = 0; y < gameActionEnumStringLeft.Count; y++)
            {
                if (Path.GetFileNameWithoutExtension(files[i]) == GetSONameWithActionID(gameActionEnumStringLeft[y]))
                {
                    Debug.LogError("The asset " + GetSONameWithActionID(gameActionEnumStringLeft[y]) + " as no reference (no same name in the InputActionAsset) is the InputActionAsset." +
                        " This asset will rename like this: " + GetSONameWithActionIDNoReference(gameActionEnumStringLeft[y]) + ". This is not remove to not remove your references in inspector in other script." +
                        " If you want to delet this asset you can remove it in: " + directoryPath + GetSONameWithActionIDWithExtension(gameActionEnumStringLeft[y]));
                    
                    AssetDatabase.RenameAsset(directoryPath + GetSONameWithActionIDWithExtension(gameActionEnumStringLeft[y]), GetSONameWithActionIDNoReference(gameActionEnumStringLeft[y]));
                }
            }
        }
    }

    private void GenerateEnumInputActionV2()
    {
        string filePath = Application.dataPath + "/Input/GameActionsMapsNames.cs";

        if (!GetFileLines(filePath, out List<string> filelines))
        {
            //Generer le fichier si il n existe pas
            return;
        }

        GetAllEnumValue(out List<string> gameActionMapEnumString, out List<string> gameActionEnumString);

        if(!GenerateEnumInputAction(ref filelines, ref gameActionEnumString, ref gameActionMapEnumString))
        {
            Debug.Log("There is no change is the enum of Actions and ActionMaps");
            return;
        }
         
        WriteLineInFile(filePath, filelines);

        RenameSOWithRemovedID(gameActionEnumString);

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh(); 

        AssetDatabase.ImportAsset("Assets/Input/GameActionsMapsNames.cs", ImportAssetOptions.ForceUpdate);

        UnityEditor.Compilation.CompilationPipeline.RequestScriptCompilation();

        Debug.LogError("You Need to click on the button on InputManager to regenerate SO_Input");
    }

    /*
        GameInputActions inputActions = new GameInputActions();
        bool isLineChanges = false;

        for (int i = 0; i < inputActions.asset.actionMaps.Count; i++)
        {
            for (int t = 0; t < inputActions.asset.actionMaps[i].actions.Count; t++)
            {
                string actionID = inputActions.asset.actionMaps[i].name + "_" + inputActions.asset.actionMaps[i].actions[t].name;

                if(!CheckIfEnumExist(ref gameActionEnumString, actionID))
                {
                    filelines.Insert(indexToInsetAction, ALINEA + ALINEA + actionID + ",");
                    indexToInsetAction++;
                    indexToInsetActionMap++;
                    isLineChanges = true;
                }
            }

            if (!CheckIfEnumExist(ref gameActionMapEnumString, inputActions.asset.actionMaps[i].name))
            {
                filelines.Insert(indexToInsetActionMap, ALINEA + ALINEA + inputActions.asset.actionMaps[i].name + ",");
                isLineChanges = true;
            }
        }
     */

    private bool GenerateEnumInputAction(ref List<string> filelines, ref List<string> gameActionEnumString, ref List<string> gameActionMapEnumString)
    {
        GetInsertsLine(filelines, out int indexToInsetAction, out int indexToInsetActionMap);

        GameInputActions inputActions = new GameInputActions();
        bool isLineChanges = false;

        for (int i = 0; i < inputActions.asset.actionMaps.Count; i++)
        {
            for (int t = 0; t < inputActions.asset.actionMaps[i].actions.Count; t++) 
            {
                string actionID = FormatActionID(inputActions.asset.actionMaps[i].name + "_" + inputActions.asset.actionMaps[i].actions[t].name);

                if (!CheckIfEnumExist(ref gameActionEnumString, actionID))
                {
                    filelines.Insert(indexToInsetAction, ALINEA + ALINEA + actionID + ",");
                    indexToInsetAction++;
                    indexToInsetActionMap++;
                    isLineChanges = true;
                }
            }

            string actionMapID = FormatActionID(inputActions.asset.actionMaps[i].name);

            if (!CheckIfEnumExist(ref gameActionMapEnumString, actionMapID))
            {
                filelines.Insert(indexToInsetActionMap, ALINEA + ALINEA + actionMapID + ",");
                isLineChanges = true;
            }
        }

        if(gameActionEnumString.Count != 0 || gameActionMapEnumString.Count != 0)
        {
            RemoveActionsEnumLines(ref filelines, gameActionEnumString);
            RemoveActionsEnumLines(ref filelines, gameActionMapEnumString);
            isLineChanges = true;
        }

        return isLineChanges;
    }

    public static string FormatActionID(string actionID)
    {
        // Étape 1 : Enlever les caractères spéciaux non autorisés
        // Conserver uniquement les lettres, chiffres, les espaces et les underscores (_)
        string cleanedID = Regex.Replace(actionID, @"[^a-zA-Z0-9_ ]", "");
        // Étape 2 : Capitaliser la première lettre et les lettres après les espaces ou underscores
        string formattedID = "";
        bool capitalizeNext = true;

        foreach (char c in cleanedID)
        {
            if (c == ' ')
            {
                capitalizeNext = true;
                continue;
            }

            if (c == '_')
            {
                formattedID += c;
                capitalizeNext = true;
                continue;
            }

            if (capitalizeNext)
            {
                formattedID += char.ToUpper(c); // Capitaliser la lettre
                capitalizeNext = false;
                continue;
            }

            formattedID += c;
        }

        // Étape 3 : Capitaliser la première lettre de la chaîne entière
        if (formattedID.Length > 0)
        {
            formattedID = char.ToUpper(formattedID[0]) + formattedID.Substring(1);
        }

        return formattedID;
    }

    //Gerer les enum qui n existe plus et les retirer
    //Gerer si les valeur des enum on etait supp
    //Recompiler qui au besoin si changement dans le fichier
    private void GenerateEnumInputActionV0001()
    {

        // Define the file path where you want to write the output
        string filePath = Application.dataPath + "/Input/GameActionsMapsNames.cs";

        if(!File.Exists(filePath))
        {
            Debug.Log("The file GameActionsMapsNames does not exist it s will be create at this path: " + filePath);
            return;
        }


        GameInputActions inputActions = new GameInputActions();

        List<string> lines = File.ReadAllLines(filePath).ToList();

        int indexToInsetAction = -1;
        int indexToInsetActionMap = -1;

        for (int l = 0; l < lines.Count; l++)
        {
            if (lines[l].Contains("GameActionInsertLine")) { indexToInsetAction = l; }
            if (lines[l].Contains("GameActionMapInsertLine")) {  indexToInsetActionMap = l; }
            if (indexToInsetAction != -1 && indexToInsetActionMap != -1) { break; }
        }

        for (int i = 0; i < inputActions.asset.actionMaps.Count; i++)
        {
            for (int t = 0; t < inputActions.asset.actionMaps[i].actions.Count; t++)
            {
                bool isActionExist = false;

                for(int v = 0; v < Enum.GetValues(typeof(GameActionsEnum)).Length; v++)
                {
                    if(((GameActionsEnum)v).ToString() == inputActions.asset.actionMaps[i].name + "_" + inputActions.asset.actionMaps[i].actions[t].name)
                    {
                        isActionExist = true;
                        break;
                    }
                }

                if(isActionExist) { continue; }

                lines.Insert(indexToInsetAction, ALINEA + ALINEA + inputActions.asset.actionMaps[i].name + "_" + inputActions.asset.actionMaps[i].actions[t].name + ",");
                indexToInsetAction++;
                indexToInsetActionMap++;
            }

            bool isActionMapExist = false;

            for (int v = 0; v < Enum.GetValues(typeof(GameActionsMapsEnum)).Length; v++)
            {
                if (((GameActionsMapsEnum)v).ToString() == inputActions.asset.actionMaps[i].name)
                {
                    isActionMapExist = true;
                    break;
                }
            }

            if (isActionMapExist) { continue; }

            lines.Insert(indexToInsetActionMap, ALINEA + ALINEA + inputActions.asset.actionMaps[i].name + ",");
            indexToInsetActionMap++;
        }


        using (StreamWriter writer = new StreamWriter(filePath))
        {
            foreach (string line in lines)
            {
                writer.WriteLine(line);
            }
        }

        Debug.Log("File written successfully.");

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();



        AssetDatabase.ImportAsset("Assets/Input/GameActionsMapsNames.cs", ImportAssetOptions.ForceUpdate);

        UnityEditor.Compilation.CompilationPipeline.RequestScriptCompilation();
    }



    //Gerer si des asset sont dans le dossier et ne servent plus a rien 
    private void GenerateScriptableObjectV4()
    {
        List<InputManager.InputMapsData> inputMapData = new List<InputManager.InputMapsData>();

        GameInputActions inputActions = new GameInputActions();

        string directoryPath = SO_InputManagerSettings.GetPathSOInputs;

        if (String.IsNullOrEmpty(directoryPath))
        {
            SO_InputManagerSettings.GetOrCreateSettings();
            directoryPath = SO_InputManagerSettings.GetPathSOInputs;
        }

        for (int i = 0; i < inputActions.asset.actionMaps.Count; i++)
        {
            inputMapData.Add(new InputManager.InputMapsData());

            string actionMapName = FormatActionID(inputActions.asset.actionMaps[i].name);

            if (!GetEnumActionMapByString(actionMapName, out GameActionsMapsEnum actionMapEnum))
            {
                Debug.Log("didn t find the action map enum: " + actionMapEnum.ToString() +" Target: " + actionMapName);
            }

            inputMapData[i].ActionMap = actionMapEnum;

            for (int t = 0; t < inputActions.asset.actionMaps[i].actions.Count; t++)
            {
                string actionName = inputActions.asset.actionMaps[i].actions[t].name;
                string actionID = actionMapName + "_" + FormatActionID(actionName);
                string assetPath = directoryPath + GetSONameWithActionIDWithExtension(actionID);

                switch (inputActions.asset.actionMaps[i].actions[t].expectedControlType)
                {
                    case "Vector2":

                        if(CheckAndSetAssetExistAndSameValue<SO_Input_Vector2, Vector2>(ref inputMapData[i].ListActionVector2Enum, ref inputMapData[i].Vector2SOInput, assetPath, actionID, ValueType.Vector2))
                        {
                            break;
                        }

                        SO_Input_Vector2 soInputVector2 = CreateInstance<SO_Input_Vector2>();

                        CreateAndSetSOInput(assetPath, ref soInputVector2, ref inputMapData[i].ListActionVector2Enum, ref inputMapData[i].Vector2SOInput, actionID, actionName, ValueType.Vector2);

                        break;

                    case "Axis":


                        if (CheckAndSetAssetExistAndSameValue< SO_Input_Float, float>(ref inputMapData[i].ListActionFloatEnum, ref inputMapData[i].FloatSOInput, assetPath, actionID, ValueType.Float))
                        {
                            break;
                        }

                        SO_Input_Float soInputFloat = CreateInstance<SO_Input_Float>();

                        CreateAndSetSOInput(assetPath, ref soInputFloat, ref inputMapData[i].ListActionFloatEnum, ref inputMapData[i].FloatSOInput, actionID, actionName, ValueType.Float);

                        break;

                    default:

                        if (CheckAndSetAssetExistAndSameValue<SO_Input_Bool, bool>(ref inputMapData[i].ListActionBoolEnum, ref inputMapData[i].BoolSOInput, assetPath, actionID, ValueType.Bool))
                        {
                            break;
                        }

                        SO_Input_Bool soInputBool = CreateInstance<SO_Input_Bool>();

                        CreateAndSetSOInput(assetPath, ref soInputBool, ref inputMapData[i].ListActionBoolEnum, ref inputMapData[i].BoolSOInput, actionID, actionName, ValueType.Bool);

                        break;
                }
            }
        }

        SaveAssetAndSetInputMapData(inputMapData);
    }

    private bool CheckAndSetAssetExistAndSameValue<TScriptableObject, T> (ref List<GameActionsEnum> listGameActionEnum, ref List<SO_Input_Base<T>> listSo, string assetPath, string actionID, ValueType valueType) where TScriptableObject : SO_Input_Base<T>
    {
        if (File.Exists(Application.dataPath.Replace("Assets", "") + assetPath))
        {
            UnityEngine.Object soInputAsset = AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(assetPath);

            if (!(soInputAsset is SO_Input_Base<T> soInputAssetConverted))
            {
                Debug.LogWarning(listGameActionEnum + " Exist But The The Type Of SO_Input is not Valid. It's will be delet and recreat all the reference of this SO will be null.");
                return false;
            }

            Type type = typeof(SO_Input_Base<T>);

            GetEnumActionByString(actionID, out GameActionsEnum enumValue);
            listGameActionEnum.Add(enumValue);

            FieldInfo actionIDField = type.GetField("_actionID", BindingFlags.NonPublic | BindingFlags.Instance);
            actionIDField.SetValue(soInputAssetConverted, enumValue);

            listSo.Add(soInputAssetConverted);
            return true;
        }
        else
        {
            Debug.Log("do not exist: " + (Application.dataPath.Replace("Assets/", "") + assetPath));
        }

        return false;
    }

    // where TScriptableObject : SO_Input_Base<T> 
    // Permet de s assurer que  TScriptableObject deriver de SO_Input_Base<T> 
    // Permet d acceder au varirable de la classe SO_Input_Base<T> 
    private void CreateAndSetSOInput<TScriptableObject, T> (string assterPath,ref TScriptableObject SOInput, ref List<GameActionsEnum> listGameActionEnum, ref List<SO_Input_Base<T>> listSOInput, string actionID, string actionName, ValueType valueType) where TScriptableObject : SO_Input_Base<T>
    {
        Type inputSOType = typeof(SO_Input_Base<T>);
        FieldInfo actionIDField = inputSOType.GetField("_actionID", BindingFlags.NonPublic | BindingFlags.Instance);
        FieldInfo actionNameField = inputSOType.GetField("_actionName", BindingFlags.NonPublic | BindingFlags.Instance);
        FieldInfo valueTypeField = inputSOType.GetField("_valueType", BindingFlags.NonPublic | BindingFlags.Instance);

        GetEnumActionByString(actionID, out GameActionsEnum enumValue);
        actionIDField.SetValue(SOInput, enumValue);
        actionNameField.SetValue(SOInput, actionName);
        valueTypeField.SetValue(SOInput, valueType);

        AssetDatabase.CreateAsset(SOInput, assterPath);

        listGameActionEnum.Add(enumValue);
        listSOInput.Add(SOInput);
    }

    /// <summary>
    /// Fonction qui Save lees SO_Input dans l'asset folder
    /// Set List<InputManager.InputMapsData> dans InputManager (Contient tout les SO / Enum des InputAction)
    /// </summary>
    /// <param name="inputMapData">List<InputManager.InputMapsData> a set dans l inputManager</param>
    private void SaveAssetAndSetInputMapData (List<InputManager.InputMapsData> inputMapData)
    {
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        
        InputManager inputManager = target as InputManager;

        Type inputManagerType = typeof(InputManager);
        FieldInfo allActionEnumtField = inputManagerType.GetField("_inputMapData", BindingFlags.NonPublic | BindingFlags.Instance);

        allActionEnumtField.SetValue(inputManager, inputMapData);

        EditorUtility.SetDirty(inputManager);
    }
    
    private string GetSONameWithActionIDWithExtension(string actionID)
    {
        return GetSONameWithActionID(actionID) + ".asset";
    }

    private string GetSONameWithActionID (string actionID)
    {
        return "SO_Input_" + actionID;
    }

    private string GetSONameWithActionIDNoReferenceWithExtension(string actionID)
    {
        return GetSONameWithActionIDNoReference(actionID) +".asset";
    }

    private string GetSONameWithActionIDNoReference(string actionID)
    {
        return "SO_Input_" + actionID + "_NoReference";
    }
}

