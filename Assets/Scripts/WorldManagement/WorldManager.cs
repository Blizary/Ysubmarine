using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldManager : MonoBehaviour
{
    [Header("Info")]
    [SerializeField] private GameObject player;
    [SerializeField] private GameObject cityPrefab;
    [SerializeField] private GameObject beaconPrefab;
    [SerializeField] private GameObject loadscreen;

    [Header("Settings")]
    [SerializeField] private int numOfCities;
    [SerializeField] private int numOfBeacons;

    [Header("Lists")]
    [SerializeField] private List<GameObject> playerSpawns;
    [SerializeField] private List<GameObject> citySpawns;
    [SerializeField] private List<GameObject> enemySpawns;
    [SerializeField] private List<GameObject> beaconSpawns;


    private bool waveFunctionComplete = false;
    // Start is called before the first frame update
    void Start()
    {
        playerSpawns = new List<GameObject>();
        citySpawns = new List<GameObject>();
        enemySpawns = new List<GameObject>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void StartManager()
    {
        FindAllSpawns();
        SetPlayerPos();
        SetCities();
        SetBeacons();
        loadscreen.GetComponent<Animator>().SetBool("WFCComplete", true);
        waveFunctionComplete = true;
    }




    private void FindAllSpawns()
    {
        foreach (GameObject playerS in GameObject.FindGameObjectsWithTag("PlayerSpawn"))
        {
            playerSpawns.Add(playerS);
        }

        foreach (GameObject cityS in GameObject.FindGameObjectsWithTag("CitySpawn"))
        {
            citySpawns.Add(cityS);
        }

        foreach (GameObject enemyS in GameObject.FindGameObjectsWithTag("EnemySpawn"))
        {
            enemySpawns.Add(enemyS);
        }

        foreach (GameObject beaconS in GameObject.FindGameObjectsWithTag("BeaconSpawn"))
        {
            beaconSpawns.Add(beaconS);
        }

    }

    private void SetPlayerPos()
    {
        int randomSpawn = Random.Range(0, playerSpawns.Count - 1);
        Vector3 pos = new Vector3(playerSpawns[randomSpawn].transform.position.x, playerSpawns[randomSpawn].transform.position.y, player.transform.position.z);
        player.transform.position = pos;
    }

    private void SetCities()
    {
        for(int i = 0; i<numOfCities;i++)
        {
            int randomSpawn = Random.Range(0, citySpawns.Count - 1);
            Instantiate(cityPrefab, citySpawns[randomSpawn].transform.position, Quaternion.identity, this.transform);
            citySpawns.RemoveAt(randomSpawn);
        }
    }

    private void SetBeacons()
    {
        for (int i = 0; i < numOfBeacons; i++)
        {
            int randomSpawn = Random.Range(0, beaconSpawns.Count - 1);
            Instantiate(beaconPrefab, beaconSpawns[randomSpawn].transform.position, Quaternion.identity, this.transform);
            citySpawns.RemoveAt(randomSpawn);
        }
    }
}
