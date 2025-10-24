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

    private IEnumerator FootstepsCoroutine()
    {
        isPlayingFootsteps = true;

        while (true)
        {
            AudioManager.Instance.Play(footsteps[Random.Range(0,footsteps.Length)]);
            yield return new WaitForSeconds(0.5f); // Adjust timing for speed
        }
    }

    public void PlayFootsteps()
    {
        if (!isPlayingFootsteps)
        {
            footstepsCoroutine = StartCoroutine(FootstepsCoroutine());
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