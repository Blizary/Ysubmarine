using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FSMMooseRun : State
{
    //in this state the moose doesnt case very much for keeping close and instead
    //is more focused on runing away from the wolf
    //it is also in panic there for its movement is faster and less linear
    //it will leave this state if the wolf is no longer close and it has calmed down -> leaves to caution state
    public override void OnEnterState()
    {
        MooseAI ai = agent.GetComponent<MooseAI>();

        //change weights of sterring forces in order to better fit this state
        agent.GetComponent<Vehicle>().behaviours.Add(agent.GetComponent<Flee>());
        agent.GetComponent<Cohesion>().weight = ai.coehVal.z;
        agent.GetComponent<Alignment>().weight = ai.aligVal.z;
        agent.GetComponent<Separation>().weight = ai.separVal.z;
        agent.GetComponent<ConstantSpeed>().speed = ai.speed.z;
        agent.GetComponent<Wander>().weight = ai.wondVal.z;
        ai.StartFear();

    }

    public override void UpdateState()
    {
        //check if the wolf is no longer close and if it stoped panicing
        if(!agent.GetComponent<MooseAI>().wolfClose && agent.GetComponent<MooseAI>().inFearTimer<=0)
        {
            fsm.ActivateState<FSMMooseCauntion>();
        }
    }
}
