using AudioSystem;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;
using Slider = UnityEngine.UI.Slider;
using Toggle = UnityEngine.UI.Toggle;

public class Settings : MonoBehaviour
{
    [SerializeField] Slider[] sliders;
    [SerializeField] Toggle[] toggles;
    [SerializeField] private PlayerController playerController;
    [SerializeField] private AudioManager audioManager;
    /// <summary>
    /// Called each time the settings menu is enabled.
    /// </summary>
    private void OnEnable()
    {
        GetPrefs();
        SetSliders();
    }

    /// <summary>
    /// Sets all the UI Elements to their saved values
    /// </summary>
    public void GetPrefs()
    {
        // Set each of the sliders to the saved preferences
        foreach (var slider in sliders)
        {
            slider.value = GetPlayerPrefs(slider.name);
        }

        //// Sets the value of each toggle active in the list 
        //for (int i = 0; i < toggles.Length; i++)
        //{
        //    Toggle toggle = toggles[i];
        //    if (GetPlayerPrefs(toggle.name) % 1 == 0)
        //    {
        //        toggle.isOn = true;
        //        Debug.Log(toggle.name + " Slider Muted");
        //    }
        //    else
        //    {
        //        toggle.isOn = false;
        //    }
        //}
    }
 
    #region Slider Methods
    /// <summary>
    /// Adjusts all the sliders based on their saved preferences
    /// </summary>
    void SetSliders()
    {
        AdjustSFXSlider(GetPlayerPrefs("VolumeSlider"));
        AdjustSensitivityX(GetPlayerPrefs("SensitivityXSlider"));
        AdjustSensitivityY(GetPlayerPrefs("SensitivityYSlider"));
    }

    /// <summary>
    /// Adjusts the player's x sensitivity
    /// </summary>
    /// <param name="value"></param>
    public void AdjustSensitivityX(float value)
    {
        playerController.MouseSensitivityX = value;
    }

    /// <summary>
    /// Adjusts player's y sensitivity
    /// </summary>
    /// <param name="value"></param>
    public void AdjustSensitivityY(float value)
    {
        playerController.MouseSensitivityY = value;
    }

    /// <summary>
    /// Adjust the slider for sound effects
    /// </summary>
    /// <param name="value"></param>
    public void AdjustSFXSlider(float value)
    {
        audioManager.sfxAudioMixerGroup.audioMixer.SetFloat("Volume", Mathf.Log10(value) * 20);
        if(value == 0)
        {
            toggles[0].isOn = true;
        }
        else
        {
            toggles[0].isOn = false;
        }
    }
    #endregion

    #region Helper Methods
    /// <summary>
    /// Helper method for getting player preference values
    /// </summary>
    /// <param name="pref"></param>
    private float GetPlayerPrefs(string pref)
    {
        return PlayerPrefs.GetFloat(pref);
    }

    /// <summary>
    /// Helper method for setting player preference values
    /// </summary>
    /// <param name="pref"></param>
    /// <param name="value"></param>
    public void SetPlayerPrefs(string pref, float value)
    {
        PlayerPrefs.SetFloat(pref, value);
        PlayerPrefs.Save();
    }

    /// <summary>
    /// Helper method for setting slider values
    /// </summary>
    /// <param name="slider"></param>
    public void SetSlider(Slider slider)
    {
        SetPlayerPrefs(slider.name, slider.value);
    }
    #endregion
}
