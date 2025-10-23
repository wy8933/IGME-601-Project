using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using UnityEngine;

[Serializable]
public class VariableEntry { public string key; public string value; }

[Serializable]
public class VariableSnapshot
{
    public List<VariableEntry> entries = new();
    public string savedUtc;
}

[Serializable]
public class ConditionRow
{
    public string key;
    public VariableCompareType compareType = VariableCompareType.Equals;
    public string value;
}

public class SaveSystemTest : MonoBehaviour
{
    [Header("Save Profile")]
    [Tooltip("List of keys to persist.")]
    [SerializeField] private SaveProfileSO saveKeyProfile;

    [Header("File")]
    [Tooltip("Relative save path under persistentDataPath.")]
    [SerializeField] private string relativePath = "/variables.json";

    [Header("Defaults")]
    [Tooltip("Default values seeded on Start if missing (only for keys present in the save profile).")]
    [SerializeField]
    private List<VariableEntry> defaultSeed = new()
    {
        new() { key = "player.id", value = "tester"},
        new() { key = "player.health", value = "100"},
        new() { key = "can-save", value = "false"},
    };

    [Header("Behavior")]
    [Tooltip("Use encryption for save/load.")]
    [SerializeField] private bool encryptionEnabled = true;

    [SerializeField]
    private List<ConditionRow> conditionsToAllowSave = new()
    {
        new ConditionRow { key = "can-save", compareType = VariableCompareType.Equals, value = "true" }
    };

    private readonly IDataService dataService = new JsonDataService();
    private string FullPath => Application.persistentDataPath + relativePath;


    private void Awake()
    {
        if (saveKeyProfile == null)
        {
            Debug.LogError("Missing SaveKeyProfileSO. Create one and assign it.");
        }
    }

    private void Start()
    {
        // Reflect encryption flag for rules
        GameVariables.Set("can-save", encryptionEnabled ? "true" : "false");

        // Seed defaults only for keys in the save profile that are missing
        if (saveKeyProfile != null)
        {
            var allowed = new HashSet<string>(saveKeyProfile.GetAllKeys());
            foreach (var entry in defaultSeed)
            {
                if (string.IsNullOrWhiteSpace(entry.key)) continue;
                if (!allowed.Contains(entry.key)) continue;

                if (!VariableConditionManager.Instance.TryGet(entry.key, out var existing) || string.IsNullOrEmpty(existing))
                {
                    GameVariables.Set(entry.key, entry.value ?? "");
                    Debug.Log($"[Seed] {entry.key} = {entry.value}");
                }
            }
        }

        Debug.Log($"persistentDataPath = {Application.persistentDataPath}");
        Debug.Log($"target file = {FullPath}");
        LogLiveVariables();
    }


    [ContextMenu("Save")]
    public void SaveOnce()
    {
        if (saveKeyProfile == null) { Debug.LogError("No SaveKeyProfile assigned."); return; }

        var conds = new List<VariableCondition>();
        foreach (var c in conditionsToAllowSave)
            conds.Add(new VariableCondition(c.key, c.compareType, c.value));

        bool ok = VariableConditionManager.Instance.CheckAll(conds);
        Debug.Log($"conditions pass? {ok}");
        if (!ok) return;
        

        var snap = BuildSnapshot(saveKeyProfile.GetAllKeys());

        // Ensure folder exists
        var dir = Path.GetDirectoryName(FullPath);
        if (!string.IsNullOrEmpty(dir)) Directory.CreateDirectory(dir);

        // Time with DateTime ticks
        long t0 = DateTime.UtcNow.Ticks;
        bool okSave = dataService.SaveData(relativePath, snap, encryptionEnabled);
        double ms = (DateTime.UtcNow.Ticks - t0) / (double)TimeSpan.TicksPerMillisecond;

        Debug.Log(okSave
            ? $"OK ({ms:N3} ms). Wrote {snap.entries.Count} keys. Encrypted={encryptionEnabled}"
            : "ERROR");

        if (okSave)
        {
            Debug.Log($"Preview JSON:\n{JsonConvert.SerializeObject(snap, Formatting.Indented)}");
        }
    }

    [ContextMenu("Load")]
    public void LoadOnce()
    {
        if (saveKeyProfile == null) { Debug.LogError("No SaveKeyProfile assigned."); return; }

        try
        {
            long t0 = DateTime.UtcNow.Ticks;
            var snap = dataService.LoadData<VariableSnapshot>(relativePath, encryptionEnabled);
            double ms = (DateTime.UtcNow.Ticks - t0) / (double)TimeSpan.TicksPerMillisecond;

            ApplySnapshot(snap);
            Debug.Log($"OK ({ms:N3} ms). Applied {snap.entries.Count} keys. Encrypted={encryptionEnabled}");
            Debug.Log($"File JSON:\n{JsonConvert.SerializeObject(snap, Formatting.Indented)}");
            LogLiveVariables();
        }
        catch (Exception e)
        {
            Debug.LogError($"ERROR: {e.Message}");
        }
    }

    [ContextMenu("Clear File")]
    public void ClearFile()
    {
        if (File.Exists(FullPath))
        {
            File.Delete(FullPath);
            Debug.Log("Deleted snapshot.");
        }
        else
        {
            Debug.Log("Nothing to delete.");
        }
    }

    [ContextMenu("Toggle Encryption Flag")]
    public void ToggleEncryptionFlag()
    {
        encryptionEnabled = !encryptionEnabled;
        GameVariables.Set("save.encrypted", encryptionEnabled ? "true" : "false");
        Debug.Log($"encryptionEnabled = {encryptionEnabled}");
    }


    private VariableSnapshot BuildSnapshot(IEnumerable<string> keysToPersist)
    {
        var snap = new VariableSnapshot { savedUtc = DateTime.UtcNow.ToString("o") };

        foreach (var key in keysToPersist)
        {
            if (string.IsNullOrWhiteSpace(key)) continue;

            if (VariableConditionManager.Instance.TryGet(key, out var value))
                snap.entries.Add(new VariableEntry { key = key, value = value });
            else
                snap.entries.Add(new VariableEntry { key = key, value = "" });
        }

        //stamp save time
        GameVariables.Set("save.time.utc", snap.savedUtc);
        GameVariables.Set("save.time.ticks", DateTime.UtcNow.Ticks.ToString());

        return snap;
    }

    private void ApplySnapshot(VariableSnapshot snap)
    {
        if (snap == null || snap.entries == null) return;
        foreach (var e in snap.entries)
        {
            if (!string.IsNullOrEmpty(e.key))
                GameVariables.Set(e.key, e.value ?? "");
        }
    }

    private void LogLiveVariables()
    {
        if (saveKeyProfile == null) return;

        var live = new VariableSnapshot { savedUtc = DateTime.UtcNow.ToString("o") };
        foreach (var key in saveKeyProfile.GetAllKeys())
        {
            if (VariableConditionManager.Instance.TryGet(key, out var v))
                live.entries.Add(new VariableEntry { key = key, value = v });
            else
                live.entries.Add(new VariableEntry { key = key, value = "" });
        }
        Debug.Log($"\n{JsonConvert.SerializeObject(live, Formatting.Indented)}");
    }
}
