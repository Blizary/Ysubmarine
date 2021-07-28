using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WFCOBJ : MonoBehaviour
{
    public int original;
    public float probability;

    public List<int> connectUp;
    public List<int> connectDown;
    public List<int> connectLeft;
    public List<int> connectRight;

    public void ResetAllWFC()
    {
        connectUp.Clear();
        connectDown.Clear();
        connectLeft.Clear();
        connectRight.Clear();
        probability = 0;
    }
}
