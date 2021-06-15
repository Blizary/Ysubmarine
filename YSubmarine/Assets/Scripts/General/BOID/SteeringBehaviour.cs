using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//abstract so u cant add it to objs

public abstract class SteeringBehaviour : MonoBehaviour
{
	protected SteeringHelper helper = new SteeringHelper(); // protected can be used by this class and all that extend it

	public float weight = 1; // used for prioritization
	public int priority = 0;
	public Color debugColor = Color.white;

	public abstract Vector3 Calculate(Vehicle vehicle);

}
