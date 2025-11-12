using UnityEngine;


[CreateAssetMenu(fileName = "QuitGameCommandEventSO", menuName = "Events/Commands/QuitGameCommandEventSO")]
public sealed class QuitGameCommandEventSO : BaseEventSO<CommandRequested>
{
    protected override void OnEvent(CommandRequested e)
    {
        Application.Quit();
    }
}