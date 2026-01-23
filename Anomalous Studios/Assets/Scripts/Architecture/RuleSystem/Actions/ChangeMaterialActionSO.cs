using UnityEngine;

namespace RuleViolationSystem
{
    [CreateAssetMenu(fileName = "ChangeMaterialActionSO", menuName = "Rules/Actions/Change Material")]
    public sealed class ChangeMaterialActionSO : RuleActionSO
    {
        [SerializeField] private Material _materialToApply;
        [SerializeField] private bool _applyToChildren = true;
        [SerializeField] private bool _useSharedMaterial = true;

        public override void Execute(IRuleQuery query, RuleAssetSO rule)
        {
            if (query == null || _materialToApply == null) return;

            var target = query.Target != null ? query.Target : query.Instigator;
            if (target == null) return;

            if (_applyToChildren)
            {
                var renderers = target.GetComponentsInChildren<Renderer>(includeInactive: true);
                for (int i = 0; i < renderers.Length; i++)
                {
                    var r = renderers[i];
                    if (r == null) continue;

                    if (_useSharedMaterial) r.sharedMaterial = _materialToApply;
                    else r.material = _materialToApply;
                }
            }
            else
            {
                var r = target.GetComponent<Renderer>();
                if (r == null) return;

                if (_useSharedMaterial) r.sharedMaterial = _materialToApply;
                else r.material = _materialToApply;
            }
        }
    }
}
