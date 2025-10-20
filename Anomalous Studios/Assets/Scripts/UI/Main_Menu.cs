using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Main_Menu : MonoBehaviour
{
    [Tooltip("Where to go from the main menu, this should be the first level in the list")]
    [SerializeField] private Level _startLevel;

    [SerializeField] GameObject credits;
    
    bool isVisible = false;

    /// <summary>
    /// Start game by loading first scene
    /// </summary>
    public void StartGame()
    {
        //SceneManager.LoadScene(1);
        EventBus<LoadLevel>.Raise(new LoadLevel { newLevel = _startLevel });
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
