using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Separation : GroupBehaviour
{
	// separation applies a force in order to separete the current vehicle from its surrounding neighbours
	public override Vector3 Calculate(Vehicle vehicle)
	{
		// no neighbours no force is applied
		if (neighbours.Count == 0)
		{
			return new Vector3();
		}

		Vector3 force = new Vector3();
		foreach (Vehicle v in neighbours)
		{
			Vector3 awayVec = vehicle.transform.position - v.transform.position;
			float dist = awayVec.magnitude;
			awayVec /= dist;
			force += awayVec / dist;

		}

		return force;

	}
}
