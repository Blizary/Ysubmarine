using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ConstantSpeed : SteeringBehaviour
{
	public float speed;

	public override Vector3 Calculate(Vehicle vehicle)
	{
		Vector3 force = vehicle.direction * speed;


		return force - vehicle.velocity;
	}

}
