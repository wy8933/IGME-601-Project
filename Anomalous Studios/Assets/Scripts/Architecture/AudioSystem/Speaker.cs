using AudioSystem;
using Unity.AppUI.UI;
using UnityEngine;

public class Speaker : MonoBehaviour
{
    // Sounds
    [SerializeField] SoundDataSO elevatorMusicSO;
    [SerializeField] SoundDataSO staticSO;

    /// <summary>
    /// Plays static - called by speaker manager
    /// </summary>
    public void PlayStatic()
    {
        AudioManager.Instance.Play(staticSO, transform.position);
    }

    /// <summary>
    /// Plays music - called by speaker manager
    /// </summary>
    public void PlayMusic()
    {
        Debug.Log("<color=red>Speaker Playing " + this + "</color>");
        AudioManager.Instance.Play(elevatorMusicSO, gameObject, transform.position);
    }
}
