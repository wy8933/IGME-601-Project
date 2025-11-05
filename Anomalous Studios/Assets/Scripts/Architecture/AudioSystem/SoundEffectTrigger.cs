using System.Collections;
using AudioSystem;
using UnityEngine;

public class SoundEffectTrigger : MonoBehaviour
{
    public static SoundEffectTrigger Instance { get; private set; }
    AudioManager _audioManager;
    [Header("Footstep SFX")]
    [SerializeField] private SoundDataSO[] footsteps;
    private Coroutine footstepsCoroutine;
    private bool isPlayingFootsteps;
    [Header("Door SFX")]
    [SerializeField] private SoundDataSO doorOpen;
    [SerializeField] private SoundDataSO doorClose;
    [Header("Elevator SFX")]
    [SerializeField] private SoundDataSO elevatorDoorOpen;
    [SerializeField] private SoundDataSO elevatorDoorClose;
    [SerializeField] private SoundDataSO elevatorChime;
    [SerializeField] private SoundDataSO elevatorMusic;
    

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

    private void Start()
    {
        _audioManager = AudioManager.Instance;
    }

    private IEnumerator FootstepsCoroutine(float _delay)
    {
        isPlayingFootsteps = true;

        while (true)
        {
            _audioManager.Play(footsteps[0]); 
            yield return new WaitForSeconds(_delay); // Adjust timing for speed
        }
    }

    public void PlayFootsteps(float _delay)
    {
        if (!isPlayingFootsteps)
        {
            footstepsCoroutine = StartCoroutine(FootstepsCoroutine(_delay));
        }
    }

    public void StopFootsteps()
    {
        if (isPlayingFootsteps && footstepsCoroutine != null)
        {
            StopCoroutine(footstepsCoroutine);
            footstepsCoroutine = null;
            isPlayingFootsteps = false;
        }
    }

    public void PlayDoorOpen(Transform _door)
    {
        _audioManager.Play(doorOpen, _door.position);
    }

    public void PlayDoorClose(Transform _door)
    {
        _audioManager.Play(doorOpen, _door.position);
    }
}