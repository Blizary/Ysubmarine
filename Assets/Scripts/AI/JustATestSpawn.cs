using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JustATestSpawn : MonoBehaviour
{
    private EvolutionManager evolutionBrain;
    private List<GameObject> enemySpawns;

    // Start is called before the first frame update
    void Start()
    {
        evolutionBrain = GameObject.FindGameObjectWithTag("Queen").GetComponent<EvolutionManager>();
        enemySpawns = new List<GameObject>();
        foreach (GameObject enemyS in GameObject.FindGameObjectsWithTag("EnemySpawn"))
        {
            enemySpawns.Add(enemyS);
        }
    }

    // Update is called once per frame
    void Update()
    {
        SpawnEnemy();
    }

    void SpawnEnemy()
    {
        if(evolutionBrain.originalBreedingDone)
        {
            if (evolutionBrain.availableEnemies.Count == 0)
            {
                //spawn a test enemy
                //pick random place
                int rand = Random.Range(0, enemySpawns.Count);
                evolutionBrain.CreateEnemy(enemySpawns[rand].transform);
            }
        }
        
    }
}
