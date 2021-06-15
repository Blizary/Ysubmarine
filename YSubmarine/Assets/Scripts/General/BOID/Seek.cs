using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Seek : SteeringBehaviour
{
	public Transform target;

	public override Vector3 Calculate(Vehicle vehicle)
	{
		return helper.Seek(vehicle, target.position);
		
	}


}
