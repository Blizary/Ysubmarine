using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SGoap;

/// <summary>
/// This script was used to learn to use the SGOAP it was not created me but by the people who made the asset
/// is it used on the wolf that is disabled on the GOAP scene
/// </summary>
// Every SGoap Action inherits from either BasicAction or Action
public class ShootAction : BasicAction
{
    /*
    // This action has a cool down of 1 second every use.
    public override float CooldownTime => 1;
    // The planner by default skips actions that are cooling down unless we set this to true
    public override bool AlwaysIncludeInPlan => true;
    // Override Perform to execute the action.
    public override EActionStatus Perform()
    {
        Debug.Log("Shot");
        return EActionStatus.Success;
    }
    */

    public override float CooldownTime => 1;
    public override bool AlwaysIncludeInPlan => true;
    public override EActionStatus Perform()
    {
        var bullet = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        bullet.transform.localScale = Vector3.one * 0.2f;
        bullet.transform.position = AgentData.Position;
        bullet.transform.GoTo(AgentData.Target.position, 1);
        Destroy(bullet.gameObject, 1.2f);
        return EActionStatus.Success;
    }

}

