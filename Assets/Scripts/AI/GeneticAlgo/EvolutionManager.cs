using BehaviorDesigner.Runtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EvolutionType
{
    flipCoin
}


public class EvolutionManager : MonoBehaviour
{
    [Header("Stats")]
    public int numberPerGeneration;
    public int numOfChoosenDNA;
    public int numOfMutations;
    public List<ExternalBehavior> portfolio;
    public GameObject enemyPrefab;
    public EvolutionType evoType;

    [Header("Current enemies")]
    public List<GameObject> availableEnemies;
    public List<DNA> nextDNAs;
    public List<DNA> oldDNAs;

    [Header("Fitness variables")]
    public float damageDoneInfluence;
    public float timeAliveInfluence;
    public float distanceTravelledInfluence;

    public bool originalBreedingDone = false;



    // Start is called before the first frame update
    void Start()
    {
        availableEnemies = new List<GameObject>();
        nextDNAs = new List<DNA>();
        oldDNAs = new List<DNA>();
        OriginalSpawn();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void Breeding()
    {
        //sort to find the best performing
        oldDNAs.Sort(SortByFitness);
        //merge the top numOfChoosenDNA with each other twice to regenerate the population
        List<DNA> bestDNA = new List<DNA>();
        for(int i=0;i<numOfChoosenDNA;i++)
        {
            bestDNA.Add(oldDNAs[i]);
        }

        for (int i = 0; i < numOfChoosenDNA; i++)
        {
            for(int j =0;j<bestDNA.Count;j++)
            {
                if(i!=j)
                {
                    List<int> breedDNACode = CombineDNA(bestDNA[i].dnaCode, bestDNA[j].dnaCode);
                    DNA breedDNA = new DNA();
                    breedDNA.GenerateDNA(portfolio, breedDNACode, damageDoneInfluence, timeAliveInfluence, distanceTravelledInfluence);
                    nextDNAs.Add(breedDNA);
                }
                
            }
        }


        //add the mutated dna to prevent platou
        for (int i=0;i< numOfMutations;i++)
        {
            List<int> dnaCode = new List<int>();
            for (int j = 0; j < portfolio.Count; j++)
            {
                //dna goes from 0 to 10
                int randomint = Random.Range(0, 11);
                dnaCode.Add(randomint);

            }
            DNA newdna = new DNA();
            newdna.GenerateDNA(portfolio, dnaCode, damageDoneInfluence, timeAliveInfluence, distanceTravelledInfluence);
            nextDNAs.Add(newdna);
        }
        
    }

    private void OriginalSpawn()
    {
        for(int i=0;i< numberPerGeneration;i++)
        {
            //generate random dna strands
            List<int> dnaCode = new List<int>();
            for(int j=0;j< portfolio.Count;j++)
            {
                //dna goes from 0 to 10
                int randomint = Random.Range(0, 11);
                dnaCode.Add(randomint);

            }
            DNA newdna = new DNA();
            newdna.GenerateDNA(portfolio, dnaCode, damageDoneInfluence, timeAliveInfluence, distanceTravelledInfluence);
            nextDNAs.Add(newdna);
        }
        originalBreedingDone = true;
    }


    private void CheckPopulation()
    { 
        if(nextDNAs.Count == 0)
        {
            Breeding();
        }
    }


    public void CreateEnemy(Transform _spawnpos)
    {
        //FIX !!!!
        Vector3 pos = _spawnpos.position;
        pos.z = 0;

        //generate new enemy 
        GameObject newEnemy = Instantiate(enemyPrefab, pos, Quaternion.identity, transform);
        DNA newDNA = new DNA() ;
        newDNA.GenerateDNA(portfolio, nextDNAs[0].dnaCode, damageDoneInfluence, timeAliveInfluence, distanceTravelledInfluence);
        newEnemy.GetComponent<EnemyManager>().currentDNA = newDNA;
        availableEnemies.Add(newEnemy);
        newEnemy.SetActive(true);
        nextDNAs.RemoveAt(0);
    }

    private List<int> CombineDNA(List<int> _dna1, List<int> _dna2)
    {
        List<int> newDNA = new List<int>();
        switch(evoType)
        {
            case EvolutionType.flipCoin:
                for(int i = 0;i<_dna1.Count;i++)
                {
                    int coinFlip = Random.Range(0, 2);
                    if(coinFlip==0)//add strand from dna1
                    {
                        newDNA.Add(_dna1[i]);
                    }
                    else//add strand from dna2
                    {
                        newDNA.Add(_dna2[i]);
                    }
                }
                break;
        }

        return newDNA;
    }


    public void AddDeathDNA(DNA _deadDNA, GameObject _deadEnemy)
    {
        //add dna of dead enemy with its fitness
        DNA newDeath = new DNA();
        newDeath.GenerateDNA(_deadDNA.portfolio, _deadDNA.dnaCode, damageDoneInfluence, timeAliveInfluence, distanceTravelledInfluence);
        newDeath.fitness = _deadDNA.fitness;
        oldDNAs.Add(newDeath);

        //remove enemy from available list
        availableEnemies.Remove(_deadEnemy);

        //check if its time to breed
        CheckPopulation();
    }



    static int SortByFitness(DNA _dna, DNA _dna2)
    {
        return _dna.fitness.CompareTo(_dna2.fitness);
    }
}
