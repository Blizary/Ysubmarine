using UnityEngine;
using System.Collections;

public abstract class State 
{

	public GameObject agent; 
	public FSM fsm;



	//virtual means this function can be override on classes the extend this class
	public virtual void OnLoad(){}

	public virtual void UpdateState(){}

	public virtual void OnEnterState(){}

	public virtual void OnExitState(){}
}
