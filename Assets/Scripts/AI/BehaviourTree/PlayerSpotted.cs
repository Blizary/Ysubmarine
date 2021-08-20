using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviorDesigner.Runtime.Tasks;
using BehaviorDesigner.Runtime;

[TaskCategory("DeepDark/Conditional")]
public class PlayerSpotted : Conditional
{
    private EnemyManager currentManager;
    public SharedVector3 playerPosition;
    private List<GameObject> visibleObjs;
    private List<GameObject> proximityObjs;

    public override void OnAwake()
    {
        currentManager = GetComponent<EnemyManager>();
        
    }

    public override TaskStatus OnUpdate()
    {
        visibleObjs = currentManager.CheckVision();
        proximityObjs = currentManager.CheckProximity();

        foreach (GameObject obj in visibleObjs)
        {
           if(CheckPlayer(obj))
            {
                return TaskStatus.Success;
            }
        }

        foreach (GameObject obj in proximityObjs)
        {
            if (CheckPlayer(obj))
            {
                return TaskStatus.Success;
            }
        }

        return TaskStatus.Failure;
    }


    private bool CheckPlayer(GameObject _obj)
    {
        if (_obj.CompareTag("Player"))
        {
            Vector3 playerPosGrounded = _obj.transform.position;
            playerPosGrounded.z = transform.position.z;
            playerPosition.Value = playerPosGrounded;
            return true;
        }
        return false;
    }
}
