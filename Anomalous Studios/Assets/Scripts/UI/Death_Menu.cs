using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DeathMenu : MonoBehaviour
{
    /// <summary>
    /// Return to Main Menu
    /// </summary>
    public void MainMenu()
    {
        EventBus<LoadLevel>.Raise(new LoadLevel { newLevel = Level.mainMenu });
    }

    /// <summary>
    /// TO DO: Add persistent logic
    /// For now plays first scene
    /// </summary>
    public void Restart()
    {
        EventBus<LoadLevel>.Raise(new LoadLevel { newLevel = Level.B1 });
    }
    /// <summary>
    /// Exit to desktop or back to editor if in unity
    /// </summary>
    public void QuitGame()
    {
#if UNITY_EDITOR
        EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

}
