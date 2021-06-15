using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SteeringHelper

{
	public Vector3 Seek(Vehicle vehicle, Vector3 target)
	{
		Vector3 toTarget = target - vehicle.transform.position; // obtain vector to target
		Vector3 desiredForce = toTarget.normalized * vehicle.maxSpeed; // normalize and rescale the vector
		//return new Vector3(0, 0, 0);
		return desiredForce - vehicle.velocity;// properly scales the vector in consideration to its current path
		

	}

	public Vector3 Flee(Vehicle vehicle, Vector3 target, float panicDistance)
	{
		Vector3 toTarget = -(target - vehicle.transform.position); // obtain vector to target
		if (toTarget.magnitude < panicDistance)
		{
			Vector3 desiredForce = toTarget.normalized * vehicle.maxSpeed; // normalize and rescale the vector
			return desiredForce - vehicle.velocity;// properly scales the vector in consideration to its current path

		}
		else
		{
			return new Vector3();
		}

	}

	public Vector3 Arrive(Vehicle vehicle, Vector3 target)
	{
		Vector3 toTarget = target - vehicle.transform.position; // obtain vector to target

		float distance = toTarget.magnitude;

		if (distance < Mathf.Epsilon)  // in order to not divide by zero and make the program go booooom
		{
			return Vector3.zero;
		}


		float speed = Mathf.Min(vehicle.maxSpeed, distance / vehicle.deceleration);

		Vector3 desiredForce = (toTarget / distance) * speed;// optimization of the one above couse square root is heavy to calculate 
		return desiredForce - vehicle.velocity;// properly scales the vector in consideration to its current path


	}
}
