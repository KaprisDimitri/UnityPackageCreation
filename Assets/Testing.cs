using System.Collections;
using UnityEngine;
using static GameActionsMapsNames;

public class Testing : MonoBehaviour
{
    [SerializeField] private SO_Input_Vector2 mouvement;

    // Start is called before the first frame update
    void Start()
    {
        mouvement.SetReceiveBridgeValue(MouvementAction, true);
        mouvement.SetReceiveBridgeValue(MouvementAction, false);
        InputManager.Instance.EnableAction(GameActionsEnum.FirstPersonActionMap_MovingAction2, true);

        mouvement.OnActived.SetRecieveValue(Active, true);
        StartCoroutine(Coroutine01()); 
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void MouvementAction (Vector2 value)
    {
        Debug.Log("je bouge value: " + value);
    }

    public void Active (bool pressed, float time, string deviceName)
    {
        if (pressed) { Debug.Log("je Commence a Bouger avec le device: " + deviceName); }
        else { Debug.Log("j'arret de bouger: " + deviceName); }
    }

    IEnumerator Coroutine01 ()
    {
        yield return new WaitForSeconds(30);
       /* Debug.Log("j ai attendu 1 je desactive");
        InputManager.Instance.EnableAction(GameActionsMapsNames.GameActionsEnum.FirstPersonActionMap_MovingAction, false);

        yield return new WaitForSeconds(30);
        Debug.Log("j ai attendu 1 je active");
        InputManager.Instance.EnableAction(GameActionsMapsNames.GameActionsEnum.FirstPersonActionMap_MovingAction, true);*/
    }
}
