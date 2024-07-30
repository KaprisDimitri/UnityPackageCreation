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
       InputManager.Instance.EnableAction(GameActionsEnum.ExempleActionMap_Moving, true);

        mouvement.OnActived.SetRecieveValue(Active, true);
        mouvement.OnPerformed.SetRecieveValue(MouvementAction2, true);
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
    public void MouvementAction2(Vector2 value, string device)
    {
        Debug.Log("je bouge value: " + value + " Avec le device: " + device);
    }

    public void Active (bool pressed, float time, string deviceName)
    {
        if (pressed) { Debug.Log("je Commence a Bouger avec le device: " + deviceName); }
        else { Debug.Log("j'arret de bouger: " + deviceName); }
    }

    IEnumerator Coroutine01 ()
    {
        yield return new WaitForSeconds(30);
        Debug.Log("je set la valeur a a false man tu devrais rien recevoir");
      //  InputManager.Instance.EnableAction(GameActionsEnum.FirstPersonActionMap_MovingAction, false);
        /* Debug.Log("j ai attendu 1 je desactive");
         InputManager.Instance.EnableAction(GameActionsMapsNames.GameActionsEnum.FirstPersonActionMap_MovingAction, false);

         yield return new WaitForSeconds(30);
         Debug.Log("j ai attendu 1 je active");
         InputManager.Instance.EnableAction(GameActionsMapsNames.GameActionsEnum.FirstPersonActionMap_MovingAction, true);*/
    }
}
