using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviorDesigner.Runtime.Tasks;
using BehaviorDesigner.Runtime;
using PolyNav;

[TaskCategory("DeepDark/Action")]
public class MoveTowards : Action
{
    public float proximityRange;
    public SharedVector3 target;

    private PolyNavAgent polyAgent;
    private EnemyManager currentManager;

    public override void OnAwake()
    {
        polyAgent = GetComponent<PolyNavAgent>();
        currentManager = GetComponent<EnemyManager>();
    }

    public override TaskStatus OnUpdate()
    {
        if(Vector3.Distance(transform.position, target.Value)< proximityRange)
        {
            return TaskStatus.Success;
        }
        else
        {
            polyAgent.SetDestination(target.Value);

        }


        return TaskStatus.Failure;
    }
}
