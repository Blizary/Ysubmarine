using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class WFCOBJpossibility 
{
    public List<int> availableOptions;
    public bool hasBeenChoosen = false;
    public Vector2Int location;
    public int originalQuantityOfOptions;
    public float entropy;


    public void CopyData(int size)
    {
        availableOptions = new List<int>();
        for (int i = 0; i < size; i++)
        {
            availableOptions.Add(i);
        }
        originalQuantityOfOptions = availableOptions.Count;
        entropy = availableOptions.Count / originalQuantityOfOptions;
    }

    public bool UpdateOptions(List<int> neightbourOptions)
    {
        List<int> newoptions = new List<int>();
        foreach(int option in neightbourOptions)
        {
            if(availableOptions.Contains(option))
            {
                newoptions.Add(option);
            }
        }
        Debug.Log("list count: " + availableOptions.Count + " - " + newoptions.Count);
        if(availableOptions.Count == newoptions.Count)
        {
            return false;//values remained the same
        }
        else
        {
            availableOptions = newoptions;
            return true;//values were changed
        }
       
    }


    public float CheckIfChoosen()
    {
        if(availableOptions.Count ==1)
        {
            entropy = 0;
            hasBeenChoosen = true;
        }
        else if (availableOptions.Count<1)
        {
            entropy = -1;
        }
        else
        {
            entropy = ((float)availableOptions.Count) / ((float)originalQuantityOfOptions);
        }

        return entropy;
    }

}
