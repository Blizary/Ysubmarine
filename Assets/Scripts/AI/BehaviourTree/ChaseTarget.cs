using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviorDesigner.Runtime.Tasks;
using BehaviorDesigner.Runtime;
using PolyNav;


//Action used to chase a target 
//target tag can be inputed in the behaviour tree designer
[TaskCategory("DeepDark/Action")]
public class ChaseTarget : Action
{
    public string targetString;
    private GameObject target;
    public SharedVector3 targetPos;

    public override void OnAwake()
    {
        
    }

    public override TaskStatus OnUpdate()
    {
        target = GameObject.FindGameObjectWithTag(targetString);
        Vector3 targetPosGround = new Vector3(target.transform.position.x, target.transform.position.y, transform.position.z);
        targetPos.Value = targetPosGround;
        return TaskStatus.Success;
    }
}
