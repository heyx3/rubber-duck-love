using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ProjectileState
{
	
}

public class Projectile : MonoBehaviour
{
	[Header("State Variables")]
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
	
	// Update is called once per frame
	void Update ()
	{
		
	}
}
