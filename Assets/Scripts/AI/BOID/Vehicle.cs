using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(Rigidbody2D))]
public class Vehicle : MonoBehaviour
{
	public Vector3 direction;
	public float maxForces;
	public float maxSpeed;
	public float deceleration;
	public float mass
	{
		get
		{
			return rb.mass;
		}
		set
		{
			rb.mass = value;
		}
	}
	public Vector3 velocity
	{
		get
		{
			return rb.velocity;
		}

	}

	public enum CombineMode
	{
		Weight,
		Priority
	}

	public CombineMode combineMode;

	public List<SteeringBehaviour> behaviours;

	private Rigidbody2D rb;

	void Start()
	{
		rb = GetComponent<Rigidbody2D>();
		SortBehaviour();

	}


	void FixedUpdate()
	{
		Vector2 forces = new Vector2();
		forces = CalculateWithWeight();

		switch (combineMode)
		{
			case CombineMode.Priority:
				forces = CalculateWithPriority();
				break;
			case CombineMode.Weight:
				forces = CalculateWithWeight();
				break;

		}

		rb.AddForce(forces);

		float minDist = 0.1f;
		float currSpeed = rb.velocity.magnitude;

		if (currSpeed > minDist)
		{
			direction = rb.velocity / currSpeed;
		}

		if (currSpeed > maxSpeed)
		{
			rb.velocity = (rb.velocity / currSpeed) * maxSpeed;
		}




	}

	// every time u change the behaviours u have to call this function again so it sorts them out
	// DONT put on every update as it is quite intensive
	public void SortBehaviour()
	{
		behaviours.Sort((a, b) => -a.priority.CompareTo(b.priority)); // This is an anonymos function, the part after the => 
																	  //  is the part that is returned
																	  // - becouse the compare function returns small => big and we want the oposite

	}



	public Vector2 LocaltoWorld(Vector2 local)
	{
		Vector2 pos = transform.position;
		Quaternion rot = Quaternion.LookRotation(direction);
		Vector2 scale = new Vector2(1, 1);
		Matrix4x4 mat = Matrix4x4.TRS(pos, rot, scale);
		return mat.MultiplyPoint(local);
	}

	/// <summary>
	/// This function multiplies each steering behaviours with its weight property
	/// Sums them all together and then truncates the result
	/// Its a costly process and can be hard to adjust
	/// </summary>
	/// <returns></returns>
	private Vector3 CalculateWithWeight()
	{
		Vector3 forces = new Vector3();
		foreach (SteeringBehaviour b in behaviours)
		{
			forces += b.Calculate(this) * b.weight;
			Debug.DrawLine(transform.position, transform.position + (b.Calculate(this) * b.weight),b.debugColor);
		}

		if (forces.magnitude > maxForces)
		{
			forces = forces.normalized * maxForces;
		}


		return forces;

	}


	/// <summary>
	/// Similiar to the weight calculator but takes into consideration the priority of each force
	/// this allows for better controll 
	/// </summary>
	/// <returns></returns>
	private Vector3 CalculateWithPriority()
	{
		Vector3 totalForces = new Vector3();
		float forcesAllowed = maxForces;
		foreach (SteeringBehaviour b in behaviours)
		{
			if (forcesAllowed <= 0)
			{
				break;
			}


			Vector3 force = b.Calculate(this) * b.weight;
			float forceAmount = force.magnitude;
			if (forceAmount > forcesAllowed)
			{
				force = force.normalized * forcesAllowed;
				forcesAllowed = 0;
			}
			else
			{
				forcesAllowed -= forceAmount;
			}
			totalForces += force;
		}
		return totalForces;


	}


	void OnDrawGizmos()
	{

	}
}
