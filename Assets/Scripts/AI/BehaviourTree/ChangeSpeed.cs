using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviorDesigner.Runtime.Tasks;
using BehaviorDesigner.Runtime;
using PolyNav;


//Action used to adjust the speed of the enemy
//the variable can be changed in the beheavior tree designer
[TaskCategory("DeepDark/Action")]
public class ChangeSpeed : Action
{
    public float speedChange;
    private EnemyManager currentManager;

    public override void OnAwake()
    {
        currentManager = GetComponent<EnemyManager>();
    }

    public override TaskStatus OnUpdate()
    {
        currentManager.ChangeSpeed(speedChange);
        return TaskStatus.Success;
    }




}
