using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FishBrain : MonoBehaviour
{
	private Cohesion cohesion;
	private Separation separation;
	private Alignment alignment;
	private ConstantSpeed constantSpeed;
	private Transform fishTransform;
	private bool playerInPath;


	void Awake()
	{
		cohesion = GetComponent<Cohesion>();
		separation = GetComponent<Separation>();
		alignment = GetComponent<Alignment>();
		constantSpeed = GetComponent<ConstantSpeed>();
		fishTransform = GetComponent<Transform>();
	}


	//public string boidTag;

	// Use this for initialization
	void Start()
	{


		//tag = boidTag;


	}

	// Update is called once per frame
	void Update()
	{
		float rotSpeed = 3 * Time.deltaTime;
		transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(GetComponent<Vehicle>().direction), rotSpeed);

		//Debug.Log (GetComponent<Rigidbody> ().velocity);
	}


	void OnTriggerEnter(Collider target)
	{
		if (target.CompareTag("Player"))
		{
			playerInPath = true;
		}
	}

	void OnTriggerExit(Collider target)
	{
		if (target.CompareTag("Player"))
		{
			playerInPath = false;
		}
	}
}
