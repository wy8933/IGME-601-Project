using UnityEngine;
using UnityEngine.SceneManagement;


[CreateAssetMenu(fileName = "GoToSceneCommandSO", menuName = "Events/Commands/GoToSceneCommandSO")]
public sealed class GoToSceneCommandSO : BaseEventSO<CommandRequested>
{
    protected override void OnEvent(CommandRequested e)
    {
        if (SceneManager.GetSceneByName(e.Args[0]).IsValid()) 
        {
            SceneManager.LoadScene(e.Args[0]);
        }

    }
}