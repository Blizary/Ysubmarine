using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviorDesigner.Runtime.Tasks;
using BehaviorDesigner.Runtime;
using PolyNav;

[TaskCategory("DeepDark/Conditional")]
public class CanAttack : Conditional
{
    private EnemyManager currentManager;

    public override void OnAwake()
    {
        currentManager = GetComponent<EnemyManager>();
    }

    public override TaskStatus OnUpdate()
    {
        if (currentManager.canAttack)
        {
            return TaskStatus.Success;
        }

        return TaskStatus.Failure;
    }
}
