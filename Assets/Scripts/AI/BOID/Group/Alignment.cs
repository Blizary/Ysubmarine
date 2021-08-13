using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Alignment : GroupBehaviour
{
	// alignment makes the vehicle try to go has fast as the average speed of its neighbours
	public override Vector3 Calculate(Vehicle vehicle)
	{

		// no neighbours no influence
		if (neighbours.Count == 0)
		{
			return new Vector3();
		}

		// calculate the average velocity of all neighbours
		Vector3 averageVels = new Vector3();
		foreach (Vehicle v in neighbours)
		{
			averageVels += v.velocity;

		}
		averageVels /= (float)neighbours.Count;

		return averageVels - vehicle.velocity;

	}
}
