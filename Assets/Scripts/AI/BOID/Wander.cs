using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wander : SteeringBehaviour
{
	public float jitter;
	public float radius;
	public float distance;

	private Vector3 localTarget;

	public override Vector3 Calculate(Vehicle vehicle)
	{
		localTarget.x += Random.Range(-jitter, jitter);
		localTarget.y += Random.Range(-jitter, jitter);
		localTarget.z += Random.Range(-jitter, jitter);

		localTarget.Normalize();
		localTarget *= radius;

		//Vector3 worldTarget = vehicle.transform.localToWorldMatrix.MultiplyPoint (localTarget);
		Vector3 worldTarget = vehicle.LocaltoWorld(localTarget);

		worldTarget += vehicle.direction * distance;

		return worldTarget - vehicle.transform.position;
	}
}
