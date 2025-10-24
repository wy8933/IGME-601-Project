using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;
using UnityEngine.SceneManagement;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "Attack", story: "[Agent] attacks [target]", category: "Action", id: "c0463c3b74bae776c5c4960943826640")]
public partial class AttackAction : Action
{
    [SerializeReference] public BlackboardVariable<GameObject> Agent;
    [SerializeReference] public BlackboardVariable<GameObject> Target;

    /// <summary>
    /// An extremely temporary measure. In the future, actions will be reliant on damage values
    /// </summary>
    /// <returns></returns>
    protected override Status OnStart()
    {
        return Status.Running;
    }

    protected override Status OnUpdate()
    {
        //EventBus<LoadLevel>.Raise(new LoadLevel { newLevel = (Level)SceneLoader.CurrentLevel });
        return Status.Success;
    }
}

