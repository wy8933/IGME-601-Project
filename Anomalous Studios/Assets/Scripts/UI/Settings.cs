using UnityEngine;
using UnityEngine.UI;

public class Settings : MonoBehaviour
{
    [SerializeField] Slider[] sliders;

    /// <summary>
    /// Called each time the settings menu is enabled.
    /// </summary>
    private void OnEnable()
    {
        GetPrefs();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    /// <summary>
    /// Sets all the sliders to their saved values
    /// </summary>
    public void GetPrefs()
    {
        foreach (var slider in sliders)
        {
            slider.value = GetPlayerPrefs(slider.name);
        }
    }
    
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
    }

    public void SetSlider(Slider slider)
    {
        SetPlayerPrefs(slider.name, slider.value);
    }
    #endregion
}
