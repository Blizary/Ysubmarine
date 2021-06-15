using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MooseManager : MonoBehaviour
{
    [Header("Moose data")]
    public GameObject moosePrefab;
    public Vector3 mooseSpawn;
    public int moosePopulationStart;// the number of moose peer group
    public List<GameObject> moosePopulation;
    public int mooseGroups;// the amount of groups of moose around the island


    private GameObject wolf;
    // Start is called before the first frame update
    void Start()
    {
        wolf = GameObject.FindGameObjectWithTag("Wolf");
        //MooseSpawn(moosePrefab, mooseSpawn, moosePopulationStart);
        
    }

    public void MooseSpawn(Vector3 _prefabSpawn)
    {
        for(int i=0;i< moosePopulationStart; i++)
        {
            Vector3 spawnLocation = new Vector3();
            spawnLocation.x = _prefabSpawn.x;
            spawnLocation.y = _prefabSpawn.y;
            spawnLocation.z = 0;

            GameObject newMoose = Instantiate(moosePrefab, spawnLocation, Quaternion.identity, transform);
            newMoose.GetComponent<MooseAI>().StartBehaviours(wolf);
            

        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    void OnDrawGizmos()
    {

    }


}
