using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.Tilemaps;
using Random = UnityEngine.Random;

public enum ConnectionSide { Up, Down, Left, Right };



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
    public GameObject exampleworlds; //obj that holds the samples for the WFC
    public int lineSize; // the horizontal size of the maps
    public int columSize;//the vertical size of the maps




    [Header("Solution")]
    public GameObject solutionWold;// gameobj where the solution will go
    public int solutionSize;// number of tiles in the solution, it is a square
    public Tile posibility;//tile that show while the wave hasnt collapsed for this tile
    public Tile propagated;// tile that ilustrates the propagation of the wave function collaspe
    public Tile water;//tile for the water


    public Vector2Int posOfSample;// this is the current position being checked on the example by the function UpdateExample
    private WFCpossibility[,] possibilites;//matrix of the possibilites on the tile map
    private List<WFCpossibility> possibilityList;//keeps the possibilites available and is used to sort them by entropy
    //private List<WFCpossibility> propagateQueu;//stores the tiles that need to be propagate while they wait for their turn\

    private bool ispropagating;
    private bool generationgWater;
    public float checkedTiles;//number of tiles checked during sample update that is used to calculate probabilities
    public List<Vector2Int> waterQueu;
    private float numOfsamples;//stores the number of samples given and its used to calculate probabilities
    [HideInInspector] public bool waveFunctionComplete;

    // NEW ALGO
    public List<GameObject> availableTileOBJ;
    public List<WFCOBJ> readList;
    WFCOBJpossibility[,] world;


    private Dictionary<int, WFCOBJ> virtualTiles = new Dictionary<int, WFCOBJ>();
    public List<Vector2Int> propagateQueu;//stores the tiles that need to be propagate while they wait for their turn
    public List<WFCOBJpossibility> entropyOrder;// used to organize the tiles by entropy
    public GameObject debugHost;
    public GameObject debugWorld;
    public GameObject debugWorldLast;

    public List<WFCChoice> worldStates;







    /*

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
        */


    private void Start()
    {
        //Start lists
        propagateQueu = new List<Vector2Int>();
        entropyOrder = new List<WFCOBJpossibility>();

        //obtain data from json file
        string path = Application.dataPath + "/WfcObjects.txt";
        string jsons = File.ReadAllText(path);

        Container readConteiner = JsonUtility.FromJson<Container>(jsons);
        foreach (WFCOBJ readOBJ in readConteiner.content)
        {
            readList.Add(readOBJ);
        }

        //Update dictionary
        foreach (WFCOBJ availableOBJ in readList)
        {
            virtualTiles.Add(availableOBJ.original, availableOBJ);
        }



        //create virtual map
        world = new WFCOBJpossibility[lineSize, columSize];
        for (int i = 0; i < lineSize; i++)
        {
            for (int j = 0; j < columSize; j++)
            {
                WFCOBJpossibility newTile = new WFCOBJpossibility();
                newTile.CopyData(availableTileOBJ.Count);
                newTile.location = new Vector2Int(i, j);
                world[i, j] = newTile;
                entropyOrder.Add(newTile);
            }
        }

        if (surroundedByWater)
        {
            //make the boarders of the tilemap water
            for (int i = 0; i < columSize; i++)
            {
                for (int j = 0; j < lineSize; j++)
                {
                    if (j == 0 || i == 0 || j == lineSize - 1 || i == columSize - 1)
                    {
                        waterQueu.Add(new Vector2Int(i, j));
                    }

                }
            }
            generationgWater = true;
        }

        worldStates = new List<WFCChoice>();
        //Debug.Break();

        UpdateDebugWorld();
    }

    /// <summary>
    /// This function is called in the inspector to update the json file that contains the info regarding the objs and their neighbours
    /// It starts by scan each of the children of the give example world as this allows for more variations of neighbours 
    /// And therefor more variations of maps
    /// It goes by each indiviudal map using the horizontal and vertical size given to check the neighbours of each obj
    /// And adds them to a WFCOBJ 
    /// In the end it writes all this info a Json file so it can be used in the begining of the game to generate a Dictionary 
    /// For the WFC algorithm
    /// </summary>
    public void UpdateWFC()
    {
        //Debug.Log("Mark 1");
        availableTileOBJ = new List<GameObject>();
        List<string> verifyUniqueTile = new List<string>();

        Dictionary<int, WFCOBJ> checkingTiles = new Dictionary<int, WFCOBJ>();

        //checks each children 
        foreach (Transform world in exampleworlds.transform)
        {
            //generate a virtual matrix of the world
            int[,] exampleWorld = new int[lineSize, columSize];
            int currentChild = 0;
            for (int i = 0; i < lineSize; i++)
            {
                for (int j = 0; j < columSize; j++)
                {
                    //validade that the tile is available in the list of tiles
                    if (verifyUniqueTile.Contains(world.GetChild(currentChild).GetChild(0).gameObject.name))
                    {
                        int pos = verifyUniqueTile.IndexOf(world.GetChild(currentChild).GetChild(0).gameObject.name);
                        exampleWorld[i, j] = pos;
                        //Debug.Log("Tile was already in the list");

                    }
                    else
                    {
                        verifyUniqueTile.Add(world.GetChild(currentChild).GetChild(0).gameObject.name);
                        availableTileOBJ.Add(world.GetChild(currentChild).GetChild(0).gameObject);
                        exampleWorld[i, j] = availableTileOBJ.Count - 1;
                        //Debug.Log("Added a new tile number: "+ exampleWorld[i, j]);

                    }
                    currentChild += 1;
                }

            }


            //check connections
            for (int i = 0; i < lineSize; i++)
            {
                for (int j = 0; j < columSize; j++)
                {

                    //check if this obj already exist in the dictionary
                    WFCOBJ neighbourUpdates;
                    if (checkingTiles.ContainsKey(exampleWorld[i, j]))
                    {
                        neighbourUpdates = checkingTiles[exampleWorld[i, j]];
                    }
                    else
                    {
                        neighbourUpdates = new WFCOBJ();
                        neighbourUpdates.StartAllWFC();
                        neighbourUpdates.original = exampleWorld[i, j];
                        neighbourUpdates.tileName = verifyUniqueTile[exampleWorld[i, j]];
                    }

                    Debug.Log("" + i + "," + j);
                    //limitations of checks
                    //top limitation
                    if (i != 0)
                    {
                        //Check top neighbour
                        if (!neighbourUpdates.connectUp.Contains(exampleWorld[i - 1, j]))
                        {
                            neighbourUpdates.connectUp.Add(exampleWorld[i - 1, j]);
                            Debug.Log("connection Top: " + exampleWorld[i - 1, j]);
                        }
                    }

                    //bottom limitation
                    if (i < lineSize - 1)
                    {
                        //check bottom neighbour
                        if (!neighbourUpdates.connectDown.Contains(exampleWorld[i + 1, j]))
                        {
                            neighbourUpdates.connectDown.Add(exampleWorld[i + 1, j]);
                            Debug.Log("connection bottum: " + exampleWorld[i + 1, j]);
                        }

                    }

                    //left limitation
                    if (j != 0)
                    {
                        //check right neightbour
                        if (!neighbourUpdates.connectLeft.Contains(exampleWorld[i, j - 1]))
                        {
                            neighbourUpdates.connectLeft.Add(exampleWorld[i, j - 1]);
                            Debug.Log("connection Right: " + exampleWorld[i, j - 1]);
                        }
                    }

                    //right limitation
                    if (j < columSize - 1)
                    {
                        //check left neightbour
                        if (!neighbourUpdates.connectRight.Contains(exampleWorld[i, j + 1]))
                        {
                            neighbourUpdates.connectRight.Add(exampleWorld[i, j + 1]);
                            Debug.Log("connection left: " + exampleWorld[i, j + 1]);
                        }
                    }

                    //Update the dictionary
                    checkingTiles[exampleWorld[i, j]] = neighbourUpdates;

                }
            }
        }

        //Write data to the json file
        //Debug.Log("Mark 3");
        List<WFCOBJ> outlist = new List<WFCOBJ>();
        foreach (int worldOBJ in checkingTiles.Keys)
        {
            outlist.Add(checkingTiles[worldOBJ]);
        }

        Container newContainer = new Container();
        newContainer.content = outlist;
        string jsonS = JsonUtility.ToJson(newContainer);
        File.WriteAllText(Application.dataPath + "/WfcObjects.txt", jsonS);

    }

    void Update()
    {
        if (!waveFunctionComplete)
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
        if (!ispropagating)
        {

            if (propagateQueu.Count != 0)
            {
                Propagate(propagateQueu[0].x, propagateQueu[0].y);

            }
            else
            {
                if (generationgWater)
                {
                    if (waterQueu.Count != 0)
                    {
                        WFCOBJpossibility waterTile = world[waterQueu[0].x, waterQueu[0].y];
                        waterTile.availableOptions.Clear();
                        waterTile.availableOptions.Add(0);
                        waterTile.CheckIfChoosen();
                        entropyOrder.Remove(waterTile);
                        waterQueu.RemoveAt(0);
                        propagateQueu.Add(waterTile.location);
                        //Debug.Break();
                    }
                    else
                    {
                        for (int i = 0; i < columSize; i++)
                        {
                            for (int j = 0; j < lineSize; j++)
                            {
                                if(world[i,j].availableOptions.Count!=1)
                                {
                                    //remove null options
                                    world[i, j].availableOptions.Remove(0);
                                }

                            }
                        }
                        generationgWater = false;
                    }
                }
                else
                {
                    if (entropyOrder.Count != 0)
                    {
                        CollapseBaseOnProbability(entropyOrder[0]);
                    }
                    else
                    {
                        //the wave function has collapsed
                        waveFunctionComplete = true;
                        GenerateWorld();
                        Debug.Log("wave function has collapsed!");

                    }
                }


            }
        }



    }


    void Propagate(int i, int j)
    {
        Debug.Log("Propagating at [" + i + "][" + j + "]");
        //remove options on the neighbour that doesnt correspond to 
        //the allowed tiles listed in te WFCscriptableOBj for the list of available tiles

        List<int> newPossibilites = new List<int>();

        List<WFCOBJ> currentWFCOBJ = new List<WFCOBJ>();
        currentWFCOBJ = GetWFCOBJ(world[i, j].availableOptions);
        bool interrupt = false; // variable used to stop the rest of the propagate in case 1 fails

        //down neighbour
        if (i + 1 < lineSize)//is within the size of the grid
        {
            if (!world[i + 1, j].hasBeenChoosen)//tile to the right is not yet set
            {
                //get alll the connection available to the right of this obj
                List<int> connections = Connections(currentWFCOBJ, ConnectionSide.Down);

                if (connections.Count != readList.Count)
                {
                    //updates the available option on the next tile and also checks if there where changes 
                    //made and if more propagation is needed
                    if (world[i + 1, j].UpdateOptions(connections))
                    {
                        //add to queu
                        Debug.Log("Added [" + (i + 1) + "][" + j + "] to the list of propagate");

                        float entropy = world[i + 1, j].CheckIfChoosen();
                        //Check if it has been choosen and update its entropy
                        if (entropy > 0)
                        {
                            propagateQueu.Add(new Vector2Int(i + 1, j));
                            // entropyOrder.Remove(world[i + 1, j]);
                        }
                        else if (entropy < 0)
                        {
                            Debug.LogError("No solution found backtracking!");
                            ResetWorldToLastChoice();
                            interrupt = true;
                        }
                    }

                    string availableString = "Available options: ";
                    foreach (int point in world[i + 1, j].availableOptions)
                    {
                        availableString += "" + point + ", ";
                    }
                    Debug.Log(availableString);
                    Debug.Log("Entropy of tile [" + (i + 1) + "][" + j + "] is " + world[i + 1, j].entropy);
                    Debug.Log("[" + (i + 1) + "][" + j + "] : has been choose = " + world[i + 1, j].hasBeenChoosen);
                }

            }

        }


        if (!interrupt)
        {
            //up neightbour
            if (i - 1 >= 0)
            {
                if (!world[i - 1, j].hasBeenChoosen)//tile to the right is not yet set
                {
                    List<int> connections = Connections(currentWFCOBJ, ConnectionSide.Up);

                    if (connections.Count != readList.Count)
                    {
                        //updates the available option on the next tile and also checks if there where changes 
                        //made and if more propagation is needed
                        if (world[i - 1, j].UpdateOptions(connections))
                        {
                            //add to queu
                            Debug.Log("Added [" + (i - 1) + "][" + j + "] to the list of propagate");

                            float entropy = world[i - 1, j].CheckIfChoosen();
                            //Check if it has been choosen and update its entropy
                            if (entropy > 0)
                            {
                                propagateQueu.Add(new Vector2Int(i - 1, j));
                                // entropyOrder.Remove(world[i - 1, j]);
                            }
                            else if (entropy < 0)
                            {
                                Debug.LogError("No solution found backtracking!");
                                ResetWorldToLastChoice();
                                interrupt = true;
                            }
                        }

                        string availableString = "Available options: ";
                        foreach (int point in world[i - 1, j].availableOptions)
                        {
                            availableString += "" + point + ", ";
                        }
                        Debug.Log(availableString);
                        Debug.Log("Entropy of tile [" + (i - 1) + "][" + j + "] is " + world[i - 1, j].entropy);
                        Debug.Log("[" + (i + 1) + "][" + j + "] : has been choose = " + world[i - 1, j].hasBeenChoosen);
                    }
                }
            }
        }



        if (!interrupt)
        {
            //right neightbour
            if (j + 1 < columSize)
            {
                if (!world[i, j + 1].hasBeenChoosen)//tile to the right is not yet set
                {
                    //get alll the connection available to the left of this obj
                    List<int> connections = Connections(currentWFCOBJ, ConnectionSide.Right);

                    if (connections.Count != readList.Count)
                    {
                        //updates the available option on the next tile and also checks if there where changes 
                        //made and if more propagation is needed
                        if (world[i, j + 1].UpdateOptions(connections))
                        {
                            //add to queu
                            Debug.Log("Added [" + i + "][" + (j + 1) + "] to the list of propagate");

                            float entropy = world[i, j + 1].CheckIfChoosen();
                            //Check if it has been choosen and update its entropy
                            if (entropy > 0)
                            {
                                propagateQueu.Add(new Vector2Int(i, j + 1));
                                // entropyOrder.Remove(world[i + 1, j]);
                            }
                            else if (entropy < 0)
                            {
                                Debug.LogError("No solution found backtracking!");
                                ResetWorldToLastChoice();
                                interrupt = true;
                            }
                        }

                        string availableString = "Available options: ";
                        foreach (int point in world[i, j + 1].availableOptions)
                        {
                            availableString += "" + point + ", ";
                        }
                        Debug.Log(availableString);
                        Debug.Log("Entropy of tile [" + i + "][" + (j + 1) + "] is " + world[i, j + 1].entropy);
                        Debug.Log("[" + i + "][" + (j + 1) + "] : has been choose = " + world[i, j + 1].hasBeenChoosen);
                    }

                }
            }
        }


        if (!interrupt)
        {
            //left neightbour
            if (j - 1 >= 0)
            {
                if (!world[i, j - 1].hasBeenChoosen)//tile to the right is not yet set
                {
                    //get alll the connection available to the left of this obj
                    List<int> connections = Connections(currentWFCOBJ, ConnectionSide.Left);

                    if (connections.Count != readList.Count)
                    {
                        //updates the available option on the next tile and also checks if there where changes 
                        //made and if more propagation is needed
                        if (world[i, j - 1].UpdateOptions(connections))
                        {
                            //add to queu
                            Debug.Log("Added [" + i + "][" + (j - 1) + "] to the list of propagate");

                            float entropy = world[i, j - 1].CheckIfChoosen();
                            //Check if it has been choosen and update its entropy
                            if (entropy > 0)
                            {
                                propagateQueu.Add(new Vector2Int(i, j - 1));
                                // entropyOrder.Remove(world[i - 1, j]);
                            }
                            else if (entropy < 0)
                            {
                                Debug.LogError("No solution found backtracking!");
                                ResetWorldToLastChoice();
                                interrupt = true;
                            }
                        }

                        string availableString = "Available options: ";
                        foreach (int point in world[i, j - 1].availableOptions)
                        {
                            availableString += "" + point + ", ";
                        }
                        Debug.Log(availableString);
                        Debug.Log("Entropy of tile [" + i + "][" + (j - 1) + "] is " + world[i, j - 1].entropy);
                        Debug.Log("[" + i + "][" + (j - 1) + "] : has been choose = " + world[i, j - 1].hasBeenChoosen);
                    }
                }
            }



        }

        if (!interrupt)
        {
            OrganizebyEntropy();
            propagateQueu.RemoveAt(0);
            UpdateDebugWorld();
        }
        //Debug.Break();

    }


    /// <summary>
    /// Ask the dictionary for all the WFCOBJ so then we can access their possibility of sides
    /// </summary>
    /// <param name="currentpossibilities"></param>
    /// <returns></returns>
    List<WFCOBJ> GetWFCOBJ(List<int> currentpossibilities)
    {
        List<WFCOBJ> outList = new List<WFCOBJ>();
        foreach (int p in currentpossibilities)
        {
            outList.Add(virtualTiles[p]);
        }
        return outList;
    }

    /// <summary>
    /// Get all the connections for a specific side of a specfic list of WFCOBJ 
    /// that can be obtained by getting the values with the keys of the world variable in the virtual tiles dictionary
    /// </summary>
    /// <param name="currentWFCOBJ"></param>
    /// <param name="side"></param>
    /// <returns></returns>
    List<int> Connections(List<WFCOBJ> currentWFCOBJ, ConnectionSide side)
    {
        List<int> connections = new List<int>();

        foreach (WFCOBJ obj in currentWFCOBJ)
        {
            switch (side)
            {
                case ConnectionSide.Down:
                    connections = AddValuesToListOfInt(connections, obj.connectDown);
                    break;
                case ConnectionSide.Up:
                    connections = AddValuesToListOfInt(connections, obj.connectUp);
                    break;
                case ConnectionSide.Right:
                    connections = AddValuesToListOfInt(connections, obj.connectRight);
                    break;
                case ConnectionSide.Left:
                    connections = AddValuesToListOfInt(connections, obj.connectLeft);
                    break;

            }
        }
        return connections;
    }

    /// <summary>
    /// checks if a value is already present in the originaly given list 
    /// and if not present adds it to the list
    /// returns the list with the added new elements
    /// </summary>
    /// <param name="originalList"></param>
    /// <param name="newList"></param>
    /// <returns></returns>
    private List<int> AddValuesToListOfInt(List<int> originalList, List<int> newList)
    {
        foreach (int connectOBJ in newList)
        {
            if (!originalList.Contains(connectOBJ))
            {
                originalList.Add(connectOBJ);
            }
        }
        return originalList;
    }


    static int SortByEntropy(WFCOBJpossibility _possi1, WFCOBJpossibility _possi2)
    {
        return _possi1.entropy.CompareTo(_possi2.entropy);
    }

    void OrganizebyEntropy()
    {

        entropyOrder.Sort(SortByEntropy);

    }

    void CollapseBaseOnProbability(WFCOBJpossibility _tile)
    {
        //Debug.Break();
        WFCChoice choice = new WFCChoice();
        bool choicemade = _tile.availableOptions.Count != 1;
        //check if a choice will be made or if it was a tile with a already choosen option
        if (choicemade)
        {
            //create a save of the current world state
            choice.stateOfWorld = new WFCOBJpossibility[lineSize, columSize];
            for (int i = 0; i < columSize; i++)
            {
                for (int j = 0; j < lineSize; j++)
                {
                    WFCOBJpossibility a = new WFCOBJpossibility();
                    a.availableOptions = new List<int>();
                    foreach(int point in world[i,j].availableOptions)
                    {
                        a.availableOptions.Add(point);
                    }
                    a.location = world[i, j].location;
                    a.hasBeenChoosen = world[i, j].hasBeenChoosen;
                    a.entropy = world[i, j].entropy;
                    choice.stateOfWorld[i, j] = a;
                }
            }
            UpdateDebugWorld();
           // Debug.Break();
            choice.pos = _tile.location;
            choice.availabeOptions = _tile.availableOptions.Count;
            choice.stateOfEntropy = new List<WFCOBJpossibility>();

            foreach(WFCOBJpossibility possi in entropyOrder)
            {
                choice.stateOfEntropy.Add(possi);
            }
        }





        Debug.Log("PULLED TILE FROM ENTROPY");
        if (trueRandom)
        {
            int randomObj;
            List<int> newAvailable = new List<int>();
            int rand = Random.Range(0, _tile.availableOptions.Count - 1);
            randomObj = _tile.availableOptions[rand];
            choice.decision = randomObj;
            newAvailable.Add(randomObj);
            _tile.availableOptions.Clear();
            _tile.availableOptions.Add(newAvailable[0]);
            _tile.CheckIfChoosen();

        }
        else
        {
            int randomObj;
            List<int> newAvailable = new List<int>();
            int rand = Random.Range(0, _tile.availableOptions.Count - 1);
            randomObj = _tile.availableOptions[rand];
            choice.decision = randomObj;
            newAvailable.Add(randomObj);
            _tile.availableOptions.Clear();
            _tile.availableOptions.Add(newAvailable[0]);
            _tile.CheckIfChoosen();

        }

        if (choicemade)
        {
            worldStates.Add(choice);
        }

        Debug.Log(_tile.availableOptions[0]);
        Debug.Log(_tile.hasBeenChoosen);
        Debug.Log(_tile.entropy);
        entropyOrder.Remove(_tile);
        propagateQueu.Add(_tile.location);
        //Debug.Break();

    }


    /// <summary>
    /// Once the wave function has collapsed this function reads the virtual world of ints and spawns the correct objs
    /// based on the dictionary
    /// </summary>
    void GenerateWorld()
    {
        int currentChild = 0;
        for (int i = 0; i < lineSize; i++)
        {
            for (int j = 0; j < columSize; j++)
            {
                WFCOBJ newObj = virtualTiles[world[i, j].availableOptions[0]];
                GameObject newTileObj = availableTileOBJ[newObj.original];
                Instantiate(newTileObj, solutionWold.transform.GetChild(currentChild).transform.position, Quaternion.identity, solutionWold.transform.GetChild(currentChild).transform);
                currentChild += 1;
            }

        }
    }


    void UpdateDebugWorld()
    {
        int currentChild = 0;
        for (int i = 0; i < lineSize; i++)
        {
            for (int j = 0; j < columSize; j++)
            {
                string availableString = "["+ i+"]["+j+"] -- ";
                foreach (int point in world[i, j].availableOptions)
                {
                    availableString += "" + point + ", ";
                }

                debugWorld.transform.GetChild(currentChild).GetComponent<TextMeshProUGUI>().text = availableString;
                currentChild += 1;
            }

            
        }

        if(worldStates.Count!=0)
        {
            currentChild = 0;
            for (int i = 0; i < lineSize; i++)
            {
                for (int j = 0; j < columSize; j++)
                {
                    string availableString = "[" + i + "][" + j + "] -- ";
                    foreach (int point in worldStates[worldStates.Count-1].stateOfWorld[i, j].availableOptions)
                    {
                        availableString += "" + point + ", ";
                    }

                    debugWorldLast.transform.GetChild(currentChild).GetComponent<TextMeshProUGUI>().text = availableString;
                    currentChild += 1;
                }


            }
        }
        
    }


    void ResetWorldToLastChoice()
    {
        //Debug.Break();

        for (int i = 0; i < columSize; i++)
        {
            for (int j = 0; j < lineSize; j++)
            {
                WFCOBJpossibility a = new WFCOBJpossibility();
                a.availableOptions = new List<int>();
                foreach (int point in worldStates[worldStates.Count - 1].stateOfWorld[i, j].availableOptions)
                {
                    a.availableOptions.Add(point);
                }
                a.location = worldStates[worldStates.Count - 1].stateOfWorld[i, j].location;
                a.hasBeenChoosen = worldStates[worldStates.Count - 1].stateOfWorld[i, j].hasBeenChoosen;
                a.entropy = worldStates[worldStates.Count - 1].stateOfWorld[i, j].entropy;
                world[i, j] = a;
            }
        }

       

        UpdateDebugWorld();
        //Debug.Break();
        Vector2Int pos = worldStates[worldStates.Count - 1].pos;
        world[pos.x, pos.y].availableOptions.Remove(worldStates[worldStates.Count - 1].decision);
        //reset the entropy list
        entropyOrder.Clear();
        //Debug.Break();
        for (int i = 0; i < lineSize; i++)
        {
            for (int j = 0; j < columSize; j++)
            {
                entropyOrder.Add(world[i, j]);
            }
        }


        /*
        foreach (WFCOBJpossibility possi in worldStates[worldStates.Count - 1].stateOfEntropy)
        {
            if(possi.entropy<0)
            {
                //reset entropy
                entropyOrder.Add(world[possi.location.x, possi.location.y]);
            }
            else
            {
                entropyOrder.Add(possi);
            }

        }
        */

        propagateQueu.Clear();
        worldStates.RemoveAt(worldStates.Count - 1);
        //Debug.Break();
    }


}








