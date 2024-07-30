using UnityEngine;

public class ExempleInputScript : MonoBehaviour
{
    [SerializeField] private SO_Input_Vector2 mouvement;

    void Start()
    {
        EnableAction2();
        SetEvent2();
    }

    public void EnableActionMap()
    {
        InputManager.Instance.EnableActionMap(GameActionsMapsNames.GameActionsMapsEnum.ExempleActionMap, true);
        Debug.Log("Enable ActionMap");
    }

    public void EnableAction()
    {
        InputManager.Instance.EnableAction(mouvement.ActionID, true);
        Debug.Log("Enable action");
    }
    public void EnableAction2()
    {
        InputManager.Instance.EnableAction(GameActionsMapsNames.GameActionsEnum.ExempleActionMap_Moving, true);
        Debug.Log("Enable action");
    }

    public void SetEvent()
    {
        mouvement.SetReceiveBridgeValue(MouvementAction, true);
        mouvement.OnActived.SetRecieveValue(Active, true);
        mouvement.OnPerformed.SetRecieveValue(MouvementAction2, true);
    }

    public void SetEvent2()
    {
        if (!InputManager.Instance.GetSOInput(GameActionsMapsNames.GameActionsEnum.ExempleActionMap_Moving, out SO_Input_Base<Vector2> so))
        {
            Debug.Log("If you pass here it's because the type is not the good one");
            return;
        }

        so.SetReceiveBridgeValue(MouvementAction, true);
        so.OnActived.SetRecieveValue(Active, true);
        so.OnPerformed.SetRecieveValue(MouvementAction2, true);
    }

    public void MouvementAction(Vector2 value)
    {
        Debug.Log("Value receive: " + value);
    }

    public void MouvementAction2(Vector2 value, string deviceName)
    {
        Debug.Log("Value receive: " + value + " Device: " + deviceName);
    }

    public void Active(bool pressed, float time, string deviceName)
    {
        if (pressed) { Debug.Log("Start Device: " + deviceName); }
        else { Debug.Log("Stop Device: " + deviceName); }
    }
}
