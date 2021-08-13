using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StayInBounds : SteeringBehaviour
{
	//[HideInInspector]

	public Bounds bounds;

	public float padding = 0;



	public override Vector3 Calculate(Vehicle vehicle)
	{
		Vector3 pos = vehicle.transform.position;
		Vector3 force = new Vector3();

		Vector3 paddingV = new Vector3(padding, padding, padding);
		Vector3 min = bounds.min + paddingV;
		Vector3 max = bounds.max - paddingV;



		if (pos.x < min.x)
		{
			force.x = min.x - pos.x;
		}
		else if (pos.x > max.x)
		{
			force.x = max.x - pos.x;

		}



		if (pos.y < min.y)
		{
			force.y = min.y - pos.y;
		}
		else if (pos.y > max.y)
		{
			force.y = max.y - pos.y;

		}


		return force;
	}


}
