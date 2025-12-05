using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Main_Menu : MonoBehaviour
{
    

    [SerializeField] GameObject credits;
    
    bool isVisible = false;

    private void OnEnable()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        if(SoundEffectTrigger.Instance != null)
        {
            SoundEffectTrigger.Instance.StopAllCoroutines();
        }
    }

    /// <summary>
    /// Start game by loading first scene
    /// </summary>
    public void StartGame(GameObject cinematicCanvas)
    {
        cinematicCanvas.SetActive(true);
        //SceneManager.LoadScene(1);
        //EventBus<LoadLevel>.Raise(new LoadLevel { newLevel = _startLevel });
        //Cursor.lockState = CursorLockMode.Locked;
        //Cursor.visible = false;
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
