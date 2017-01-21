using System;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(MeshRenderer))]
public class Water : MonoBehaviour
{
	public struct Wave
	{
		public float Amplitude, StartTime, Period;
		public Vector2 SourceWorldPos;
		public float Dropoff;

		public Wave(float amplitude, float startTime, float period, float dropoff, Vector2 sourceWorldPos)
		{
			Amplitude = amplitude;
			StartTime = startTime;
			Period = period;
			SourceWorldPos = sourceWorldPos;
			Dropoff = dropoff;
		}
	}


	public static readonly int MaxWaves = 5;


	public MeshRenderer MyMR { get; private set; }

	private List<Wave> waves = new List<Wave>();


	private void Awake()
	{
		MyMR = GetComponent<MeshRenderer>();
	}
	private void Update()
	{

	}
}