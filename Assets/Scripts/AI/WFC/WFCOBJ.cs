using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class WFCOBJ
{
    public int original;
    public string tileName;
    public float probability;

    public List<int> connectUp;
    public List<int> connectDown;
    public List<int> connectLeft;
    public List<int> connectRight;


    public void StartAllWFC()
    {
        connectDown = new List<int>();
        connectLeft = new List<int>();
        connectRight = new List<int>();
        connectUp = new List<int>();
    }
}

//Container for writing data to json
[Serializable]
class Container
{
    public List<WFCOBJ> content = new List<WFCOBJ>();
}
