using System.Collections;
using AudioSystem;
using UnityEngine;

public class SoundEffectTrigger : MonoBehaviour
{
    public static SoundEffectTrigger Instance { get; private set; }

    [SerializeField] private SoundDataSO[] footsteps;
    private Coroutine footstepsCoroutine;
    private bool isPlayingFootsteps;

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

    private IEnumerator FootstepsCoroutine(float _delay)
    {
        isPlayingFootsteps = true;

        while (true)
        {
            AudioManager.Instance.Play(footsteps[0]); 
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
}