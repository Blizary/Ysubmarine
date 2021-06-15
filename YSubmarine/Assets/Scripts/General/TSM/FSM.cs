using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class FSM : MonoBehaviour
{

	private Dictionary<Type, State> states;

	private State activeState;

	private State globalState;


	// Awake is called before Start
	// and there are instances in witch Start doesnt even initiate
	void Awake () 
	{
		states = new Dictionary<Type,State> ();
	}
	
	// Update is called once per frame
	void Update () 
	{
		if (globalState != null)
		{
			globalState.UpdateState ();
		}

		if (activeState !=null ) 
		{
			activeState.UpdateState ();
		}
	}

	public T LoadState<T>()where T:State, new() // where T extends the State class and it must have an empty constructor
	{
		T s = new T ();
		Type type = typeof(T);
		states.Add (type, s);
		s.agent = gameObject;
		s.fsm = this;
		s.OnLoad ();
		return s;


	}

	public void ActivateState<T> () where T:State
	{
		Type type = typeof(T);
		State s = states [type];

		if (s == activeState)
		{
			return;
		}

		if (activeState != null)
		{
			activeState.OnExitState ();

		}

		activeState = s;
		s.OnEnterState ();
	}


	public bool IsActive<T>()
	{
		if (activeState == null) 
		{
			return false;

		}
		Type type = typeof(T);
		return activeState.GetType () == type;

	}


	public void ActivateGlobalState<T>() where T:State
	{
		Type type = typeof(T);
		State s = states [type];

		if (s == globalState)
		{
			return;
		}

		if (globalState != null)
		{
			globalState.OnExitState ();

		}

		globalState = s;
		s.OnEnterState ();

	}

}

