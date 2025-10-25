
using UnityEngine;

[CreateAssetMenu(fileName = "PaperDataSO", menuName = "Scriptable Objects/PaperDataSO")]
public sealed class PaperDataSO : ScriptableObject
{
    public string Description { get; private set; }
    [Tooltip("If isTask is enabled, completes the task in the Handbook.")]
    public string TaskID { get; private set; }
    public bool IsTask { get; private set; }


}