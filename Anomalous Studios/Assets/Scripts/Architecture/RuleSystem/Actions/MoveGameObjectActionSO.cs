using UnityEngine;

namespace RuleViolationSystem
{
    public enum MoveRuleActionMode { AddWorldOffset, TeleportToQueryWorldPosition }

    [CreateAssetMenu(fileName = "MoveGameObjectActionSO", menuName = "Rules/Actions/Move GameObject")]
    public sealed class MoveGameObjectActionSO : RuleActionSO
    {
        [SerializeField] private MoveRuleActionMode _mode = MoveRuleActionMode.AddWorldOffset;
        [SerializeField] private Vector3 _worldOffset;
        [SerializeField] private bool _preferRigidbodyMove = true;

        public override void Execute(IRuleQuery query, RuleAssetSO rule)
        {
            if (query == null) return;

            var target = query.Target != null ? query.Target : query.Instigator;
            if (target == null) return;

            Vector3 newPos = target.transform.position;

            switch (_mode)
            {
                case MoveRuleActionMode.AddWorldOffset:
                    newPos = target.transform.position + _worldOffset;
                    break;
                case MoveRuleActionMode.TeleportToQueryWorldPosition:
                    newPos = query.WorldPosition;
                    break;
            }

            if (_preferRigidbodyMove)
            {
                var rb3D = target.GetComponent<Rigidbody>();
                if (rb3D != null) { rb3D.MovePosition(newPos); return; }

                var rb2D = target.GetComponent<Rigidbody2D>();
                if (rb2D != null) { rb2D.MovePosition(newPos); return; }
            }

            target.transform.position = newPos;
        }
    }
}
