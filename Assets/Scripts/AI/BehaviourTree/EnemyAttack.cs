using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviorDesigner.Runtime.Tasks;
using BehaviorDesigner.Runtime;

[TaskCategory("DeepDark/Action")]
public class EnemyAttack : Action
{
    private EnemyManager currentManager;
    public override void OnAwake()
    {
        currentManager = GetComponent<EnemyManager>();
    }

    public override TaskStatus OnUpdate()
    {
        currentManager.Attack();
        return TaskStatus.Success;
    }
}
