using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SGoap;


/// <summary>
/// This script was used to learn to use the SGOAP it was not created me but by the people who made the asset
/// is it used on the wolf that is disabled on the GOAP scene
/// </summary>
public class GoToAction : BasicAction
{
    public Transform Target;
    public float Range = 4;
    public float MoveSpeed = 2;

    public float DistanceToTarget => Vector3.Distance(Target.position, transform.position);
    private void Update()
    {
        // Add or Remove an Agent's state.
        if (DistanceToTarget <= Range)
            States.SetState("InShootingRange", 1);
        else
            States.RemoveState("InShootingRange");
    }

    public override EActionStatus Perform()
    {
        var distanceToTarget = Vector3.Distance(Target.position, transform.position);
        if (distanceToTarget <= Range)
            return EActionStatus.Success;
        var directionToTarget = (Target.position - transform.position).normalized;
        AgentData.Position += directionToTarget * Time.deltaTime * MoveSpeed;
        // Returning Running will keep this action going until we return Success.
        return EActionStatus.Running;
    }
}
