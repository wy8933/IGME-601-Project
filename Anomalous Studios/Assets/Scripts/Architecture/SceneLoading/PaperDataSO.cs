
using UnityEngine;

[CreateAssetMenu(fileName = "PaperDataSO", menuName = "Scriptable Objects/PaperDataSO")]
public sealed class PaperDataSO : ScriptableObject
{
    public string Description = "";
    [Tooltip("If isTask is enabled, completes the task in the Handbook.")]
    public string TaskID = "";
    public bool IsTask = false;


}