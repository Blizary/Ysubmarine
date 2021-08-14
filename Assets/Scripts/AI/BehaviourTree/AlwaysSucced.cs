using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviorDesigner.Runtime.Tasks;
using BehaviorDesigner.Runtime;
using PolyNav;

[TaskCategory("DeepDark/Action")]
public class AlwaysSucced : Action
{
    public override TaskStatus OnUpdate()
    {
        return TaskStatus.Success;
    }
}
