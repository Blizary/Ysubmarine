using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class WFCOBJpossibility 
{
    public List<int> availableOptions;
    public bool hasBeenChoosen = false;


    public void CopyData(int size)
    {
        availableOptions = new List<int>();

        for (int i = 0; i < size; i++)
        {
            availableOptions.Add(i);
        }
    }

}
