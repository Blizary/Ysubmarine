using SGoap;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExploreAction : BasicAction
{
    public WolfAI wolfHost;
    public override float CooldownTime => 1;
    public override bool AlwaysIncludeInPlan => true;
    public override EActionStatus Perform()
    {
        wolfHost.Wandering();
        if (wolfHost.foundDeer.Count!=0)
        {
            States.SetState("FoundDeer", 1);
            return EActionStatus.Success;
        }

        return EActionStatus.Running;
    }

}
