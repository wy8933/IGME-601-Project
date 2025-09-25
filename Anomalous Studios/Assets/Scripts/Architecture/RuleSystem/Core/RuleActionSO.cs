using UnityEngine;

namespace RuleViolationSystem
{
    [CreateAssetMenu(fileName = "RuleActionSO", menuName = "Scriptable Objects/RuleActionSO")]
    public class RuleActionSO : ScriptableObject
    {
        public void Execute(IRuleQuery query, RuleAssetSO rule)
        {

        }
    }
}