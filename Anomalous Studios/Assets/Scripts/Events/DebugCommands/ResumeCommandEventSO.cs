using UnityEngine;


[CreateAssetMenu(fileName = "ResumeCommandEventSO", menuName = "Events/Commands/ResumeCommandEventSO")]
public sealed class ResumeCommandEventSO : BaseEventSO<CommandRequested>
{
    protected override void OnEvent(CommandRequested e)
    {
        Time.timeScale = 1;
    }
}