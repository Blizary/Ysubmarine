using PolyNav;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    [SerializeField] private EnemyVision coneOfVision;
    [SerializeField] private EnemyProximity proximityDetection;
    [SerializeField] private Animator animationController;

    public Vector3 destination;
    public bool hasTarget;

    private PolyNavAgent polyAgent;
    private float originalSpeed;
    


    // Start is called before the first frame update
    void Start()
    {
        polyAgent = GetComponent<PolyNavAgent>();
        originalSpeed = polyAgent.maxSpeed;
    }

    // Update is called once per frame
    void Update()
    {
        CheckMovement();
    }


    void CheckMovement()
    {
        if(polyAgent.hasPath)
        {
            animationController.SetBool("moving", true);
        }
        else
        {
            animationController.SetBool("moving", false);
        }
    }


    public List<GameObject> CheckVision()
    {
        List<GameObject> visibleObjs = new List<GameObject>();
        coneOfVision.ValidateObjs();
        foreach(GameObject obj in coneOfVision.visibleObjs)
        {
            visibleObjs.Add(obj);
        }

        
        return visibleObjs;

    }

    public Vector3 CloseToWall()
    {
        return proximityDetection.closeWall;
    }

    public void ChangeSpeed(float _difference)
    {
        polyAgent.maxSpeed = originalSpeed + _difference;
        if(polyAgent.maxSpeed< originalSpeed)
        {
            polyAgent.maxSpeed = originalSpeed;
        }
    }





    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(destination, 0.1f);
    }




}
