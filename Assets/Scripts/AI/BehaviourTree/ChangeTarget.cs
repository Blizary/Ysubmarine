using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviorDesigner.Runtime.Tasks;
using BehaviorDesigner.Runtime;

[TaskCategory("DeepDark/Action")]
public class ChaseTarget : Action
{
    public SharedVector3 originaltarget;
    public SharedVector3 newTarget;

    private EnemyManager currentManager;

    public override void OnAwake()
    {
        currentManager = GetComponent<EnemyManager>();
    }

    public override TaskStatus OnUpdate()
    {

        currentManager.destination = newTarget.Value;
        originaltarget.Value = newTarget.Value;

        return TaskStatus.Success;
    }
}
