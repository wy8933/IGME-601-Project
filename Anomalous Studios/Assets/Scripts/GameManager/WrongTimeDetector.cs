using UnityEngine;

public class WrongTimeDetector : MonoBehaviour
{

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) 
        {
            if (VariableConditionManager.Instance.Get("WrongTime") == "true") 
            {
                GameVariables.Set("enter-room-in-wrong-time:string","true");
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            GameVariables.Set("enter-room-in-wrong-time:string", "false");
        }
    }
}
