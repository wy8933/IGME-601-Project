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
    [Header("Rulekeeper SFX")]
    [SerializeField] private SoundDataSO _ruleKeeperScream;
    [SerializeField] private SoundDataSO _ruleKeeperAmbience;
    private Coroutine ambienceCoroutine;
    private bool isPlayingAmbience;
    [Header("Door SFX")]
    [SerializeField] private SoundDataSO _doorOpen;
    [SerializeField] private SoundDataSO _doorClose;
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

    /// <summary>
    /// Coroutine for footsteps
    /// </summary>
    /// <param name="_delay">Time between each step (float)</param>
    /// <returns></returns>
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
    /// Coroutine for rulekeeper ambient sounds
    /// </summary>
    /// <param name="_delay"></param>
    /// <returns></returns>
    private IEnumerator AmbienceCoroutine(float _delay, Transform _ruleKeeperTransform)
    {
        isPlayingAmbience = true;

        while (true)
        {
            _audioManager.Stop(_audioManager.gameObject, _ruleKeeperAmbience);
            _audioManager.Play(_ruleKeeperAmbience, _ruleKeeperTransform.position);
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
    /// Plays when a UI item is clicked on
    /// </summary>
    public void PlayUIClick()
    {
        AudioManager.Instance.Play(_uiClick);
    }

    /// <summary>
    /// Playes once when a UI item is hovered over
    /// </summary>
    public void PlayUIHover()
    {
       AudioManager.Instance.Play(_uiHover);
    }

    /// <summary>
    /// Plays from the rulekeeper when a rule is broken
    /// </summary>
    public void PlayScream(Transform _ruleKeeperTransform)
    {
        _audioManager.Play(_ruleKeeperScream, _ruleKeeperTransform.position);
    }

    /// <summary>
    /// Plays from the rulekeeper constantly
    /// </summary>
    public void PlayAmbience(Transform _ruleKeeperTransform)
    {
        if (!isPlayingFootsteps)
        {
            ambienceCoroutine = StartCoroutine(AmbienceCoroutine(7, _ruleKeeperTransform));
        }
    }

    /// <summary>
    /// Stops the ambience from the rulekeeper
    /// </summary>
    public void StopAmbience()
    {
        if (isPlayingAmbience && ambienceCoroutine != null)
        {
            StopCoroutine(ambienceCoroutine);
            ambienceCoroutine = null;
            isPlayingAmbience = false;
        }
    }
}