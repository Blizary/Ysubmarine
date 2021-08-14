using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviorDesigner.Runtime.Tasks;
using BehaviorDesigner.Runtime;

[TaskCategory("DeepDark/Conditional")]
public class WallProximity : Conditional
{
    private EnemyManager currentManager;
    public SharedVector3 closeWall;
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
            Debug.Log("testing objs i see ");
            if (obj.CompareTag("Rocks"))
            {
                Debug.Log("i see walls");
                if(currentManager.CloseToWall()!=Vector3.zero)
                {
                    Debug.Log("im next to a wall");
                    closeWall.Value = currentManager.CloseToWall();
                    return TaskStatus.Success;
                }
            }
        }

        return TaskStatus.Failure;
    }
}
