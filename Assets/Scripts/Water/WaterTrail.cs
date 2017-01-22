using System;
using System.Collections.Generic;
using UnityEngine;


public class WaterTrail : MonoBehaviour
{
	public float Amplitude = 1.0f,
				 Period = 0.5f,
				 Speed = 50.0f,
				 Lifetime = 1.0f,
				 Dropoff = 20.0f;
	public float MinInterval = 1.0f,
				 MaxInterval = 1.5f;


	private Transform myTr;

	private Vector2 lastPos;

	[SerializeField]
	private float distTillNext = 0.0001f;


	private void Awake()
	{
		myTr = transform;
		lastPos = myTr.position;
	}
	private void Update()
	{
		Vector2 newPos = myTr.position;
		distTillNext -= (newPos - lastPos).magnitude;
		while (distTillNext < 0.0f)
		{
			distTillNext += UnityEngine.Random.Range(MinInterval, MaxInterval);
			Water.Instance.AddWave(new Water.Wave_Circular(Amplitude, Period, Speed,
														   Dropoff, newPos, Lifetime));
		}
		lastPos = newPos;
	}
}