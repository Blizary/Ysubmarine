using SGoap;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EatDeerAction : BasicAction
{
    public WolfAI wolfHost;
    // This action has a cool down of 1 second every use.
    public override float CooldownTime => 1;
     // The planner by default skips actions that are cooling down unless we set this to true
     public override bool AlwaysIncludeInPlan => true;
     // Override Perform to execute the action.
     public override EActionStatus Perform()
     {

        if(wolfHost.isEating)
        {
            return EActionStatus.Running;
        }

        States.RemoveState("CloseToDeer");
        return EActionStatus.Success;
     }
}
