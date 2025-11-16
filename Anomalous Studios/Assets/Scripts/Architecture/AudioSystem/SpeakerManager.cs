using AudioSystem;
using Unity.AppUI.UI;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class SpeakerManager : MonoBehaviour
{
    public static SpeakerManager Instance { get; private set; }
    [SerializeField] List<Speaker> _speakers = new List<Speaker>();
    

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    void OnEnable()
    {
        // Subscribe to the sceneLoaded event
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        // Unsubscribe from the sceneLoaded event
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    /// <summary>
    /// This method will be called whenever a scene loads
    /// </summary>
    /// <param name="scene"></param>
    /// <param name="mode"></param>
    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        _speakers.Clear();
        foreach(GameObject obj in GameObject.FindGameObjectsWithTag("Speaker"))
        {
            _speakers.Add(obj.GetComponent<Speaker>());
        }
        
        Debug.Log("<color=red>Speaker Count " + _speakers.Count + "</color>");
        
        StartMusic();
    }

    /// <summary>
    /// Plays static noise when rule is broken
    /// </summary>
    public void StartStatic()
    {
        foreach (Speaker speaker in _speakers)
        {
            speaker.PlayStatic();
        }
    }

    /// <summary>
    /// Plays music when level is loaded 
    /// </summary>
    public void StartMusic()
    {
        foreach (Speaker speaker in _speakers)
        {
            speaker.PlayMusic();
        }
    }
}
