using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviorDesigner.Runtime.Tasks;
using BehaviorDesigner.Runtime;


//conditional task to check if enemy has spoted something
[TaskCategory("DeepDark/Conditional")]
public class CheckSight : Conditional
{
    private EnemyManager currentManager;

    public override void OnAwake()
    {
        currentManager = GetComponent<EnemyManager>();
    }

    public override TaskStatus OnUpdate()
    {
        if(currentManager.CheckVision().Count!=0 || currentManager.CheckProximity().Count != 0)
        {
            return TaskStatus.Success;
        }

        return TaskStatus.Failure;
    }
}
