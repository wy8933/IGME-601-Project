using System.Collections.Generic;
using UnityEngine;

public class LightSection : MonoBehaviour
{
    public string sectionId = "DefaultSection";

    [SerializeField] private List<Light> _lights = new List<Light>();

    private void Awake()
    {
        // If no lights assigned manually, auto-grab all child lights
        if (_lights.Count == 0)
        {
            GetComponentsInChildren(true, _lights);
        }

        // Register with the manager
        LightingSectionManager.RegisterSection(this);
    }

    private void OnDestroy()
    {
        LightingSectionManager.UnregisterSection(this);
    }

    public void SetSectionLights(bool on)
    {
        foreach (var l in _lights)
        {
            if (l != null)
                l.enabled = on;
        }
    }

    public void TurnOn() => SetSectionLights(true);
    public void TurnOff() => SetSectionLights(false);
}
