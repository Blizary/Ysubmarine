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

    public override void OnAwake()
    {
        currentManager = GetComponent<EnemyManager>();
        
    }

    public override TaskStatus OnUpdate()
    {
        visibleObjs = currentManager.CheckVision();
        foreach (GameObject obj in visibleObjs)
        {
            if(obj.CompareTag("Player"))
            {
                Vector3 playerPosGrounded = obj.transform.position;
                playerPosGrounded.z = transform.position.z;
                playerPosition.Value = playerPosGrounded;
                return TaskStatus.Success;
            }
        }

        return TaskStatus.Failure;
    }
}
