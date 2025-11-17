using UnityEngine;
using TMPro;

public class GameOverStatsHandler : MonoBehaviour
{
    public TMP_Text totalRuleBroken;
    public TMP_Text totalTaskComplete;


    public void Start()
    {
        totalRuleBroken.text = $"Total Number Of Policies Broken: {VariableConditionManager.Instance.Get("rule_broken_count:int")}" ;
        totalTaskComplete.text = $"Total Number Of Tasks Completed: {VariableConditionManager.Instance.Get("task_completed:int")}";

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

}
