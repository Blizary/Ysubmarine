using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviorDesigner.Runtime.Tasks;
using BehaviorDesigner.Runtime;
using PolyNav;

[TaskCategory("DeepDark/Action")]
public class AvoidWall : Action
{
    public SharedVector3 originaltarget;
    public SharedVector3 closeWall;

    private EnemyManager currentManager;

    public override void OnAwake()
    {
        currentManager = GetComponent<EnemyManager>();
    }
    public override TaskStatus OnUpdate()
    {
        /*
        float randomAngle = Random.Range(0, 180);
        Quaternion rotation = Quaternion.Euler(randomAngle, randomAngle, 0);
        Vector3 myVector = closeWall.Value - transform.position;
        myVector = myVector.normalized;
        Vector3 rotateVector = rotation * myVector;
        */

        Vector3 rotateVector = closeWall.Value - transform.position;
        if(currentManager.GetComponent<PolyNavAgent>().map.PointIsValid(rotateVector))
        {
            originaltarget.Value = rotateVector;
            currentManager.destination = rotateVector;
            return TaskStatus.Success;
        }
        else
        {
            return TaskStatus.Failure;
        }
       
    }


    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(originaltarget.Value, 0.1f);
    }
}
