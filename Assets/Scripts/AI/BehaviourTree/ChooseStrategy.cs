using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviorDesigner.Runtime.Tasks;
using BehaviorDesigner.Runtime;
using PolyNav;


// Action used to ask the enemy manager to pick the next behaviour tree depending on the dna code
[TaskCategory("DeepDark/Action")]
public class ChooseStrategy : Action
{
    private EnemyManager currentManager;

    public override void OnAwake()
    {
        currentManager = GetComponent<EnemyManager>();
    }

    public override TaskStatus OnUpdate()
    {
        currentManager.ChooseBehaviourStrategy();
        return TaskStatus.Failure;
    }

}
