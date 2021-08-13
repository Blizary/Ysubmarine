using UnityEngine;
using System.Collections;

public class AvoidObstacles : SteeringBehaviour
{
	public LayerMask obstaclesLayer;
	public float avoidDistance;
	public float avoidForce;


	public override Vector3 Calculate (Vehicle vehicle)
	{
		Vector3 pos = vehicle.transform.position;
		Vector3 dir = vehicle.direction;

		//checks if there is a obstacle of the given layer close by
		//and alters the force applied
		//limitation it doesnt tell the vehicle to go around the obj
		//simply try to present it from colliding against it
		RaycastHit2D hit = Physics2D.Raycast(pos, dir, avoidDistance, obstaclesLayer);
		if (hit.collider!=null)
		{ 
			Vector3 hitpoint = hit.point;
			Vector3 toHit = hitpoint - vehicle.transform.position;
			float norm = toHit.magnitude / avoidDistance;
			float lerpAmount = 1f - norm;
			float forceAmount = Mathf.Lerp (0,avoidForce,lerpAmount);
			Vector3 repulsion = hit.normal * forceAmount;
			return repulsion - vehicle.velocity;
		}
		return new Vector3();
	}



}
