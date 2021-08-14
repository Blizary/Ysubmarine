using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cohesion : GroupBehaviour
{
	// cohesion makes the vehicle move towards the center/average position of all its neighbours
	public override Vector3 Calculate(Vehicle vehicle)
	{
		// if there are no neighbours then there is no cohesion
		if (neighbours.Count == 0)
		{
			return Vector3.zero;

		}

		// calculates the center of mass, based on the neighbours, in order to direct the agent to it
		Vector3 centerOfMass = new Vector3();
		foreach (Vehicle v in neighbours)
		{
			centerOfMass += v.transform.position;
		}
		centerOfMass /= (float)neighbours.Count;

		return helper.Seek(vehicle, centerOfMass);
	}

}
