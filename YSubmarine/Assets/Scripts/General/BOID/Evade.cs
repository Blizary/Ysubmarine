using UnityEngine;
using System.Collections;

public class Evade : SteeringBehaviour 
{
	public Vehicle evader;
	public float panicDistance;

	public override Vector3 Calculate (Vehicle vehicle)
	{
		Vector3 toEvader = vehicle.transform.position - evader.transform.position;

		float relativeDir = Vector3.Dot (vehicle.direction, evader.direction); // this is for when the target is ahead of this vehicle
		if (relativeDir < -0.95)  // -0.95 is the arccosine of 18
		{
			return helper.Seek (vehicle,evader.transform.position);

		}

		float lookAhead = toEvader.magnitude / (evader.velocity.magnitude + vehicle.maxSpeed); 
		Vector3 target = evader.transform.position + (evader.direction * lookAhead);

		return helper.Flee (vehicle, target,panicDistance);

	}
}
