using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WolfAI : MonoBehaviour
{

    public List<GameObject> foundDeer;//list of deer that are found by the wolf
    public GameObject spritehost;//reference to the obj that has the sprite rendered its used to flip it acording to the direction of te vehicle
    public GameObject home;//reference to where the wolf spawned used for going to sleep and rest
    public float memoryTimer;//the amount of time the wolf will rememeber the last deer he found before forgetting it
    private float memoryTimerInner;
    public GameObject lastKnownDeer;
    public float food;
    public float eatingTimer;
    public bool isEating;

    private Wander wanderInternal;
    private AvoidObstacles avoidInternal;
    private Seek seekInternal;

    // Start is called before the first frame update
    void Start()
    {
        wanderInternal = GetComponent<Wander>();
        avoidInternal = GetComponent<AvoidObstacles>();
        seekInternal = GetComponent<Seek>();


        GetComponent<Vehicle>().behaviours.Add(wanderInternal);
        GetComponent<Vehicle>().behaviours.Add(avoidInternal);
    }

    // Update is called once per frame
    void Update()
    {
        FaceDirection();
    }


    /// <summary>
    /// Flips the sprite of the moose based on the direction of the forces acting on it
    /// </summary>
    void FaceDirection()
    {
        if(Mathf.Abs(GetComponent<Vehicle>().direction.x)>0.05f)//used to prevent jitter
        {
            if (GetComponent<Vehicle>().direction.x > 0)
            {
                spritehost.GetComponent<SpriteRenderer>().flipX = true;
            }
            else
            {
                spritehost.GetComponent<SpriteRenderer>().flipX = false;
            }
        }
        
    }

    //returns the deer that is closest to the wolf
    public GameObject CloseDeer()
    {
        if(foundDeer.Count!=0)
        {
            GameObject deer = foundDeer[0];
            foreach(GameObject d in foundDeer)
            {
                if(Vector3.Distance(transform.position,d.transform.position)> Vector3.Distance(transform.position, deer.transform.position))
                {
                    deer = d;
                }
            }

            return deer;
        }

        return null;
    }


    void DeerMemory()
    {
        if(lastKnownDeer!=null && foundDeer.Count!=0)//memory is only triggered when there are no other deer around
        {
            if(memoryTimerInner!=0)
            {
                memoryTimerInner -= Time.deltaTime;
            }
            else//forgot about the deer
            {
                lastKnownDeer = null;
            }
        }
    }

    /// <summary>
    /// Changes the steering behaviours to add seek and remove wander
    /// </summary>
    public void AfterDeer(GameObject _target)
    {
        if (!GetComponent<Vehicle>().behaviours.Contains(seekInternal))
        {
            GetComponent<Vehicle>().behaviours.Add(seekInternal);
            seekInternal.target = _target.transform;
        }

        if (GetComponent<Vehicle>().behaviours.Contains(wanderInternal))
        {
            GetComponent<Vehicle>().behaviours.Remove(wanderInternal);
        }
    }
        
        

    /// <summary>
    /// Changes the steering behaviour to add wander and remove seek
    /// </summary>
    public void Wandering()
    {
        if (!GetComponent<Vehicle>().behaviours.Contains(wanderInternal))
        {
            GetComponent<Vehicle>().behaviours.Add(wanderInternal);
        }

        if (GetComponent<Vehicle>().behaviours.Contains(seekInternal))
        {
            GetComponent<Vehicle>().behaviours.Remove(seekInternal);
        }

    }

    /// <summary>
    /// Starts the timer for feeding and its triggered by the collision with the deer
    /// </summary>
    /// <param name="_amount"></param>
    public void Feeding(int _amount)
    {
        if (GetComponent<Vehicle>().behaviours.Contains(wanderInternal))
        {
            GetComponent<Vehicle>().behaviours.Remove(wanderInternal);
        }

        if (GetComponent<Vehicle>().behaviours.Contains(seekInternal))
        {
            GetComponent<Vehicle>().behaviours.Remove(seekInternal);
        }
        isEating = true;
        GetComponent<Rigidbody2D>().velocity = Vector3.zero;
        StartCoroutine(FeedingIE(_amount));

    }

    IEnumerator FeedingIE(int _amount)
    {
        yield return new WaitForSeconds(eatingTimer);
        food += _amount;
        if (food > 100)//cap at 100
        {
            food = 100;
        }
        isEating = false;

    }

    void OnTriggerEnter2D(Collider2D c)
    {
        //if a deer comes close add them to te list of deer the wolf is aware of
        if (c.gameObject.CompareTag("Moose"))
        {
            foundDeer.Add(c.gameObject);
        }
    }

    void OnTriggerExit2D(Collider2D c)
    {
        //if a deer wanders away then remove this deer from the list
        if (c.gameObject.CompareTag("Moose"))
        {
            if(foundDeer.Count==1)
            {
                lastKnownDeer = foundDeer[0];//if this was the last deer the wolf know keep it in memory for alittle while
                memoryTimerInner = memoryTimer;
            }
            foundDeer.Remove(c.gameObject);
        }
    }


    
}
