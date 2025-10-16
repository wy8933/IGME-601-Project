using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Main_Menu : MonoBehaviour
{
    [SerializeField] GameObject credits;
    bool isVisible = false;

    /// <summary>
    /// Start game by loading first scene
    /// </summary>
    public void StartGame()
    {
        //SceneManager.LoadScene(1);
        EventBus<LoadLevel>.Raise(new LoadLevel { newLevel = LevelTESTING.B1 });
    }
    public void QuitGame()
    {
#if UNITY_EDITOR
        EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    public void ToggleCredits()
    {
        isVisible = !isVisible;
        credits.SetActive(isVisible);
    }

}
