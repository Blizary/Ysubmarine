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
    [SerializeField] private GameObject frontLight;

    [Header("Stats")]
    [SerializeField] private float maxLife;
    [SerializeField] private float currentlife;
    [SerializeField] public float boostSpeed;
    [SerializeField] public float attackPower;
    [SerializeField] public float stamina;


    public Vector3 destination;
    public bool hasTarget;

    private PolyNavAgent polyAgent;
    private float originalSpeed;
    private bool deathTriggered;
    private EvolutionManager evolutionBrain;
    public DNA currentDNA;

    private bool engaged;
    public float timeEngaged;
    private Vector3 originalEngagedPos;
    public float distanceTraveled;
    public float damagedealt;
    private float playerlifeAtBattleStart;
    private GameObject player;


    private float averageSize;

    // Start is called before the first frame update
    void Start()
    {
        currentlife = maxLife;
        deathTriggered = false;
        polyAgent = GetComponent<PolyNavAgent>();
        evolutionBrain = GameObject.FindGameObjectWithTag("Queen").GetComponent<EvolutionManager>();
        player = GameObject.FindGameObjectWithTag("Player");
        originalSpeed = polyAgent.maxSpeed;
        DNAInterpretacion();
    }

    // Update is called once per frame
    void Update()
    {
        CheckMovement();
        if(!deathTriggered)
        {
            CheckLife();
            CalculateEngagedVariable();
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
            engaged = false;
            damagedealt = playerlifeAtBattleStart - player.GetComponent<PlayerManager>().currentHealth;
            currentDNA.CalculateFitness(damagedealt, timeEngaged, distanceTraveled);//!!!!!!!!!!!RANDOM NUMBERS FOR NOW CHANGE LATER!!!!!!!!!!!!!!
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

    public void DNAInterpretacion()
    {
        //set bases
        averageSize = transform.localScale.x;

        int skipBaseDna = currentDNA.portfolio.Count - 1;
        //1st number == speed
        originalSpeed = currentDNA.dnaCode[skipBaseDna + 1];
        polyAgent.maxSpeed = currentDNA.dnaCode[skipBaseDna + 1];
        //2nd == speed boost when used when chasing or fleeing
        boostSpeed = currentDNA.dnaCode[skipBaseDna + 2];
        //3rd == Health
        maxLife = currentDNA.dnaCode[skipBaseDna + 3];
        float newSize = (maxLife * averageSize) / evolutionBrain.Gethealth();
        gameObject.transform.GetChild(0).transform.localScale = new Vector3(newSize, newSize, 0);
        //4th == Stamina
        stamina = currentDNA.dnaCode[skipBaseDna + 4];
        
        //5th == Light
        frontLight.GetComponent<Light2D>().size = currentDNA.dnaCode[skipBaseDna + 5];
        //6th == Attack Power
        attackPower = currentDNA.dnaCode[skipBaseDna + 6];
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
        List<float> portfolioPartOfDNA = new List<float>();
        for(int i = 0;i<currentDNA.portfolio.Count;i++)
        {
            portfolioPartOfDNA.Add(currentDNA.dnaCode[i]);
        }

        int strategyChoosen = RouleteSelection(portfolioPartOfDNA);
        GetComponent<BehaviorTree>().DisableBehavior();
        GetComponent<BehaviorTree>().ExternalBehavior = currentDNA.portfolio[strategyChoosen];
        GetComponent<BehaviorTree>().EnableBehavior();

        //start variables used for fitness calculations
        playerlifeAtBattleStart = player.GetComponent<PlayerManager>().currentHealth;
        originalEngagedPos = player.transform.position;
        timeEngaged = 0;
        distanceTraveled = 0;
        engaged = true;
        

    }


    private void CalculateEngagedVariable()
    {
        if(engaged==true)
        {
            timeEngaged += Time.deltaTime;

            if(Vector3.Distance(originalEngagedPos,player.transform.position)>=0.1f)
            {
                distanceTraveled += 0.1f;
                originalEngagedPos = player.transform.position;
            }
        }
    }

    private int RouleteSelection(List<float> _dnaStrand)
    {
        //get max value basecly the 100%
        float maxValue = 0;
        foreach(int i in _dnaStrand)
        {
            maxValue += i;
        }

        //get probabilities of each member
        List<float> probabilies = new List<float>();
        float current = 0;
        foreach(int i in _dnaStrand)
        {
            current += i;
            probabilies.Add(current / maxValue);
           
        }

        //debug
        foreach(float f in probabilies)
        {
            Debug.Log("probabily: " + f);
        }

        //get a random number and check probabilities
        float randNum = Random.Range(0.0f, 1.0f);

        Debug.Log("random number: " + randNum);
        for(int f = 0; f < probabilies.Count;f++)
        {
            Debug.Log("testing: " + f);
            if(f==0)
            {
                if(randNum<= probabilies[f])
                {
                    Debug.Log("option: " + f + " was choosen");
                    return f;
                }
            }
            else
            {
                int g = f - 1;
                if(randNum> probabilies[g] && randNum< probabilies[f])
                {
                    Debug.Log("this option was choosen: "+ f);
                    return f;
                }
            }
        }


        return 0;
    }


    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(destination, 0.1f);
    }




}
