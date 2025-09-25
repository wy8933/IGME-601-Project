
using UnityEngine;
namespace RuleViolationSystem 
{ 
    [CreateAssetMenu(fileName = "RuleSetSO", menuName = "Scriptable Objects/RuleSetSO")]
    public class RuleSetSO : ScriptableObject
    {
        public string floorId;
        public RuleAssetSO rules;
    }
}