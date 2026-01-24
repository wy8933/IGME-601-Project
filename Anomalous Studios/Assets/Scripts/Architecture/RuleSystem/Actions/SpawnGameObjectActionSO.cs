using UnityEngine;

namespace RuleViolationSystem
{
    [CreateAssetMenu(fileName = "SpawnGameObjectActionSO", menuName = "Rules/Actions/Spawn GameObject")]
    public sealed class SpawnGameObjectActionSO : RuleActionSO
    {
        [SerializeField] private GameObject _prefabToSpawn;
        [SerializeField] private Vector3 _worldOffset;

        public override void Execute(IRuleQuery query, RuleAssetSO rule)
        {
            if (query == null || _prefabToSpawn == null) return;

            var pos = query.WorldPosition + _worldOffset;
            Object.Instantiate(_prefabToSpawn, pos, Quaternion.identity);
        }
    }
}
