using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEditor;
using System.IO;
using static GameActionsMapsNames;
using System.Linq;

[CustomEditor(typeof(InputManager))]
public class InputManagerEditor : Editor
{
    InputManager inputManager;
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        GUILayout.Space(10);

        if(inputManager == null)
        {
            inputManager = target as InputManager;
            //Verifier si l enum n est pas la bonne
            GenerateEnumInputAction();
        }

         
        if (GUILayout.Button("Find / categorize / Initialize SO_Input_Base assets"))
        {
            GenerateScriptableObjectV4();
        }
    }

    private const string ALINEA = "    ";

    //Gerer les enum qui n existe plus et les retirer
    //Gerer si les valeur des enum on etait supp
    //Recompiler qui au besoin si changement dans le fichier
    private void GenerateEnumInputAction()
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
            if(GetEnumActionMapByString(inputActions.asset.actionMaps[i].name, out GameActionsMapsEnum actionMapEnum))
            {
                Debug.Log("j ai trouver enum: " + actionMapEnum.ToString());
            }
            else
            {
                Debug.Log("j ai pas trouver enum: " + actionMapEnum.ToString() +" Target: " + inputActions.asset.actionMaps[i].name);
            }

            inputMapData[i].ActionMap = actionMapEnum;

            for (int t = 0; t < inputActions.asset.actionMaps[i].actions.Count; t++)
            {
                string actionName = inputActions.asset.actionMaps[i].actions[t].name;
                string actionID = inputActions.asset.actionMaps[i].name + "_" + actionName;
                string assetPath = directoryPath + "SO_Input_" + actionID + ".asset";

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

    private bool  CheckAndSetAssetExistAndSameValue<TScriptableObject, T> (ref List<GameActionsEnum> listGameActionEnum, ref List<SO_Input_Base<T>> listSo, string assetPath, string actionID, ValueType valueType) where TScriptableObject : SO_Input_Base<T>
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
}

