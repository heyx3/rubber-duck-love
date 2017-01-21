using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(WaterReader))]

public class WaterRider : MonoBehaviour
{
	public float maxAccel = 15f;
	public float maxSpeed = 5f;
	public float dragFactor = 0.01f;
	private Vector2 currVel = new Vector2();

	private WaterReader reader;

	// Use this for initialization
	void Start ()
	{
		reader = GetComponent<WaterReader>();	
	}
	
	// Update is called once per frame
	void Update ()
	{
		// Vector2 waterDir = reader.tunedPushDir * maxSpeed;
		// Vector2 waterVel = waterDir * Time.deltaTime;
		// transform.position = transform.position + new Vector3(waterVel.x, waterVel.y, 0);
		Vector2 waterAccel = reader.tunedPushDir * maxAccel;
		currVel += waterAccel;
		transform.position = transform.position + new Vector3(currVel.x, currVel.y, 0);
		currVel *= 1 - dragFactor;
	}
}
