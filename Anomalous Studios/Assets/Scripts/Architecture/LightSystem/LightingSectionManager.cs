using System.Collections.Generic;
using UnityEngine;

public class LightingSectionManager : MonoBehaviour
{
    public static LightingSectionManager Instance;
    private static readonly Dictionary<string, LightSection> _sections = new();

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

    public static void RegisterSection(LightSection section)
    {
        if (section == null || string.IsNullOrEmpty(section.sectionId))
            return;

        _sections[section.sectionId] = section;
    }

    public static void UnregisterSection(LightSection section)
    {
        if (section == null || string.IsNullOrEmpty(section.sectionId))
            return;

        if (_sections.ContainsKey(section.sectionId))
            _sections.Remove(section.sectionId);
    }

    public static void SetSection(string sectionId, bool isOn)
    {
        if (string.IsNullOrEmpty(sectionId))
            return;

        if (_sections.TryGetValue(sectionId, out var section))
        {
            section.SetSectionLights(isOn);
        }
        else
        {
            Debug.LogWarning($"No section found with id: {sectionId}");
        }
    }

    public static void TurnOn(string sectionId) => SetSection(sectionId, true);
    public static void TurnOff(string sectionId) => SetSection(sectionId, false);

    public static void TurnOffAll()
    {
        foreach (var section in _sections.Values)
        {
            section?.SetSectionLights(false);
        }
    }

    public static void TurnOnAll()
    {
        foreach (var section in _sections.Values)
        {
            section?.SetSectionLights(true);
        }
    }
}
