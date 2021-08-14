using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using System.Linq;

[System.Serializable]
public class WFCpossibility 
{
    public List<WFCScriptableOBJ> possibleWFC;
    public Vector2Int location;
    public bool hasBeenChoosen = false;
    public bool wasChange = false;
    public void CopyData(List<WFCScriptableOBJ> _availableOpt)
    {
        possibleWFC = new List<WFCScriptableOBJ>();

        for (int i=0;i< _availableOpt.Count;i++)
        {
            possibleWFC.Add(_availableOpt[i]);
        }
    }

    public void CopyConnectionData(WFCScriptableOBJ _option)
    {
        possibleWFC.Clear();
        possibleWFC.Add(_option);
        hasBeenChoosen = true;
        
    }


    public bool CheckIfCollapse()
    {
        //check if there is only 1 possible option available
        if(possibleWFC.Count==1)
        {
            //then collapse this possibility
            return true;
        }
        else if(possibleWFC.Count == 0)
        {
            Debug.Log("WARNING:there is a possibility with no options to collapse too please check the algorithm");
            return false;
        }
        else
        {
            return false;
        }    
    }

    public Tile FinalTile()
    {
        CopyConnectionData(possibleWFC[0]);
        return possibleWFC[0].WFCtile;
    }


    public void RemovePossibility(List<WFCScriptableOBJ> _collapseWFC,int _mode)
    {
        // store the new available possibilies taking in concideration the given list of Scripatble obj
        List<WFCScriptableOBJ> newPossibilites = new List<WFCScriptableOBJ>();
        switch(_mode)
        {
            case 0:
                for (int i = 0; i < _collapseWFC.Count; i++)
                {
                    for (int j = 0; j < _collapseWFC[i].rightWFC.Count; j++)
                    {
                        if (possibleWFC.Contains(_collapseWFC[i].rightWFC[j]))
                        {
                            newPossibilites.Add(_collapseWFC[i].rightWFC[j]);
                        }
                    }
                }
                break;

            case 1:
                for (int i = 0; i < _collapseWFC.Count; i++)
                {
                    for (int j = 0; j < _collapseWFC[i].leftWFC.Count; j++)
                    {
                        if (possibleWFC.Contains(_collapseWFC[i].leftWFC[j]))
                        {
                            newPossibilites.Add(_collapseWFC[i].leftWFC[j]);
                        }
                    }
                }
                break;

            case 2:
                for (int i = 0; i < _collapseWFC.Count; i++)
                {
                    for (int j = 0; j < _collapseWFC[i].upWFC.Count; j++)
                    {
                        if (possibleWFC.Contains(_collapseWFC[i].upWFC[j]))
                        {
                            newPossibilites.Add(_collapseWFC[i].upWFC[j]);
                        }
                    }
                }
                break;

            case 3:
                for (int i = 0; i < _collapseWFC.Count; i++)
                {
                    for (int j = 0; j < _collapseWFC[i].downWFC.Count; j++)
                    {
                        if (possibleWFC.Contains(_collapseWFC[i].downWFC[j]))
                        {
                            newPossibilites.Add(_collapseWFC[i].downWFC[j]);
                        }
                    }
                }
                break;

        }

        newPossibilites = newPossibilites.Distinct().ToList();

        if(newPossibilites.Count!=possibleWFC.Count)
        {
            wasChange = true;
        }
        else
        {
            wasChange = false;
        }

        possibleWFC.Clear();
        for (int i = 0; i < newPossibilites.Count; i++)
        {
            if(!possibleWFC.Contains(newPossibilites[i]))
            {
                possibleWFC.Add(newPossibilites[i]);
            }
            
           
        };
    }

    public void SetNewPossibilities(List<WFCScriptableOBJ> _newPossibilities)
    {
        possibleWFC.Clear();
        for (int i = 0; i < _newPossibilities.Count; i++)
        {
            possibleWFC.Add(_newPossibilities[i]);
        }
      
    }




}
