using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.InputSystem;
using static InputManager;

public class InputManager : MonoBehaviorDontDestroy
{
    public static InputManager Instance { get; private set; }
    private GameInputActions _inputActions;

    [SerializeField] private List<InputMapsData> _inputMapData;

    #region Unity Methods
    protected override void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        InitializeInputManager();
        base.Awake();
    }

    protected override void OnDestroy()
    {
#if UNITY_EDITOR
        for (int i = 0; i < _inputMapData.Count; i++)
        {
            for (int j = 0; j < _inputMapData[i].Vector2SOInput.Count; j++)
            {
                Type inputVector2 = typeof(SO_Input_Base<Vector2>);
                FieldInfo initilizeField = inputVector2.GetField("_initialazed", BindingFlags.NonPublic | BindingFlags.Instance);
                initilizeField.SetValue(_inputMapData[i].Vector2SOInput[j], false); 

            }

            for (int j = 0; j < _inputMapData[i].BoolSOInput.Count; j++)
            {
                Type inputBool = typeof(SO_Input_Base<bool>);
                FieldInfo initilizeField = inputBool.GetField("_initialazed", BindingFlags.NonPublic | BindingFlags.Instance);
                initilizeField.SetValue(_inputMapData[i].BoolSOInput[j], false);
            }

            for (int j = 0; j < _inputMapData[i].FloatSOInput.Count; j++)
            {
                Type inputFloat = typeof(SO_Input_Base<float>);
                FieldInfo initilizeField = inputFloat.GetField("_initialazed", BindingFlags.NonPublic | BindingFlags.Instance);
                initilizeField.SetValue(_inputMapData[i].FloatSOInput[j], false);
            }
        }
#endif
        base.OnDestroy();
    }
    #endregion

    #region Initilization Methods
    private void InitializeInputManager()
    {
        _inputActions = new GameInputActions();

        _inputActions.Enable();

        for (int i = 0;  i < _inputMapData.Count; i++)
        {
            for (int j = 0; j < _inputMapData[i].Vector2SOInput.Count; j++)
            {
                InitializeInputs(_inputMapData[i].Vector2SOInput[j], ref _inputMapData[i].OnActivedVector2, ctx => ctx.ReadValue<Vector2>());
            }

            for (int j = 0; j < _inputMapData[i].BoolSOInput.Count; j++)
            {
                InitializeInputs(_inputMapData[i].BoolSOInput[j], ref _inputMapData[i].OnActivedBool, ctx => ctx.ReadValueAsButton());
            }

            for (int j = 0; j < _inputMapData[i].FloatSOInput.Count; j++)
            {
                InitializeInputs(_inputMapData[i].FloatSOInput[j], ref _inputMapData[i].OnActivedFloat, ctx => ctx.ReadValue<float>());
            }
        }
    }
   
    private void InitializeInputs<T>(SO_Input_Base<T> inputSO, ref List<Action<bool, float, string>> onActivetedList,  Func<InputAction.CallbackContext, T> readValue)
    {
        var action = _inputActions.FindAction(inputSO.ActionName);
        if (action == null)
        {
            Debug.LogWarning($"Action {inputSO.ActionName} not found.");
            return;
        }

        inputSO.InitializationSOValueInput(out Action<bool, float, string> newAction, out Action<T, string> performedAction);
        onActivetedList.Add(newAction);

        _inputActions.FindAction(inputSO.ActionName).started += (ctx) => { newAction?.Invoke(true, 0, ctx.control.device.name); };

        _inputActions.FindAction(inputSO.ActionName).performed += (ctx) => { inputSO.Value = readValue(ctx); };
        _inputActions.FindAction(inputSO.ActionName).performed += (ctx) => { performedAction?.Invoke(readValue(ctx), ctx.control.device.name); };

        _inputActions.FindAction(inputSO.ActionName).canceled += (ctx) => {
             inputSO.Value = readValue(ctx);
             newAction?.Invoke(false, (float)ctx.duration, ctx.control.device.name); 
        };
    }
    #endregion

    //Changer methode pour eviter a faire trop de for quite a faire une liste global/Dictionnaire a l initialization pour faciliter ca.
    //le faire pour gagner des perf pas prio
    #region Enable Methods
    //A Securiser
    public void EnableActionMap (GameActionsMapsNames.GameActionsMapsEnum actionMapEnum,bool value)
    {
        for(int i = 0; i < _inputMapData.Count; i++)
        {
            if (_inputMapData[i].ActionMap == actionMapEnum)
            {
                EnableActionMap(_inputMapData[i], value);
                return;
            }
        }
    }

    private void EnableActionMap (InputMapsData inputMapData, bool value)
    {
        for(int i = 0;  i < inputMapData.Vector2SOInput.Count; i++)
        {
            inputMapData.Vector2SOInput[i].BridgeValue.ActiveBridge = value;
        }
        for (int i = 0; i < inputMapData.BoolSOInput.Count; i++)
        {
            inputMapData.BoolSOInput[i].BridgeValue.ActiveBridge = value;
        }
        for (int i = 0; i < inputMapData.FloatSOInput.Count; i++)
        {
            inputMapData.FloatSOInput[i].BridgeValue.ActiveBridge = value; 
        }
    }

    //A Securiser
    public void EnableAction (GameActionsMapsNames.GameActionsEnum actionEnum, bool value)
    {
        GameActionsMapsNames.GetEnumActionMapByString(actionEnum.ToString().Split("_")[0], out GameActionsMapsNames.GameActionsMapsEnum actionMapEnum);

        for (int i = 0; i < _inputMapData.Count; i++)
        {
            if (_inputMapData[i].ActionMap == actionMapEnum)
            {
                EnableActionInActionMap(actionEnum, _inputMapData[i], value);
                return;
            }
        }

        Debug.Log("there is not InputData with this actionEnum in the inputManager");
    }

    //TODO Faire en sorte d ameliorer la recherche
    private void EnableActionInActionMap(GameActionsMapsNames.GameActionsEnum actionEnum, InputMapsData inputMapData, bool value)
    {

        for (int i = 0; i < inputMapData.Vector2SOInput.Count; i++)
        {
            if (_inputMapData[i].ListActionVector2Enum[i] == actionEnum)
            {
                inputMapData.Vector2SOInput[i].BridgeValue.ActiveBridge = value;
                return;
            }
        }
        for (int i = 0; i < inputMapData.BoolSOInput.Count; i++)
        {
            if (_inputMapData[i].ListActionBoolEnum[i] == actionEnum)
            {
                inputMapData.BoolSOInput[i].BridgeValue.ActiveBridge = value;
                return;
            }
        }
        for (int i = 0; i < inputMapData.FloatSOInput.Count; i++)
        {
            if (_inputMapData[i].ListActionFloatEnum[i] == actionEnum)
            {
                inputMapData.FloatSOInput[i].BridgeValue.ActiveBridge = value;
                return;
            }
        }
        Debug.LogWarning($"Action {actionEnum} not found in ActionMap {inputMapData.ActionMap}.");
    }
    #endregion

    public bool GetSOInput<T> (GameActionsMapsNames.GameActionsEnum actionEnum, out SO_Input_Base<T> soInput)
    {
        GameActionsMapsNames.GetEnumActionMapByString(actionEnum.ToString().Split("_")[0], out GameActionsMapsNames.GameActionsMapsEnum actionMapEnum);
        soInput = null;

        for (int t = 0; t < _inputMapData.Count; t++)
        {
            if (_inputMapData[t].ActionMap == actionMapEnum)
            {
                for (int i = 0; i < _inputMapData[t].Vector2SOInput.Count; i++)
                {
                    if (_inputMapData[t].ListActionVector2Enum[i] == actionEnum)
                    {
                        if (!CheckIfSameType(typeof(Vector2), typeof(T)))
                        {
                            return false;
                        }
                        soInput =  _inputMapData[t].Vector2SOInput[i] as SO_Input_Base<T>;
                        return true;
                    }
                }
                for (int i = 0; i < _inputMapData[i].BoolSOInput.Count; i++)
                {
                    if (_inputMapData[i].ListActionBoolEnum[i] == actionEnum)
                    {
                        if (!CheckIfSameType(typeof(bool), typeof(T)))
                        {
                            return false;
                        }
                        soInput = _inputMapData[t].BoolSOInput[i] as SO_Input_Base<T>;
                        return true;
                    }
                }
                for (int i = 0; i < _inputMapData[i].FloatSOInput.Count; i++)
                {
                    if (_inputMapData[i].ListActionFloatEnum[i] == actionEnum)
                    {
                        if (!CheckIfSameType(typeof(float), typeof(T)))
                        {
                            return false;
                        }
                        soInput = _inputMapData[t].FloatSOInput[i] as SO_Input_Base<T>;
                        return true;
                    }
                }

            }
        }
        Debug.LogWarning("Not SO_Input was find multiple resond can be the origine actionMapEnum not match with a InputMapsData or not actionEnum match with GameActionsMapsNames.GameActionsEnum int the InputMapsData search value: " + actionEnum);
        return false;
    }

    //This onction is here just for the Debug.Log
    private bool CheckIfSameType (Type tocheck, Type reference)
    {
        if (tocheck != reference)
        {
            Debug.LogWarning("You try to get a SOInput with the wrong type, actual type: " + tocheck + " type you try to get: " + reference);
            return false;
        }
        return true;
    }

    [Serializable]
    public class InputMapsData
    {
        [SerializeField, StringReadOnly] public GameActionsMapsNames.GameActionsMapsEnum ActionMap;

        [SerializeField, StringReadOnly] public List<GameActionsMapsNames.GameActionsEnum> ListActionVector2Enum;
        [SerializeField, StringReadOnly] public List<SO_Input_Base<Vector2>> Vector2SOInput;
        [SerializeField, HideInInspector] public List<Action<bool, float, string>> OnActivedVector2;
        [SerializeField, StringReadOnly] public List<GameActionsMapsNames.GameActionsEnum> ListActionFloatEnum;
        [SerializeField, StringReadOnly] public List<SO_Input_Base<float>> FloatSOInput;
        [SerializeField, HideInInspector] public List<Action<bool, float, string>> OnActivedFloat;
        [SerializeField, StringReadOnly] public List<GameActionsMapsNames.GameActionsEnum> ListActionBoolEnum;
        [SerializeField, StringReadOnly] public List<SO_Input_Base<bool>> BoolSOInput;
        [SerializeField, HideInInspector] public List<Action<bool, float, string>> OnActivedBool;

        public InputMapsData ()
        {
            ListActionVector2Enum = new List<GameActionsMapsNames.GameActionsEnum> ();
            Vector2SOInput = new List<SO_Input_Base<Vector2>>();
            OnActivedVector2 = new List<Action<bool, float, string>>();

            ListActionFloatEnum = new List<GameActionsMapsNames.GameActionsEnum>();
            FloatSOInput = new List<SO_Input_Base<float>>();
            OnActivedFloat = new List<Action<bool, float, string>>();

            ListActionBoolEnum = new List<GameActionsMapsNames.GameActionsEnum>();
            BoolSOInput = new List<SO_Input_Base<bool>>();
            OnActivedBool = new List<Action<bool, float, string>>();
        }
    }
}



