using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FSMMooseCauntion : State
{
    //the cauntion state is started when a wolf is spoted near by 
    // in this state the deer will move closer together and wander less 
    // to simulate the animal being aware of the wolfs presence
   // it will leave this state if the wolf is no longer spoted -> goes back to wander
   // or if the wolf comes close-> goes to panic
    public override void OnEnterState()
    {
        MooseAI ai = agent.GetComponent<MooseAI>();

        agent.GetComponent<Cohesion>().weight = ai.coehVal.y;
        agent.GetComponent<Alignment>().weight = ai.aligVal.y;
        agent.GetComponent<Separation>().weight = ai.separVal.y;
        agent.GetComponent<ConstantSpeed>().speed = ai.speed.y;
        agent.GetComponent<Wander>().weight = ai.wondVal.y;

    }

    public override void UpdateState()
    {
        if (agent.GetComponent<MooseAI>().wolfClose)
        {
            fsm.ActivateState<FSMMooseRun>();
        }
        else if (!agent.GetComponent<MooseAI>().wolfClose)
        {
            List<Vehicle> neighbours = agent.GetComponent<Separation>().neighbours;
            bool isWolfClose = false;
            for (int i = 0; i < neighbours.Count; i++)
            {
                if (!isWolfClose)
                {
                    if (neighbours[i].GetComponent<MooseAI>().wolfSpoted)
                    {
                        isWolfClose = true;
                    }
                }
                
            }

            if(!isWolfClose)
            {
                agent.GetComponent<MooseAI>().wolfSpoted = false;
                fsm.ActivateState<FSMMooseWander>();
            }
        }

    }

}
