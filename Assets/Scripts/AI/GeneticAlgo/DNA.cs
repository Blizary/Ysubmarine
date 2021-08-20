using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviorDesigner.Runtime;
using System;

[Serializable]
public class DNA 
{
    public List<ExternalBehavior> portfolio;
    public List<float> dnaCode;
    public float fitness;

    private float damageDoneInfluence;
    private float timeAliveInfluence;
    private float distanceTravelledInfluence;

    

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void GenerateDNA(List<ExternalBehavior> _availableStrategies, List<float> _newDNACode, float _damageDoneInfluence, float _timeAliveInfluence, float _distanceTravelledInfluence)
    {
        portfolio = new List<ExternalBehavior>();
        dnaCode = new List<float>();

        foreach (ExternalBehavior strat in _availableStrategies)
        {
            portfolio.Add(strat);
        }

        foreach (float code in _newDNACode)
        {
            dnaCode.Add(code);
        }

        fitness = 0;
        damageDoneInfluence = _damageDoneInfluence;
        timeAliveInfluence = _timeAliveInfluence;
        distanceTravelledInfluence = _distanceTravelledInfluence;
    }

    public void CalculateFitness(float _damageDealt,float _timeEngaged,float _distanceTravelled)
    {
        fitness = (_damageDealt * damageDoneInfluence) + (_timeEngaged * timeAliveInfluence) + (_distanceTravelled * distanceTravelledInfluence);
    }
}
