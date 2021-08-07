using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class WFCChoice 
{
    public WFCOBJpossibility[,] stateOfWorld;
    public Vector2Int pos;
    public int decision;
    public int availabeOptions;

}

