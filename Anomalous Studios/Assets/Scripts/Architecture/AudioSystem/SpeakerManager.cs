using AudioSystem;
using UnityEngine;

public class SpeakerManager : MonoBehaviour
{
    [SerializeField] GameObject[] Speakers;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // Begins music on all speakers
        foreach (var speaker in Speakers)
        {
            SoundEffectTrigger.Instance.PlayElevatorMusic(speaker.transform);
        }
    }

    /// <summary>
    /// Cuts music in level changing it to static noise
    /// </summary>
    public void StartStatic()
    {
        foreach (var speaker in Speakers)
        {
            SoundEffectTrigger.Instance.PlayElevatorMusic(speaker.transform);
        }
    }
}
