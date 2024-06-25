using System;
using System.Diagnostics;

public class SecuredAction
{
    /// <summary>
    /// Action called when the value is changed
    /// </summary>
    private Action _recieve;
    private Action send;

    private Func<bool> _conditionToSend;

    public void SetRecieveValue(Action recieveFunc, bool set)
    {
        if (set)
        {
            _recieve += recieveFunc;
            return;
        }

        _recieve -= recieveFunc;
    }

    public SecuredAction(out Action send, Func<bool> conditionToSend = null)
    {
        this.send += OnValueChange;
        send = this.send;

        if (conditionToSend == null)
        {
            _conditionToSend = () => { return true; };
            return;
        }

        _conditionToSend = conditionToSend;
    }


    private void OnValueChange()
    {
        if(!_conditionToSend()) { return; }
        _recieve?.Invoke();
    }
}

public class SecuredAction<T>
{
    /// <summary>
    /// Action called when the value is changed
    /// </summary>
    private Action<T> _recieve;
    private Action<T> send;
    private Func<bool> _conditionToSend;
    public void SetRecieveValue(Action<T> recieveFunc, bool set)
    {
        if (set)
        {
            _recieve += recieveFunc;
            return;
        }

        _recieve -= recieveFunc;
    }

    public SecuredAction(out Action<T> send, Func<bool> conditionToSend = null)
    {
        this.send += OnValueChange;
        send = this.send;

        if (conditionToSend == null)
        {
            _conditionToSend = () => { return true; };
            return;
        }

        _conditionToSend = conditionToSend;
    }

    private void OnValueChange(T value)
    {
        if (!_conditionToSend()) { return; }
        _recieve?.Invoke(value);
    }
}

public class SecuredAction<T, T2>
{
    /// <summary>
    /// Action called when the value is changed
    /// </summary>
    private Action<T, T2> _recieve;
    private Action<T, T2> send;
    private Func<bool> _conditionToSend;
    public void SetRecieveValue(Action<T, T2> recieveFunc, bool set)
    {
        if (set)
        {
            _recieve += recieveFunc;
            return;
        }

        _recieve -= recieveFunc;
    }

    public SecuredAction(out Action<T, T2> send, Func<bool> conditionToSend = null)
    {
        this.send += OnValueChange;
        send = this.send;

        if (conditionToSend == null)
        {
            _conditionToSend = () => { return true; };
            return;
        }

        _conditionToSend = conditionToSend;
    }

    private void OnValueChange(T value, T2 value2)
    {
        if (!_conditionToSend()) { return; }
        _recieve?.Invoke(value, value2);
    }
}

public class SecuredAction<T, T2, T3>
{
    /// <summary>
    /// Action called when the value is changed
    /// </summary>
    private Action<T, T2, T3> _recieve;
    private Action<T, T2, T3> send;
    private Func<bool> _conditionToSend;
    public void SetRecieveValue(Action<T, T2, T3> recieveFunc, bool set)
    {
        if (set)
        {
            _recieve += recieveFunc;
            return;
        }

        _recieve -= recieveFunc;
    }

    public SecuredAction(out Action<T, T2, T3> send, Func<bool> conditionToSend = null)
    {
        this.send += OnValueChange;
        send = this.send;

        if (conditionToSend == null)
        {
            _conditionToSend = () => { return true; };
            return;
        }

        _conditionToSend = conditionToSend;
    }

    private void OnValueChange(T value, T2 value2, T3 value3)
    {
        if (!_conditionToSend()) { return; }
        _recieve?.Invoke(value, value2, value3);
    }
}

