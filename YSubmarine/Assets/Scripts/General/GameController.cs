using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class GameController : MonoBehaviour
{
    public List<Tile> deerCanSpawn;//list of tiles where the deer can spawn
    public List<Tile> wolfCanHaveHome;//list of tiles where a wolf can spawn
    [SerializeField] private WFCController wfcController;
    [SerializeField] private GameObject boidManager;
    [SerializeField] private Tilemap islandMap;

    private List<Vector2Int> deerPosibleSpawns;
    public List<Vector2Int> wolfPosibleSpawns;
    private bool deerHaveSpawn;
    private bool wolfHaveSpawn;
    private GameObject wolf;

    // Start is called before the first frame update
    void Start()
    {
        deerPosibleSpawns = new List<Vector2Int>();
        wolfPosibleSpawns = new List<Vector2Int>();
        wolf = GameObject.FindGameObjectWithTag("Wolf");
        wolf.SetActive(false);
        deerHaveSpawn = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (wfcController.waveFunctionComplete)//wave function has collapse
        {
            //spawn deer
            if (!deerHaveSpawn)
            {
                //1st find a place where it is safe to spawn the deer
                //cycle tilemap
                for (int i = 0; i < wfcController.solutionSize; i++)
                {
                    for (int j = 0; j < wfcController.solutionSize; j++)
                    {
                        if (GoodForDear(new Vector2Int(i, j)))
                        {
                            deerPosibleSpawns.Add(new Vector2Int(i, j));
                        }
                    }
                }

                //spawn each group of deer
                for(int i = 0;i< boidManager.GetComponent<MooseManager>().mooseGroups;i++)
                {
                    if(deerPosibleSpawns.Count!=0)
                    {
                        //pick a random available location to spawn deer
                        int rand = Random.Range(0, deerPosibleSpawns.Count);
                        boidManager.GetComponent<MooseManager>().MooseSpawn(new Vector3(deerPosibleSpawns[rand].x, deerPosibleSpawns[rand].y, 0));
                        //remove this point from the list of spawn locations to prevent chaos
                        deerPosibleSpawns.RemoveAt(rand);
                    }
                    else
                    {
                        Debug.Log("WARNING: there arent enought spots to spawn more deer");
                    }
                  
                }
                deerHaveSpawn = true;

            }

            //spawn wolf
            if(!wolfHaveSpawn)
            {
                //1st find a place where it is safe to move the wolf to
                //cycle tilemap
                for (int i = 0; i < wfcController.solutionSize; i++)
                {
                    for (int j = 0; j < wfcController.solutionSize; j++)
                    {
                        if (GoodForWolf(new Vector2Int(i, j)))
                        {
                            if(WolfSpawnOption(new Vector2Int(i, j))!=Vector2Int.zero)
                            {
                                wolfPosibleSpawns.Add(WolfSpawnOption(new Vector2Int(i, j)));
                            }
                            
                        }
                    }
                }


                //move the wolf to one of the locations found
                int rand = Random.Range(0, wolfPosibleSpawns.Count);
                wolf.transform.position = new Vector3(wolfPosibleSpawns[rand].x, wolfPosibleSpawns[rand].y, 0);
                wolf.SetActive(true);
                wolfHaveSpawn = true;
            }
        }
        
    }

    /// <summary>
    /// checks if the tile at the position given is a good tile to spawn deer
    /// the requirements for this are specific they the list of possible tiles where a deer can spawn
    /// these are store in the deercanSpawn list
    /// </summary>
    /// <param name="_pos"></param>
    /// <returns></returns>
    bool GoodForDear(Vector2Int _pos)
    {
        //check if center position is safe to spawn
        if (deerCanSpawn.Contains(islandMap.GetTile<Tile>(new Vector3Int(_pos.x, _pos.y, 0))))
        {
            //check all sides
            if (!deerCanSpawn.Contains(islandMap.GetTile<Tile>(new Vector3Int(_pos.x-1, _pos.y, 0))))
            {
                return false;
            }
            if (!deerCanSpawn.Contains(islandMap.GetTile<Tile>(new Vector3Int(_pos.x + 1, _pos.y, 0))))
            {
                return false;
            }
            if (!deerCanSpawn.Contains(islandMap.GetTile<Tile>(new Vector3Int(_pos.x, _pos.y-1, 0))))
            {
                return false;
            }
            if (!deerCanSpawn.Contains(islandMap.GetTile<Tile>(new Vector3Int(_pos.x, _pos.y+1, 0))))
            {
                return false;
            }
            if (!deerCanSpawn.Contains(islandMap.GetTile<Tile>(new Vector3Int(_pos.x - 1, _pos.y-1, 0))))
            {
                return false;
            }
            if (!deerCanSpawn.Contains(islandMap.GetTile<Tile>(new Vector3Int(_pos.x - 1, _pos.y+1, 0))))
            {
                return false;
            }
            if (!deerCanSpawn.Contains(islandMap.GetTile<Tile>(new Vector3Int(_pos.x + 1, _pos.y - 1, 0))))
            {
                return false;
            }
            if (!deerCanSpawn.Contains(islandMap.GetTile<Tile>(new Vector3Int(_pos.x + 1, _pos.y + 1, 0))))
            {
                return false;
            }
            return true;
        }
        return false;
    }


    /// <summary>
    /// Checks if the tile at the position given is a good tile to move the wolf to
    /// The wolf spawns near in woods and mountains but since we dont want it to spawn inside these tiles
    /// we need to find if the neighbour tiles are walkable
    /// For that we use the deercanspawn list of tiles
    /// </summary>
    /// <param name="_pos"></param>
    /// <returns></returns>
    bool GoodForWolf(Vector2Int _pos)
    {
        //check if center position is safe to spawn
        if (wolfCanHaveHome.Contains(islandMap.GetTile<Tile>(new Vector3Int(_pos.x, _pos.y, 0))))
        {
            
            return true;
        }
        return false;
    }

    Vector2Int WolfSpawnOption(Vector2Int _pos)
    {
        //check all sides
        if (deerCanSpawn.Contains(islandMap.GetTile<Tile>(new Vector3Int(_pos.x - 1, _pos.y, 0))))
        {
            return new Vector2Int(_pos.x - 1, _pos.y);
        }
        if (!deerCanSpawn.Contains(islandMap.GetTile<Tile>(new Vector3Int(_pos.x + 1, _pos.y, 0))))
        {
            return new Vector2Int(_pos.x + 1, _pos.y);
        }
        if (!deerCanSpawn.Contains(islandMap.GetTile<Tile>(new Vector3Int(_pos.x, _pos.y - 1, 0))))
        {
            return new Vector2Int(_pos.x, _pos.y - 1);
        }
        if (!deerCanSpawn.Contains(islandMap.GetTile<Tile>(new Vector3Int(_pos.x, _pos.y + 1, 0))))
        {
            return new Vector2Int(_pos.x, _pos.y + 1);
        }
        if (!deerCanSpawn.Contains(islandMap.GetTile<Tile>(new Vector3Int(_pos.x - 1, _pos.y - 1, 0))))
        {
            return new Vector2Int(_pos.x - 1, _pos.y - 1);
        }
        if (!deerCanSpawn.Contains(islandMap.GetTile<Tile>(new Vector3Int(_pos.x - 1, _pos.y + 1, 0))))
        {
            return new Vector2Int(_pos.x - 1, _pos.y + 1);
        }
        if (!deerCanSpawn.Contains(islandMap.GetTile<Tile>(new Vector3Int(_pos.x + 1, _pos.y - 1, 0))))
        {
            return new Vector2Int(_pos.x + 1, _pos.y - 1);
        }
        if (!deerCanSpawn.Contains(islandMap.GetTile<Tile>(new Vector3Int(_pos.x + 1, _pos.y + 1, 0))))
        {
            return new Vector2Int(_pos.x + 1, _pos.y + 1);
        }

        return Vector2Int.zero;
    }


    /// <summary>
    /// Returns a random point around the given point but never the point itself
    /// </summary>
    /// <param name="_pos"></param>
    /// <returns></returns>
    Vector2Int RandomAroundPoint(Vector2Int _pos)
    {
        Vector2Int randV = new Vector2Int(Random.Range(-1, 1), Random.Range(-1, 1));
        if(randV == Vector2Int.zero)
        {
            RandomAroundPoint(_pos);
        }
        else
        {
            randV.x = _pos.x + randV.x;
            randV.y = _pos.x + randV.y;
            return randV;
        }

        return _pos;
    }

}
