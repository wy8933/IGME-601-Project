using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "Assign Random Location", story: "Randomize new [target]", category: "Action", id: "909ffff334e8c81ad715cc4ebd066421")]
public partial class AssignRandomLocationAction : Action
{
    [SerializeReference] public BlackboardVariable<Vector3> Target;
    [SerializeReference] public BlackboardVariable<float> Reach;

    protected override Status OnUpdate()
    {
        int rotation = new System.Random().Next(0, 360);
        Vector3 direction = Quaternion.AngleAxis(rotation, Vector3.up) * Vector3.forward;


        if (Physics.Raycast(Target, direction, out RaycastHit hit, Reach.Value, ~LayerMask.GetMask("IgnoreRaycast")))
        {
            Target.Value = hit.point;
        }
        else
        {
            Target.Value = direction * Reach.Value;
        }

        return Status.Success;
    }
}

