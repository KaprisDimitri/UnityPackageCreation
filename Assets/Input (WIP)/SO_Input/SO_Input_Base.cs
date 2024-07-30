using System;
using UnityEngine;

public abstract class SO_Input_Base<T> : SO_Value_Base<T>
{
    
    [SerializeField, StringReadOnly] private GameActionsMapsNames.GameActionsEnum _actionID;
    public GameActionsMapsNames.GameActionsEnum ActionID => _actionID;
    public string ActionIDString => _actionID.ToString();

    [SerializeField, StringReadOnly] private string _actionName;
    public string ActionName => _actionName;
    [SerializeField, StringReadOnly] private ValueType _valueType;
    public ValueType ValueType => _valueType;


    /// <summary>
    /// Call when the input Start/Cancel, the bool value is if is started or cancel(false),  the float value is the total time pressed
    /// </summary>
    private SecuredAction<bool,float, string> _onActived;
    public SecuredAction<bool, float, string> OnActived => _onActived;

    private SecuredAction<T, string> _onPerformed;
    public SecuredAction<T, string> OnPerformed => _onPerformed;


    public virtual bool InitializationSOValueInput (out Action<bool,float, string> sendStarted, out Action<T, string> sendPerformed)
    {
        sendStarted = null;
        sendPerformed = null;
        if (_initialazed) { Debug.LogWarning(this.name + " is already initialize"); return false; }
        base.Initialization(false);

        _onActived = new SecuredAction<bool,float, string>(out sendStarted, () => { return BridgeValue.ActiveBridge; });
        _onPerformed = new SecuredAction<T, string>(out sendPerformed, () => { return BridgeValue.ActiveBridge; });
        return true;
    }
}

public enum ValueType
{
    Bool,
    Float,
    Int,
    Vector2,
}






