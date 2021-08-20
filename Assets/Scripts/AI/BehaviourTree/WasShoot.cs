using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviorDesigner.Runtime.Tasks;
using BehaviorDesigner.Runtime;
using PolyNav;

// task used to trigger an effect in case this enemy was shoot
// it checks if its life has changed
[TaskCategory("DeepDark/Conditional")]
public class WasShoot : Conditional
{
    private EnemyManager currentManager;

    public override void OnAwake()
    {
        currentManager = GetComponent<EnemyManager>();
    }

    public override TaskStatus OnUpdate()
    {
        if (currentManager.StateOfLife()!=1)
        {
            return TaskStatus.Success;
        }

        return TaskStatus.Failure;
    }
}