using System;
using UnityEngine;

/// <summary>
/// Can be use when you want to have a action send the value change And can be deactiveted
/// </summary>
/// <typeparam name="T"></typeparam>
[Serializable]
public class BridgeValue<T>
{
    /// <summary>
    /// is this value is at FALSE the action will not send the value to recevers (_recieveBridgeValue)
    /// </summary>
    [SerializeField, StringReadOnly]  private bool _activeBridge = true;
    //TODO: ESSAYER DE METTRE EN PLACE UNE SECURED VALUE ICI CAR POTENTIELLEMENT PLUSIEUR ENDROIT VOUDRAIS ACTIVER OU DESACTIVER LE PONT !!!!
    public bool ActiveBridge { get { return _activeBridge; } set { _activeBridge = value; } }

    /// <summary>
    /// Action called when the value is changed
    /// </summary>
    private SecuredAction<T> _recieveBridgeValue;
    private Action<T> _recieveBridgeValueKeySender;
    private Action<T> _senderBridgeValue;

    public SecuredAction<T> RecieveBridgeValue => _recieveBridgeValue;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="actionSendBridgeValue">it s the bridge of the owner. It s use to send the value tu the BridgeClass. Like that just the owner of BridgeValue can send value tu the BridgeClass</param>
    /// <param name="activeBridge"></param>
    public BridgeValue(out Action<T> actionSendBridgeValue, bool activeBridge = true)
    {
        _senderBridgeValue += OnValueChange;
        actionSendBridgeValue = _senderBridgeValue;

        _activeBridge = activeBridge;

        _recieveBridgeValue = new SecuredAction<T>(out _recieveBridgeValueKeySender);
    }

    private void OnValueChange(T value)
    {
        if (!_activeBridge) { return; }
        _recieveBridgeValueKeySender?.Invoke(value);
    }
}
