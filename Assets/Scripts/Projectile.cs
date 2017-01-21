using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ProjectileState
{
	Windup = 0,
	Airborne = 1,
	Landed = 2
}

public class Projectile : MonoBehaviour
{
	[Header("State Variables")]
	public ProjectileState currState = ProjectileState.Windup;
	public float currTime;
	public float totalThrowTime;
	
	[Header("Tuning Variables")]
	public float speed = 10f;
	public float maxThrowTime = 2f;
	
	public AnimationCurve arc = new AnimationCurve();


	// Use this for initialization
	void Start ()
	{
		
	}
	
	public void StartThrow(float power)
	{
		Debug.Log("Setting projectile power to " + power.ToString("f2"));
		totalThrowTime = power * maxThrowTime;
		currTime = 0f;
		currState = ProjectileState.Airborne;
	}

	// Update is called once per frame
	void Update ()
	{
		switch(currState)
		{
		case ProjectileState.Airborne:
			UpdateAirborne();
			break;
		}
	}

	void UpdateAirborne()
	{
		transform.Translate(transform.up * speed * Time.deltaTime);
		currTime = currTime + Time.deltaTime;
		if (currTime >= totalThrowTime)
			currState = ProjectileState.Landed;
	}
}
