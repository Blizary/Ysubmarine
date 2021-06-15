using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MooseAI : MonoBehaviour
{
    [Header("Moose Stats")]
    //all these are vectors 3 becouse there are only 3 states in the FSM
    //0- wander
    //1- cauntion
    //2 - flee
    //in the future if more states are required this values will have to be stored in a list instead
    public Vector3 speed;
    public Vector3 coehVal;
    public Vector3 separVal;
    public Vector3 aligVal;
    public Vector3 wondVal;
    public float fearRange;
    public float safetlyRange;
    public GameObject spritehost;

    //references to the steering behaviours available
    private FSM fsm;
    private Cohesion cohesionInternal;
    private Separation separationInternal;
    private Alignment alignmentInternal;
    private ConstantSpeed constantSpeedInternal;
    private Flee fleeInternal;
    private AvoidObstacles avoidInternal;
    private Wander wanderInternal;


    public bool wolfSpoted;
    public bool wolfClose;
    [HideInInspector]
    public GameObject target;
    public float inFearTimer;
    private float inFearTimerInternal;


    // Start is called before the first frame update
    void Start()
    {
        safetlyRange = GetComponent<CircleCollider2D>().radius+1;
        inFearTimerInternal = inFearTimer;
        inFearTimer = 0;
    }

    // Update is called once per frame
    void Update()
    {
        WolfProximity();
        FaceDirection();
        InFear();
    }

    /// <summary>
    /// Flips the sprite of the moose based on the direction of the forces acting on it
    /// </summary>
    void FaceDirection()
    {
        if(GetComponent<Vehicle>().direction.x>0)
        {
            spritehost.GetComponent<SpriteRenderer>().flipX=false;
        }
        else
        {
            spritehost.GetComponent<SpriteRenderer>().flipX = true;
        }
    }


    /// <summary>
    /// function called when the obj is created by the MooseManager in order to set all variables
    /// </summary>
    /// <param name="_cohW"></param>
    /// <param name="_sepW"></param>
    /// <param name="_aliW"></param>
    /// <param name="_sibW"></param>
    public void StartBehaviours(GameObject _predator)
    {
        //add all the components to the behaviours list to they can be properly calculated
        cohesionInternal = GetComponent<Cohesion>();
        cohesionInternal.weight = coehVal.x;
        separationInternal = GetComponent<Separation>();
        separationInternal.weight = separVal.x;
        alignmentInternal = GetComponent<Alignment>();
        alignmentInternal.weight = aligVal.x;
        constantSpeedInternal = GetComponent<ConstantSpeed>();
        constantSpeedInternal.speed = speed.x;
        fleeInternal = GetComponent<Flee>();
       // fleeInternal.evader = _predator.GetComponent<Vehicle>();
        avoidInternal = GetComponent<AvoidObstacles>();
        wanderInternal = GetComponent<Wander>();




        GetComponent<Vehicle>().behaviours.Add(cohesionInternal);
        GetComponent<Vehicle>().behaviours.Add(separationInternal);
        GetComponent<Vehicle>().behaviours.Add(alignmentInternal);
        GetComponent<Vehicle>().behaviours.Add(constantSpeedInternal);
        GetComponent<Vehicle>().behaviours.Add(fleeInternal);
        GetComponent<Vehicle>().behaviours.Add(avoidInternal);
        GetComponent<Vehicle>().behaviours.Add(wanderInternal);

        fsm = GetComponent<FSM>();
        fsm.LoadState<FSMMooseWander>();
        fsm.LoadState<FSMMooseCauntion>();
        fsm.LoadState<FSMMooseRun>();
        fsm.ActivateState<FSMMooseWander>();
    }



    void WolfProximity()
    {
        if (wolfSpoted )
        {
            if(Vector3.Distance(transform.position, target.transform.position) <= fearRange)
            {
                wolfClose = true;
                fleeInternal.evader = target.GetComponent<Vehicle>();
            }
            else
            {
                wolfClose = false;
            }

            if(Vector3.Distance(transform.position, target.transform.position) >= safetlyRange)
            {
                wolfSpoted = false;
            }
            
        }
    }

    /// <summary>
    /// Timer for the fear
    /// </summary>
    void InFear()
    {
        if(inFearTimer>=0)
        {
            inFearTimer -= Time.deltaTime;
        }
    }
    
    /// <summary>
    /// Starts the fear timer
    /// </summary>
    public void StartFear()
    {
        inFearTimer = inFearTimerInternal;
    }


    public void WolfHasBeenSpoted(GameObject _target)
    {
        wolfSpoted = true;
        target = _target;
    }


    void OnTriggerEnter2D(Collider2D c)
    {
        if (c.gameObject.CompareTag("Wolf"))
        {
            wolfSpoted = true;
            target = c.gameObject;
        }
    }

    void OnTriggerExit2D(Collider2D c)
    {
        if (c.gameObject.CompareTag("Wolf"))
        {
            wolfSpoted = false;
            wolfClose = false;
        }
    }

    void OnCollisionEnter2D(Collision2D col)
    {
        if (col.gameObject.CompareTag("Wolf"))
        {
            //wolf gets feed
            col.gameObject.GetComponent<WolfAI>().Feeding(10);
            //deer is dead
            Destroy(this.gameObject);
        }
    }

}
