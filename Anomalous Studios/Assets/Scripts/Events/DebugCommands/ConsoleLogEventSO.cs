using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(fileName = "ConsoleLogEventSO", menuName = "Events/Commands/ConsoleLogEventSO")]
public sealed class ConsoleLogEventSO : BaseEventSO<ConsoleLog>
{
    [Header("Buffer")]
    [SerializeField] private int maxLines = 500;

    [SerializeField] private List<string> _lines = new List<string>();

    public IReadOnlyList<string> Lines => _lines;

    protected override void OnEvent(ConsoleLog e)
    {
        var line = $"[{e.Type}] {e.Message}";
        _lines.Add(line);
        if (_lines.Count > maxLines) _lines.RemoveAt(0);
    }

    public void Reset()
    {
        _lines = new List<string>();
    }
}
