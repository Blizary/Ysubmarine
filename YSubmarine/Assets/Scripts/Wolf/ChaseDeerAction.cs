using SGoap;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChaseDeerAction : BasicAction
{
    public Transform target;
    public float Range = 4;
    public float MoveSpeed = 2;
    public WolfAI wolfHost;
    
    public float DistanceToTarget => Vector3.Distance(target.position, transform.position);
    private void Update()
    {
        if (wolfHost.CloseDeer())
        {
            target = wolfHost.CloseDeer().transform;
        }

    }

    public override EActionStatus Perform()
    {

        if (!wolfHost.CloseDeer())
        {
            States.RemoveState("FoundDeer");
            return EActionStatus.Failed;
        }

        wolfHost.AfterDeer(target.gameObject);
        // Add or Remove an Agent's state.
        if (wolfHost.isEating)
            States.SetState("CloseToDeer", 1);
        else
            States.RemoveState("CloseToDeer");

        if (wolfHost.isEating)
            return EActionStatus.Success;

        // Returning Running will keep this action going until we return Success.
        return EActionStatus.Running;
    }
    
}
