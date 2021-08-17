using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviorDesigner.Runtime.Tasks;
using BehaviorDesigner.Runtime;
using PolyNav;

[TaskCategory("DeepDark/Action")]
public class ChaseTarget : Action
{
    public string targetString;
    private GameObject target;
    public SharedVector3 targetPos;

    public override void OnAwake()
    {
        target = GameObject.FindGameObjectWithTag(targetString);
    }

    public override TaskStatus OnUpdate()
    {
        targetPos.Value = target.transform.position;
        return TaskStatus.Success;
    }
}
