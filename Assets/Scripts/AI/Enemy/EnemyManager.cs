using BehaviorDesigner.Runtime;
using PolyNav;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private EnemyVision coneOfVision;
    [SerializeField] private EnemyProximity proximityDetection;
    [SerializeField] private Animator animationController;

    [Header("Stats")]
    [SerializeField] private float maxLife;
    [SerializeField] private float currentlife;


    public Vector3 destination;
    public bool hasTarget;

    private PolyNavAgent polyAgent;
    private float originalSpeed;
    private bool deathTriggered;
    private EvolutionManager evolutionBrain;
    public DNA currentDNA;


    // Start is called before the first frame update
    void Start()
    {
        currentlife = maxLife;
        deathTriggered = false;
        polyAgent = GetComponent<PolyNavAgent>();
        evolutionBrain = GameObject.FindGameObjectWithTag("Queen").GetComponent<EvolutionManager>();
        originalSpeed = polyAgent.maxSpeed;
    }

    // Update is called once per frame
    void Update()
    {
        CheckMovement();
        if(!deathTriggered)
        {
            CheckLife();
        }
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

    void CheckLife()
    {
        if(currentlife<=0)
        {
            //enemy died
            polyAgent.Stop();
            GetComponent<BehaviorTree>().enabled = false;
            //inform the performance of this enemy to the Evolution manager
            currentDNA.CalculateFitness(2, 2, 2);//!!!!!!!!!!!RANDOM NUMBERS FOR NOW CHANGE LATER!!!!!!!!!!!!!!
            evolutionBrain.AddDeathDNA(currentDNA, this.gameObject);

            StartCoroutine(DeathAnimation());
            
        }
    }

    IEnumerator DeathAnimation()
    {
        deathTriggered = true;
        animationController.SetTrigger("death");
        yield return new WaitForSeconds(3);
        Destroy(this.gameObject);
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

    public void ChangeLife(float _difference)
    {
        float nextLife = currentlife + _difference;
        if(nextLife>=maxLife)
        {
            currentlife = maxLife;
        }
        else if(nextLife<=0)
        {
            currentlife = 0;
        }
        else
        {
            currentlife = nextLife;
        }

    }


    public void Attack()
    {
        animationController.SetTrigger("bite");
    }

    public void ChooseBehaviourStrategy()
    {
        //find 2 top strategies acording to dna
        Vector2Int topstrats = Vector2Int.zero;
        Vector2Int topstratValues = Vector2Int.zero;
       for(int i=0;i<currentDNA.dnaCode.Count;i++)
        {
            if(currentDNA.dnaCode[i]>topstrats.x)
            {
                topstrats.x = currentDNA.dnaCode[i];
                topstratValues.x = i;
            }
            else if(currentDNA.dnaCode[i] > topstrats.y)
            {
                topstrats.y = currentDNA.dnaCode[i];
                topstratValues.y = i;
            }
        }

        //coin flip
        int coin = Random.Range(0, 2);
        if(coin==0)
        {
            //pick 1st strategy
            GetComponent<BehaviorTree>().ExternalBehavior = currentDNA.portfolio[topstratValues.x];
        }
        else
        {
            //pick 2nd strategy
            GetComponent<BehaviorTree>().ExternalBehavior = currentDNA.portfolio[topstratValues.y];
        }

       
    }


    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(destination, 0.1f);
    }




}
