using UnityEditor;
using UnityEngine;
using UnityEngine.Video;

public class Main_Menu : MonoBehaviour
{
    public VideoPlayer player;
    public GameObject MainMenuUI;

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

    public void Update()
    {
        if (Input.anyKeyDown) 
        {
            player.Stop();
            MainMenuUI.SetActive(true);
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
        //isVisible = !isVisible;
        //credits.SetActive(isVisible);
        player.Play();
        MainMenuUI.SetActive(false);
    }

}
