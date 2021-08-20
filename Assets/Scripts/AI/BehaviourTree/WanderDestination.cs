using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviorDesigner.Runtime.Tasks;
using BehaviorDesigner.Runtime;
using PolyNav;

// Action  used to pick a random place valid within the navmesh
// distance of the point can be defined in the behaviour designer 
[TaskCategory("DeepDark/Action")]
public class WanderDestination : Action
{
    public SharedVector3 destination;
    public float targetDistance;

    private EnemyManager currentManager;

    public override void OnAwake()
    {
        currentManager = GetComponent<EnemyManager>();
    }



    public override TaskStatus OnUpdate()
    {
        if(currentManager.hasTarget)//target has already been picked and should be moving towards it
        {
            destination.Value = currentManager.destination;
            return TaskStatus.Success;
        }
        else //pick a target within range
        {
            Vector2 pos = new Vector2(transform.position.x, transform.position.y);
            Vector3 randompos = (Random.insideUnitCircle.normalized * targetDistance + pos);
            if(currentManager.GetComponent<PolyNavAgent>().map.PointIsValid(randompos))
            {
                destination.Value = randompos;
                currentManager.destination = destination.Value;
                currentManager.hasTarget = true;
            }
            else
            {
                return TaskStatus.Failure;
            }
            
            
        }

        return TaskStatus.Success;
    }


    
}
