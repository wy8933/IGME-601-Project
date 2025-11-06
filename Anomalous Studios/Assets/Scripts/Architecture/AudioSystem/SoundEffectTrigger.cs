using System.Collections;
using AudioSystem;
using Newtonsoft.Json;
using UnityEngine;

public class SoundEffectTrigger : MonoBehaviour
{
    public static SoundEffectTrigger Instance { get; private set; }
    AudioManager _audioManager;
    [Header("Footstep SFX")]
    [SerializeField] private SoundDataSO _footstep;
    private Coroutine footstepsCoroutine;
    private bool isPlayingFootsteps;
    [Header("Door SFX")]
    [SerializeField] private SoundDataSO _doorOpen;
    [SerializeField] private SoundDataSO _doorClose;
    [Header("Elevator SFX")]             
    [SerializeField] private SoundDataSO _elevatorMusic;
    [Header("UI SFX")]
    [SerializeField] private SoundDataSO _uiClick;
    [SerializeField] private SoundDataSO _uiHover;
    [SerializeField] private SoundDataSO _uiPageFlip;


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

    /// <summary>
    /// Gets instance of audio manager
    /// </summary>
    private void Start()
    {
        _audioManager = AudioManager.Instance;
    }

    private IEnumerator FootstepsCoroutine(float _delay)
    {
        isPlayingFootsteps = true;

        while (true)
        {
            _audioManager.Play(_footstep); 
            yield return new WaitForSeconds(_delay); // Adjust timing for speed
        }
    }

    /// <summary>
    /// Plays walking sound repeatedly
    /// </summary>
    /// <param name="_delay"></param>
    public void PlayFootsteps(float _delay)
    {
        if (!isPlayingFootsteps)
        {
            footstepsCoroutine = StartCoroutine(FootstepsCoroutine(_delay));
        }
    }

    /// <summary>
    /// Stops walking sound
    /// </summary>
    public void StopFootsteps()
    {
        if (isPlayingFootsteps && footstepsCoroutine != null)
        {
            StopCoroutine(footstepsCoroutine);
            footstepsCoroutine = null;
            isPlayingFootsteps = false;
        }
    }

    /// <summary>
    /// Plays door opening sound after interacting with unlcoked door
    /// </summary>
    /// <param name="_door"></param>
    public void PlayDoorOpen(Transform _door)
    {
        _audioManager.Play(_doorOpen, _door.position);
    }

    /// <summary>
    /// Plays music from all speakers in scene
    /// </summary>
    /// <param name="_speaker"></param>
    public void PlayElevatorMusic(Transform _speaker)
    {
        _audioManager.Play(_elevatorMusic, _speaker.position);
    }

    /// <summary>
    /// Plays when a UI item is clicked on
    /// </summary>
    public void PlayUIClick()
    {
        _audioManager.Play(_uiClick);
    }

    /// <summary>
    /// Playes once when a UI item is hovered over
    /// </summary>
    public void PlayUIHover()
    {
        _audioManager.Play(_uiHover);
    }
}