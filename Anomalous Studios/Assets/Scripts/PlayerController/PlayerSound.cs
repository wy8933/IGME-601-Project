using AudioSystem;
using UnityEngine;

public class PlayerSound : MonoBehaviour
{
    [Header("Sound Data")]
    [SerializeField] private SoundDataSO SprintSlowSO;
    [SerializeField] private SoundDataSO SprintMedSO;
    [SerializeField] private SoundDataSO SprintFastSO;
    private float _audioCooldownTime = 0.5f;
    private float lastPlayTime;
    
    /// <summary>
    /// Plays an audio sound and prevents audio clip from spamming
    /// </summary>
    /// <param name="sd">Sound Data Scriptable Object</param>
    public void PlaySound(SoundDataSO sd)
    {
        if (Time.time - lastPlayTime >= _audioCooldownTime)
        {
            if (AudioManager.Instance)
            {
                AudioManager.Instance.Play(sd, this.transform.position);
            }
            lastPlayTime = Time.time;
        }
    }

    /// <summary>
    /// Plays the appropriate audio file depending on player's current stamina when running
    /// </summary>
    /// <param name="InStamina">Input Stamina Value</param>
    public void SprintPantingDepletionSFX(float InStamina)
    {
        if (InStamina > 66.0f)
        {
            PlaySound(SprintSlowSO);
        }
        else if (InStamina > 33.0f)
        {
            AudioManager.Instance.Stop(gameObject, SprintSlowSO);
            PlaySound(SprintMedSO);
        }
        else if (InStamina > 0)
        {
            AudioManager.Instance.Stop(gameObject, SprintMedSO);
            PlaySound(SprintFastSO);
        }
    }

    /// <summary>
    /// Plays the appropriate audio file depending on player's current stamina when not running
    /// </summary>
    /// <param name="InStamina">Input Stamina Value</param>
    public void SprintPantingRegenSFX(float InStamina)
    {
        if (InStamina <= 33.0f)
        {
            PlaySound(SprintFastSO);
        }
        else if (InStamina <= 66.0f)
        {
            AudioManager.Instance.Stop(gameObject, SprintFastSO);
            PlaySound(SprintMedSO);
        }
        else if (InStamina < 100)
        {
            AudioManager.Instance.Stop(gameObject, SprintMedSO);
            PlaySound(SprintSlowSO);
        }
        else
        {
            AudioManager.Instance.Stop(gameObject, SprintSlowSO);
        }
    }
}
