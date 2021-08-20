using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviorDesigner.Runtime.Tasks;
using BehaviorDesigner.Runtime;
using PolyNav;


// conditional to check if the wait timer has passed and the enemy can attack again
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
