
using UnityEngine;

[CreateAssetMenu(fileName = "PaperDataSO", menuName = "Scriptable Objects/PaperDataSO")]
public sealed class PaperDataSO : ScriptableObject
{
    public string Description;
    [Tooltip("If isTask is enabled, references the associated task in the handbook. ")]
    public string TaskID;
    public bool IsTask;
}