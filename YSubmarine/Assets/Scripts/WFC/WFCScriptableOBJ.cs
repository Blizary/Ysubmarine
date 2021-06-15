using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/WFC", order = 1)]
public class WFCScriptableOBJ : ScriptableObject
{

 
    public Tile WFCtile;
    public float probability;

    [SerializeField] public List<WFCScriptableOBJ> upWFC; // list of obj connect to this obj on the top position
    [SerializeField] public List<WFCScriptableOBJ> downWFC; // list of obj connect to this obj on the down position
    [SerializeField] public List<WFCScriptableOBJ> leftWFC; // list of obj connect to this obj on the left position
    [SerializeField] public List<WFCScriptableOBJ> rightWFC; // list of obj connect to this obj on the right position


    public void ResetAllWFC()
    {
        upWFC.Clear();
        downWFC.Clear();
        leftWFC.Clear();
        rightWFC.Clear();
        probability = 0;
    }

}
