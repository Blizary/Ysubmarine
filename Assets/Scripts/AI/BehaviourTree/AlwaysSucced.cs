using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviorDesigner.Runtime.Tasks;
using BehaviorDesigner.Runtime;
using PolyNav;

//Simple action that always returns success
//Used in the end of selectors inside sequences in order to continue
[TaskCategory("DeepDark/Action")]
public class AlwaysSucced : Action
{
    public override TaskStatus OnUpdate()
    {
        return TaskStatus.Success;
    }
}
