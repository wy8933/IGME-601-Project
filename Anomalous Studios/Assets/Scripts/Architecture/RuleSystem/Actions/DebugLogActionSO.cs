using System;
using UnityEngine;

namespace RuleViolationSystem
{
    [CreateAssetMenu(fileName = "DebugLogActionSO", menuName = "Rules/Actions/DebugLogActionSO")]
    public class DebugLogActionSO : RuleActionSO
    {
        public enum Level { Log, Warning, Error }

        [Header("Message")]
        [TextArea(2, 4)]
        public string message;

        public Level level = Level.Log;

        public override void Execute(IRuleQuery query, RuleAssetSO rule)
        {
            if (rule == null) return;

            var floorId = query != null ? query.FloorId : string.Empty;
            var utc = query != null ? query.UtcNow : DateTime.UtcNow;
            var nowLocal = DateTime.Now;

            // Token replacement
            string msg = (message ?? string.Empty)
                .Replace("{ruleId}", rule.ruleId ?? string.Empty)
                .Replace("{floorId}", floorId ?? string.Empty)
                .Replace("{utc}", utc.ToString("O"))
                .Replace("{now}", nowLocal.ToString("T"));

            switch (level)
            {
                case Level.Warning:
                    Debug.LogWarning(msg);
                    break;
                case Level.Error:
                    Debug.LogError(msg);
                    break;
                default:
                    Debug.Log(msg);
                    break;
            }
        }
    }
}