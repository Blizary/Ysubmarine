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
    [Header("Settings")]
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
    //private List<WFCpossibility> propagateQueu;//stores the tiles that need to be propagate while they wait for their turn\

    private bool ispropagating;
    private bool generationgWater;
    public float checkedTiles;//number of tiles checked during sample update that is used to calculate probabilities
    public List<Vector2Int> waterQueu;
    private float numOfsamples;//stores the number of samples given and its used to calculate probabilities
    [HideInInspector] public bool waveFunctionComplete;

    // NEW ALGO
    public bool debugTools;
    public List<GameObject> availableTileOBJ;
    public List<WFCOBJ> readList;
    WFCOBJpossibility[,] world;


    private Dictionary<int, WFCOBJ> virtualTiles = new Dictionary<int, WFCOBJ>();
    public List<Vector2Int> propagateQueu;//stores the tiles that need to be propagate while they wait for their turn
    public List<WFCOBJpossibility> entropyOrder;// used to organize the tiles by entropy
    public GameObject debugWorld;
    public GameObject debugWorldLast;

    public List<WFCChoice> worldStates;
    public WorldManager worldManager;



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


            Debug.Log("Tile = " + availableOBJ.tileName);
            string upCon = "connections Up: ";
            foreach (int i in availableOBJ.connectUp)
            {
                upCon+=" "+i+",";
            }
            Debug.Log(upCon);

            string downCon = "connections down: ";
            foreach (int i in availableOBJ.connectDown)
            {
                downCon += " " + i + ",";
            }
            Debug.Log(downCon);

            string rightCon = "connections right: ";
            foreach (int i in availableOBJ.connectRight)
            {
                rightCon += " " + i + ",";
            }
            Debug.Log(rightCon);

            string leftCon = "connections left: ";
            foreach (int i in availableOBJ.connectLeft)
            {
                leftCon += " " + i + ",";
            }
            Debug.Log(leftCon);

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

        if(debugTools)
        {
            debugWorld.SetActive(true);
            debugWorldLast.SetActive(true);
            UpdateDebugWorld();
        }
        
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
        //Debug.Break();
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

                        waveFunctionComplete = true;
                        GenerateWorld();
                        Debug.Log("wave function has collapsed!");
                        Debug.Log("valid: " + ValidateWorld());
                        worldManager.StartManager();

                    }
                }


            }
        }



    }


    void Propagate(int i, int j)
    {
        //Debug.Break();
        //remove options on the neighbour that doesnt correspond to 
        //the allowed tiles listed in te WFCscriptableOBj for the list of available tiles

        List<int> newPossibilites = new List<int>();

        List<WFCOBJ> currentWFCOBJ = new List<WFCOBJ>();
        currentWFCOBJ = GetWFCOBJ(world[i, j].availableOptions);
        bool interrupt = false; // variable used to stop the rest of the propagate in case 1 fails

        //down neighbour
        if (i + 1 < lineSize)//is within the size of the grid
        {
            //get all the connection available to the right of this obj
            List<int> connections = Connections(currentWFCOBJ, ConnectionSide.Down);

            if (!world[i + 1, j].hasBeenChoosen)//tile to the right is not yet set
            {
                if (connections.Count != readList.Count)
                {
                    //updates the available option on the next tile and also checks if there where changes 
                    //made and if more propagation is needed
                    if (world[i + 1, j].UpdateOptions(connections))
                    {
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
                }

            }
            else//check if valid
            {
                if(!connections.Contains(world[i + 1, j].availableOptions[0]))
                {
                    Debug.LogError("No solution found backtracking!");
                    ResetWorldToLastChoice();
                    interrupt = true;
                }
            }

        }


        if (!interrupt)
        {
            //up neightbour
            if (i - 1 >= 0)
            {
                List<int> connections = Connections(currentWFCOBJ, ConnectionSide.Up);
                if (!world[i - 1, j].hasBeenChoosen)//tile to the right is not yet set
                {

                    if (connections.Count != readList.Count)
                    {
                        //updates the available option on the next tile and also checks if there where changes 
                        //made and if more propagation is needed
                        if (world[i - 1, j].UpdateOptions(connections))
                        {
                            //add to queu
                            //Debug.Log("Added [" + (i - 1) + "][" + j + "] to the list of propagate");

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
                    }
                }
                else//check if valid
                {
                    if (!connections.Contains(world[i - 1, j].availableOptions[0]))
                    {
                        Debug.LogError("No solution found backtracking!");
                        ResetWorldToLastChoice();
                        interrupt = true;
                    }
                }
            }
           
        }



        if (!interrupt)
        {
            //right neightbour
            if (j + 1 < columSize)
            {
                //get alll the connection available to the left of this obj
                List<int> connections = Connections(currentWFCOBJ, ConnectionSide.Right);
                if (!world[i, j + 1].hasBeenChoosen)//tile to the right is not yet set
                {

                    if (connections.Count != readList.Count)
                    {
                        //updates the available option on the next tile and also checks if there where changes 
                        //made and if more propagation is needed
                        if (world[i, j + 1].UpdateOptions(connections))
                        {
                            //add to queu
                            //Debug.Log("Added [" + i + "][" + (j + 1) + "] to the list of propagate");

                            float entropy = world[i, j + 1].CheckIfChoosen();
                            //Check if it has been choosen and update its entropy
                            if (entropy > 0)
                            {
                                propagateQueu.Add(new Vector2Int(i, j + 1));
                                // entropyOrder.Remove(world[i + 1, j]);
                            }
                            else if (entropy < 0)
                            {
                                Debug.Log("No solution found backtracking!");
                                ResetWorldToLastChoice();
                                interrupt = true;
                            }
                        }
                    }

                }
                else//check if valid
                {
                    if (!connections.Contains(world[i , j+1].availableOptions[0]))
                    {
                        Debug.LogError("No solution found backtracking!");
                        ResetWorldToLastChoice();
                        interrupt = true;
                    }
                }
            }
        }


        if (!interrupt)
        {
            //left neightbour
            if (j - 1 >= 0)
            {
                //get alll the connection available to the left of this obj
                List<int> connections = Connections(currentWFCOBJ, ConnectionSide.Left);
                if (!world[i, j - 1].hasBeenChoosen)//tile to the right is not yet set
                {
                    if (connections.Count != readList.Count)
                    {
                        //updates the available option on the next tile and also checks if there where changes 
                        //made and if more propagation is needed
                        if (world[i, j - 1].UpdateOptions(connections))
                        {
                            //add to queu
                            //Debug.Log("Added [" + i + "][" + (j - 1) + "] to the list of propagate");

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

                    }
                }
                else//check if valid
                {
                    if (!connections.Contains(world[i, j -1].availableOptions[0]))
                    {
                        Debug.LogError("No solution found backtracking!");
                        ResetWorldToLastChoice();
                        interrupt = true;
                    }
                }
            }



        }

        if (!interrupt)
        {
            OrganizebyEntropy();
            propagateQueu.RemoveAt(0);
            if(debugTools)
            {
                UpdateDebugWorld();
            }
            
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
            if(debugTools)
            {
                UpdateDebugWorld();
            }
            choice.pos = _tile.location;
            choice.availabeOptions = _tile.availableOptions.Count;

        }


        if (trueRandom)
        {
            int randomObj;
            List<int> newAvailable = new List<int>();
            int rand = Random.Range(0, _tile.availableOptions.Count);
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
            int rand = Random.Range(0, _tile.availableOptions.Count);
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

        entropyOrder.Remove(_tile);
        propagateQueu.Add(_tile.location);

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

       
        if(debugTools)
        {
            UpdateDebugWorld();
        }
        
        //Debug.Break();
        Vector2Int pos = worldStates[worldStates.Count - 1].pos;
        world[pos.x, pos.y].availableOptions.Remove(worldStates[worldStates.Count - 1].decision);

        //reset the entropy list
        entropyOrder.Clear();
        for (int i = 0; i < lineSize; i++)
        {
            for (int j = 0; j < columSize; j++)
            {
                entropyOrder.Add(world[i, j]);
            }
        }

        OrganizebyEntropy();
        propagateQueu.Clear();
        worldStates.RemoveAt(worldStates.Count - 1);
    }


    private bool ValidateWorld()
    {
        for (int i = 0; i < columSize; i++)
        {
            for (int j = 0; j < lineSize; j++)
            {
                //top limitation
                if (i != 0)
                {
                    //Check top neighbour
                    if (!virtualTiles[world[i, j].availableOptions[0]].connectUp.Contains(world[i - 1, j].availableOptions[0]))
                    {
                        return false;
                    }
                }

                //bottom limitation
                if (i < lineSize - 1)
                {
                    //check bottom neighbour
                    if (!virtualTiles[world[i, j].availableOptions[0]].connectDown.Contains(world[i + 1, j].availableOptions[0]))
                    {
                        return false;
                    }

                }

                //left limitation
                if (j != 0)
                {
                    //check right neightbour
                    if (!virtualTiles[world[i, j].availableOptions[0]].connectRight.Contains(world[i, j-1].availableOptions[0]))
                    {
                        return false;
                    }

                }

                //right limitation
                if (j < columSize - 1)
                {
                    //check left neightbour
                    if (!virtualTiles[world[i, j].availableOptions[0]].connectLeft.Contains(world[i, j+1].availableOptions[0]))
                    {
                        return false;
                    }
                }
            }
        }
        return true;
    }
   


}








