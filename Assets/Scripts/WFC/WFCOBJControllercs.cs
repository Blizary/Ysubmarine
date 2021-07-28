using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class WFCOBJController : MonoBehaviour
{
    public List<WFCScriptableOBJ> dataList; // list of all the available pieces in the board
    [Header("Settings")]
    public bool showPropagation;//if triggered will illustrate the propagation of the wave function collapse
    public bool trueRandom;//if triggered the probability of each tile based on the example is ignored(allows for the creation of bigger maps than the example)
    public bool surroundedByWater;//if triggered maps the map be surrounded by water

    [Header("Examples")]
    public bool updateOBJ;// bool that controls if the WFCcontroller scans the sample or not
    //public GameObject sample;// gameobj where all the objs that represente the sample for the WFC are
    public List<Tilemap>sampleTilemap; //tilemap that holds the sample for the WFC
    public float objSize; // the size of the obj present in the samples they need to cubes

    [Header("Solution")]
    public Tilemap solution;// gameobj where the solution will go
    public int solutionSize;// number of tiles in the solution, it is a square
    public Tile posibility;//tile that show while the wave hasnt collapsed for this tile
    public Tile propagated;// tile that ilustrates the propagation of the wave function collaspe
    public Tile water;//tile for the water


    public Vector2Int posOfSample;// this is the current position being checked on the example by the function UpdateExample
    private WFCpossibility[,] possibilites;//matrix of the possibilites on the tile map
    private List<WFCpossibility> possibilityList;//keeps the possibilites available and is used to sort them by entropy
    private List<WFCpossibility> propagateQueu;//stores the tiles that need to be propagate while they wait for their turn\

    private bool ispropagating;
    private bool generationgWater;
    public float checkedTiles;//number of tiles checked during sample update that is used to calculate probabilities
    private List<Vector2Int> waterQueu;
    private float numOfsamples;//stores the number of samples given and its used to calculate probabilities
    [HideInInspector] public bool waveFunctionComplete;
    // Start is called before the first frame update
    void Start()
    {
        checkedTiles = 0;
        waveFunctionComplete = false;
        numOfsamples = sampleTilemap.Count;
        if (updateOBJ)
        {

            for(int i = 0;i<dataList.Count;i++)
            {
                dataList[i].ResetAllWFC();// reset all the previous data in the scriptable obj to prevent bugs and wrong data
            }
            //set active all the samples
            foreach(Tilemap t in sampleTilemap)
            {
                t.gameObject.SetActive(true);
            }
            //check all the samples given
            while (sampleTilemap.Count!=0)
            {
                TileMapSampleUpdate();
            }
            CalculateProbabilities();//calculate the probabilies once all the samples have been read

        }
        possibilityList = new List<WFCpossibility>();
        possibilites = new WFCpossibility[solutionSize, solutionSize];
        waterQueu = new List<Vector2Int>();
        propagateQueu = new List<WFCpossibility>();
        GenerateSolution();
        generationgWater = true;

    }

    // Update is called once per frame
    void Update()
    {
        if(!waveFunctionComplete)
        {
            ProcessPropagateQueu();
        }
        
    }

    /// <summary>
    /// processes the top of the queu until there is no more tile to process
    /// and the wave function has collapsed
    /// </summary>
    void ProcessPropagateQueu()
    {
        if(!ispropagating)
        {
            if(showPropagation)
            {
                ispropagating = true;
                StartCoroutine(WaitForPropagation());
            }
            
            if (propagateQueu.Count != 0)
            {
                Propagate(propagateQueu[0].location);

            }
            else
            {
                if(generationgWater)
                {
                    if (waterQueu.Count != 0)
                    {
                        WFCScriptableOBJ waterScriptable = FindWFCScriptableObj(water);
                        solution.SetTile(new Vector3Int(waterQueu[0].x, waterQueu[0].y, 0), water);//set the boards to water
                        possibilites[waterQueu[0].x, waterQueu[0].y].CopyConnectionData(waterScriptable);//get the data to the possibility 
                        propagateQueu.Add(possibilites[waterQueu[0].x, waterQueu[0].y]);//queu the propagate
                        possibilityList.Remove(possibilites[waterQueu[0].x, waterQueu[0].y]);//remove it from the list of possibilities since it has been choosen
                        waterScriptable.probability -= 1;
                        waterQueu.RemoveAt(0);
                        NewPropagate();
                    }
                    else
                    {
                        generationgWater = false;
                    }
                }
                else
                {
                    if (possibilityList.Count != 0)
                    {
                        NewPropagate();
                        CollapseBaseOnProbability(possibilityList[0]);

                    }
                    else
                    {
                        //the wave function has collapsed
                        waveFunctionComplete = true;
                        Debug.Log("wave function has collapsed!");
                        
                    }
                }

                
            }
        }
        
    

    }

    /// <summary>
    /// Very very tiny delay in order to better illustrate the propagation effect
    /// </summary>
    /// <returns></returns>
    IEnumerator WaitForPropagation()
    {
        yield return new WaitForSeconds(0.01f);
        ispropagating = false;
    }
    
    /// <summary>
    /// refreshes the tiles that were effected on the last propagate but not set
    /// when the option to show the propagation effect is on this function also resets the visual
    /// </summary>
    void NewPropagate()
    {
        for(int i=0; i<possibilityList.Count; i++)
        {
            possibilityList[i].wasChange = false;
            if(showPropagation)
            {
                //reset visual of tiles
                solution.SetTile(new Vector3Int(possibilityList[i].location.x, possibilityList[i].location.y, 0), posibility);
            }
            
        }
    }

    /// <summary>
    /// reads the given tilemaps in order to obtain the correct constrains for the wfc
    /// </summary>
    void TileMapSampleUpdate()
    {
        //starts at 0,0 and moves following the x axis until it doesnt find a tile
        if(sampleTilemap[0].GetTile(new Vector3Int(posOfSample.x, posOfSample.y, 0)) !=null)
        {
            WFCScriptableOBJ newTile = FindWFCScriptableObj(sampleTilemap[0].GetTile<Tile>(new Vector3Int(posOfSample.x, posOfSample.y, 0)));
            CheckSidesTileMap(posOfSample, newTile);
            newTile.probability += 1;
            posOfSample.x += 1;
            checkedTiles += 1;
           
            TileMapSampleUpdate();

        }
        else
        {
            //once it doesnt find a tile it checks if there are tiles on the line below it ( in the Y axis)
            if(sampleTilemap[0].GetTile(new Vector3Int(0, posOfSample.y-1, 0)) != null)
            {
                if(!trueRandom)
                {
                    if(solutionSize!=0)//has been set before
                    {
                        if(solutionSize!= posOfSample.x + 1)//check if samples have the same size
                        {
                            Debug.Log("WARNING: Sample have different sizes this will affect the probabilities of each tile");
                        }
                    }
                    solutionSize = posOfSample.x+1;
                }
                //if there are tiles below it resets the x axis and calls the function again
                posOfSample.x = 0;
                posOfSample.y -= 1;
                TileMapSampleUpdate();
            }
            else
            {
                //the current sample has finnished being read
                //reset and go to the next one on the list of samples
                sampleTilemap[0].gameObject.SetActive(false);
                posOfSample.x = 0;
                posOfSample.y = 0;
                sampleTilemap.RemoveAt(0);

            }
        }

    }

    /// <summary>
    /// Checks the tiles next to the given tile in order to obtain the correct neightbours
    /// </summary>
    /// <param name="_currentpos"></param>
    /// <param name="_currentData"></param>
    void CheckSidesTileMap(Vector2Int _currentpos, WFCScriptableOBJ  _currentData)
    {
        Tile neighbourTile;
        WFCScriptableOBJ neighbourData;


        //RIGHT tile
        neighbourTile = sampleTilemap[0].GetTile<Tile>(new Vector3Int(_currentpos.x + 1, _currentpos.y, 0));
        if (neighbourTile != null)
        {
            neighbourData = FindWFCScriptableObj(neighbourTile);
            //check if this tile already exists in the list for this direction
            if (!_currentData.rightWFC.Contains(neighbourData))
            {
                // update this neighbour on the list of the obj 
                _currentData.rightWFC.Add(neighbourData);
            }
        }

        //LEFT tile
        neighbourTile = sampleTilemap[0].GetTile<Tile>(new Vector3Int(_currentpos.x - 1, _currentpos.y, 0));
        if (neighbourTile != null)
        {
            neighbourData = FindWFCScriptableObj(neighbourTile);
            if (!_currentData.leftWFC.Contains(neighbourData))
            {
                _currentData.leftWFC.Add(neighbourData);
            }
        }

        //UP tile
        neighbourTile = sampleTilemap[0].GetTile<Tile>(new Vector3Int(_currentpos.x, _currentpos.y+1, 0));
        if (neighbourTile != null)
        {
            neighbourData = FindWFCScriptableObj(neighbourTile);
            if (!_currentData.upWFC.Contains(neighbourData))
            {
                _currentData.upWFC.Add(neighbourData);
            }
        }

        //DOWN tile
        neighbourTile = sampleTilemap[0].GetTile<Tile>(new Vector3Int(_currentpos.x, _currentpos.y-1, 0));
        if (neighbourTile != null)
        {
            neighbourData = FindWFCScriptableObj(neighbourTile);
            if (!_currentData.downWFC.Contains(neighbourData))
            {
                _currentData.downWFC.Add(neighbourData);
            }
        }
    }

    /// <summary>
    /// finds the scriptable obj that has the info related to the given tile
    /// </summary>
    /// <param name="_foundTile"></param>
    /// <returns></returns>
    WFCScriptableOBJ FindWFCScriptableObj(Tile _foundTile)
    {
        for(int i=0;i<dataList.Count;i++)
        {
            if(dataList[i].WFCtile == _foundTile)
            {
                return dataList[i];
            }
        }
        Debug.Log("WARNING: Tile " + _foundTile.name+" not found in the available data set " +
            "please create a scriptable obj with the tile and add to the data list");
        return null;
    }

    void CalculateProbabilities()
    {

        foreach(WFCScriptableOBJ obj in dataList)
        {
            obj.probability = Mathf.RoundToInt (obj.probability / numOfsamples);// gives the average probability
        }

        checkedTiles = Mathf.RoundToInt ( checkedTiles /numOfsamples);// gives the size one 1 sample
    }


    /// <summary>
    /// Starts the generation of the wfc solution
    /// </summary>
    void GenerateSolution()
    {
      

        //Fill the space with nodes of possibility that store information regarding all possible available options for all nodes
        for (int i = 0; i < solutionSize; i++)
        {
            for (int j = 0; j < solutionSize; j++)
            {
                solution.SetTile(new Vector3Int(i, j, 0), posibility);
                WFCpossibility newPosibility = new WFCpossibility();
                newPosibility.location = new Vector2Int(i, j);
                newPosibility.CopyData(dataList);
                newPosibility.possibleWFC.Sort(SortByProbability);
                possibilites[i, j] = newPosibility;
                possibilityList.Add(newPosibility);
            }
        }

        //if the option for water surronding the tilemap then add this to the propagation queu to be properly processed
        if(surroundedByWater)
        {
            //make the boarders of the tilemap water
            for (int i = 0; i < solutionSize; i++)
            {
                for (int j = 0; j < solutionSize; j++)
                {
                    if (j == 0 || i == 0 || j == solutionSize - 1 || i == solutionSize - 1)
                    {
                        waterQueu.Add(new Vector2Int(i, j));
                    }

                }
            }
        }
       

        

       
    }




    void Propagate(Vector2Int _inipos)
    {
        //remove options on the neighbour that doesnt correspond to 
        //the allowed tiles listed in te WFCscriptableOBj for the list of available tiles
        if(showPropagation)
        {
            ispropagating = true;
        }
        
        List<WFCScriptableOBJ> newPossibilites = new List<WFCScriptableOBJ>();

        //right neighbour
        if (_inipos.x + 1 < solutionSize)//is within the size of the grid
        {
            if (!possibilites[_inipos.x + 1, _inipos.y].hasBeenChoosen)//tile to the right is not yet set
            {
                //remove the possibilities on the tile to the right on the initial tile base on the current available possibilities
                possibilites[_inipos.x + 1, _inipos.y].RemovePossibility(possibilites[_inipos.x, _inipos.y].possibleWFC, 0);
                if(showPropagation)
                {
                    solution.SetTile(new Vector3Int(_inipos.x + 1, _inipos.y, 0), propagated);
                }
                

                if (possibilites[_inipos.x + 1, _inipos.y].wasChange)//alteration were made to the tile on the right
                {
                    if (possibilites[_inipos.x + 1, _inipos.y].CheckIfCollapse())//if true only 1 option remaining
                    {
                        solution.SetTile(new Vector3Int(_inipos.x + 1, _inipos.y, 0), possibilites[_inipos.x + 1, _inipos.y].FinalTile());
                        possibilityList.Remove(possibilites[_inipos.x + 1, _inipos.y]);    
                    }

                    if (!propagateQueu.Contains(possibilites[_inipos.x + 1, _inipos.y]))
                    {
                        propagateQueu.Add(possibilites[_inipos.x + 1, _inipos.y]);//add it to the queu so it can be processed
                    }
                }
            }
        }


        //left neightbour
        if (_inipos.x - 1 >= 0)
        {
            if (!possibilites[_inipos.x - 1, _inipos.y].hasBeenChoosen)
            {
                possibilites[_inipos.x - 1, _inipos.y].RemovePossibility(possibilites[_inipos.x, _inipos.y].possibleWFC, 1);
                if(showPropagation)
                {
                    solution.SetTile(new Vector3Int(_inipos.x - 1, _inipos.y, 0), propagated);
                }
                
                if (possibilites[_inipos.x - 1, _inipos.y].wasChange)
                {
                    if (possibilites[_inipos.x - 1, _inipos.y].CheckIfCollapse())
                    {
                        solution.SetTile(new Vector3Int(_inipos.x - 1, _inipos.y, 0), possibilites[_inipos.x - 1, _inipos.y].FinalTile());
                        possibilityList.Remove(possibilites[_inipos.x - 1, _inipos.y]);
                    }
                    if (!propagateQueu.Contains(possibilites[_inipos.x - 1, _inipos.y]))
                    {
                        propagateQueu.Add(possibilites[_inipos.x - 1, _inipos.y]);
                    }
                }
            }
        }


        //Up neightbour
        if (_inipos.y + 1 < solutionSize)
        {

            if (!possibilites[_inipos.x, _inipos.y + 1].hasBeenChoosen)
            {
                possibilites[_inipos.x, _inipos.y + 1].RemovePossibility(possibilites[_inipos.x, _inipos.y].possibleWFC, 2);
                if(showPropagation)
                {
                    solution.SetTile(new Vector3Int(_inipos.x, _inipos.y + 1, 0), propagated);
                }
                
                if (possibilites[_inipos.x, _inipos.y + 1].wasChange)
                {
                    if (possibilites[_inipos.x, _inipos.y + 1].CheckIfCollapse())
                    {
                        solution.SetTile(new Vector3Int(_inipos.x, _inipos.y + 1, 0), possibilites[_inipos.x, _inipos.y + 1].FinalTile());
                        possibilityList.Remove(possibilites[_inipos.x, _inipos.y + 1]);
                    }
                    if (!propagateQueu.Contains(possibilites[_inipos.x, _inipos.y + 1]))
                    {
                        propagateQueu.Add(possibilites[_inipos.x, _inipos.y + 1]);
                    }
                }
            }
        }


        //down neightbour
        if (_inipos.y - 1 >= 0)
        {

            if (!possibilites[_inipos.x, _inipos.y - 1].hasBeenChoosen)
            {
                possibilites[_inipos.x, _inipos.y - 1].RemovePossibility(possibilites[_inipos.x, _inipos.y].possibleWFC, 3);
                if(showPropagation)
                {
                    solution.SetTile(new Vector3Int(_inipos.x, _inipos.y - 1, 0), propagated);
                }
                
                if (possibilites[_inipos.x, _inipos.y - 1].wasChange)
                {
                    if (possibilites[_inipos.x, _inipos.y - 1].CheckIfCollapse())
                    {
                        solution.SetTile(new Vector3Int(_inipos.x, _inipos.y - 1, 0), possibilites[_inipos.x, _inipos.y - 1].FinalTile());
                        possibilityList.Remove(possibilites[_inipos.x, _inipos.y - 1]);
                    }
                    if (!propagateQueu.Contains(possibilites[_inipos.x, _inipos.y - 1]))
                    {
                        propagateQueu.Add(possibilites[_inipos.x, _inipos.y - 1]);
                    }
                }

            }
            
        }

        OrganizebyEntropy();
        possibilites[_inipos.x, _inipos.y].wasChange = true;
        propagateQueu.RemoveAt(0);

    }


    static int SortByEntropy(WFCpossibility _possi1, WFCpossibility _possi2)
    {
        return _possi1.possibleWFC.Count.CompareTo(_possi2.possibleWFC.Count);
    }

    void OrganizebyEntropy()
    {
        //propagateQueu.Sort(SortByEntropy);
        possibilityList.Sort(SortByEntropy);
        

    }

    float GetProbability(WFCScriptableOBJ _check)
    {
        return (((_check.probability / checkedTiles)*100));    
    }

    float GetRelativeProbability(float _probabilityToScale, float _hightesProbability)
    {
        return (_probabilityToScale * 100) / _hightesProbability;
    }

    static int SortByProbability(WFCScriptableOBJ _prob1, WFCScriptableOBJ _prob2)
    {
        return _prob1.probability.CompareTo(_prob2.probability);
    }


    void CollapseBaseOnProbability(WFCpossibility _tile)
    {
        if (trueRandom)
        {
            WFCScriptableOBJ randomObj;
            int rand = Random.Range(0, _tile.possibleWFC.Count);
            randomObj = _tile.possibleWFC[rand];
            _tile.CopyConnectionData(randomObj);
        }
        else
        {
            _tile.CopyConnectionData(RandomTile(_tile));
        }
        
        solution.SetTile(new Vector3Int(_tile.location.x,_tile.location.y,0), _tile.possibleWFC[0].WFCtile);
        possibilityList.Remove(_tile);
        propagateQueu.Add(_tile);

    }

    WFCScriptableOBJ RandomTile(WFCpossibility _tile)
    {
        WFCScriptableOBJ hightsProb = _tile.possibleWFC[0];
        //obtain the hights probability for this group of possibilities
        for(int i=0;i< _tile.possibleWFC.Count;i++)
        {
            if(GetProbability(_tile.possibleWFC[i])> GetProbability(hightsProb))
            {
                hightsProb = _tile.possibleWFC[i];
            }
        }
        //get a random number between 0 and 100 for probability
        int rand = Random.Range(0, 100);
        //sort the possibleWFC by probability
        _tile.possibleWFC.Sort(SortByProbability);

        //run a for loop until we find the 1st number
        //where our random is hightest than the relative probability

        for (int i = 0; i < _tile.possibleWFC.Count; i++)
        {
            if (GetRelativeProbability(GetProbability(_tile.possibleWFC[i]), GetProbability(hightsProb)) > rand)
            {
                _tile.possibleWFC[i].probability -= 1;
                return _tile.possibleWFC[i];
            }
        }
      
        //if none are higher than it is the hights probability ( the 100%)
        return hightsProb;
    }
    
}
