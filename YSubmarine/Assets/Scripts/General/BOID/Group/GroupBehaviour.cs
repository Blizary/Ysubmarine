using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// this automaticly adds a spherecollider when its used
// otherwise it will atleast provide an error
[RequireComponent(typeof(CircleCollider2D))]

public abstract class GroupBehaviour : SteeringBehaviour
{
	[HideInInspector]
	public List<Vehicle> neighbours = new List<Vehicle>();

	void OnTriggerEnter2D(Collider2D c)
	{
		if (c.gameObject.CompareTag(gameObject.tag))
		{
			Vehicle vehicle = c.GetComponent<Vehicle>();
			neighbours.Add(vehicle);

		}
	}


	void OnTriggerExit2D(Collider2D c)
	{
		if (c.gameObject.CompareTag(gameObject.tag))
		{
			Vehicle vehicle = c.GetComponent<Vehicle>();
			neighbours.Remove(vehicle);

		}
	}



}
