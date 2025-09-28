using UnityEngine;

public class RuleSystemTester : MonoBehaviour
{
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1)) GameVariables.Set("player.health:int", "30");
        if (Input.GetKeyDown(KeyCode.Alpha2)) GameVariables.Set("player.health:int", "24");
        if (Input.GetKeyDown(KeyCode.Alpha3)) GameVariables.Set("player.health:int", "20");
        if (Input.GetKeyDown(KeyCode.Alpha4)) GameVariables.Set("player.health:int", "19");
        if (Input.GetKeyDown(KeyCode.Alpha5)) GameVariables.Set("player.health:int", "-1");
        if (Input.GetKeyDown(KeyCode.Alpha6)) GameVariables.Set("player.health:int", "30");
    }

}
