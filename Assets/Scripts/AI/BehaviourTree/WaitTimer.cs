using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviorDesigner.Runtime.Tasks;
using BehaviorDesigner.Runtime;
using PolyNav;

//simple action that acts as a clock
// it has a setting to allow for random wait timers or set wait timers
// random wait timers are used for the base behaviour of wandering
// set wait timers are used in the attack behaviour

[TaskCategory("DeepDark/Action")]
public class WaitTimer : Action
{
    public bool randomWait;
    public float waitTimerMax;
    public SharedFloat waitTimerTick;
    private PolyNavAgent polyAgent;
    private EnemyManager currentManager;

    public override void OnAwake()
    {
        polyAgent = GetComponent<PolyNavAgent>();
        currentManager = GetComponent<EnemyManager>();
    }

    public override TaskStatus OnUpdate()
    {
        if(waitTimerTick.Value<=0)
        {
            currentManager.hasTarget = false;
            currentManager.destination = Vector3.zero;
            currentManager.canAttack = true;
            if (randomWait)
            {
                int coinFlip = Random.Range(0, 2);
                if (coinFlip != 0)
                {
                    waitTimerTick.Value = Random.Range(1, waitTimerMax);
                    return TaskStatus.Success;
                }
            }
            else
            {
                waitTimerTick.Value = waitTimerMax;
            }
            
        }
        else
        {
            polyAgent.activePath.Clear();
            waitTimerTick.Value -= Time.deltaTime;
            return TaskStatus.Failure;
        }
        return TaskStatus.Failure;
    }

}
