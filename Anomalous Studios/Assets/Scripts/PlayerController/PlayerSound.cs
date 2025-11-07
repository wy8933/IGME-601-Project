using AudioSystem;
using UnityEngine;

public class PlayerSound : MonoBehaviour
{
    [Header("Sound Data")]
    [SerializeField] private SoundDataSO BreathingShort;
    [SerializeField] private SoundDataSO BreathingMedium;
    [SerializeField] private SoundDataSO BreathingLong;
    [SerializeField] private SoundDataSO Running;
    private float _audioCooldownTime = 0.5f;
    private float lastPlayTime;
    
    /// <summary>
    /// Plays an audio sound and prevents audio clip from spamming
    /// </summary>
    /// <param name="sd">Sound Data Scriptable Object</param>
    public void PlaySound(SoundDataSO sd)
    {
        AudioManager.Instance.Play(sd, this.transform.position);
        //if (Time.time - lastPlayTime >= _audioCooldownTime)
        //{
        //    if (AudioManager.Instance)
        //    {
        //        AudioManager.Instance.Play(sd, this.transform.position);
        //    }
        //    lastPlayTime = Time.time;
        //}
    }

    /// <summary>
    /// Plays the appropriate audio file depending on player's current stamina when running
    /// </summary>
    /// <param name="InStamina">Input Stamina Value</param>
    public void SprintPantingDepletionSFX()
    {
        AudioManager.Instance.Stop(gameObject, BreathingShort);
        AudioManager.Instance.Stop(gameObject, BreathingMedium);
        AudioManager.Instance.Stop(gameObject, BreathingLong);
        PlaySound(Running);
    }

    /// <summary>
    /// Plays the appropriate audio file depending on player's current stamina when not running
    /// </summary>
    /// <param name="InStamina">Input Stamina Value</param>
    public void SprintPantingRegenSFX(float InStamina)
    {
        //AudioManager.Instance.Stop(gameObject, Running);
        if (InStamina <= 33.0f)
        {
            PlaySound(BreathingLong);
            print("breathing long");
        }
        else if (InStamina <= 66.0f)
        {
            PlaySound(BreathingMedium);
            print("breathing medium");

        }
        else if (InStamina < 100)
        {
            PlaySound(BreathingShort);
            print("breathing short");

        }
    }
}
