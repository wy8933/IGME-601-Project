using UnityEngine;


[CreateAssetMenu(fileName = "PauseCommandEventSO", menuName = "Events/Commands/PauseCommandEventSO")]
public sealed class PauseCommandEventSO : BaseEventSO<CommandRequested>
{
    protected override void OnEvent(CommandRequested e)
    {
        Time.timeScale = 0;
    }
}