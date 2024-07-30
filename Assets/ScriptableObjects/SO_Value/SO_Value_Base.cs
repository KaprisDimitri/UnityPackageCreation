using System;
using UnityEngine;

public abstract class SO_Value_Base<T> : ScriptableObject
{
    //TODO: ESSAYER DE METTRE UNE SCURED VALUE ICI
    [SerializeField,StringReadOnly] protected T _value;
    /// <summary>
    /// Can be call to set a action to receive the value a is change
    /// </summary>
    [SerializeField] private BridgeValue<T> _bridgeValue;
    protected Action<T> _sendBridgeValue;
    protected bool _initialazed = false;

    public T Value { get { return _value; } set { SetValue(value); } }
    public BridgeValue<T> BridgeValue => _bridgeValue;

    /// <summary>
    /// Need to be call to Initialize the SO_Value
    /// </summary>
    public virtual void Initialization(bool activeBridge = true)
    {
        if(_initialazed) { Debug.LogWarning(this.name + " is already initialize"); return; }
        _bridgeValue = new BridgeValue<T>(out _sendBridgeValue, activeBridge);
        _value = default;
        _initialazed = true;
    }

    protected void SetValue (T newValue)
    {
        if(!CheckIfCanSetValue(newValue)) { return; }

        _value = newValue;
        _sendBridgeValue?.Invoke(_value);
    } 

    /// <summary>
    /// Methode calling SetRecieveValue of the SecuredAction in BridgeValue
    /// </summary>
    /// <param name="actionToRecieveValue"></param>
    /// <param name="set"></param>
    public void SetReceiveBridgeValue (Action<T> actionToRecieveValue, bool set)
    {
        _bridgeValue.RecieveBridgeValue.SetRecieveValue(actionToRecieveValue, set);
    }

    protected virtual bool CheckIfCanSetValue (T newValue)
    {
        return !newValue.Equals(_value);
    }
}
