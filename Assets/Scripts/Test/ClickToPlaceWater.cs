using System;
using System.Collections.Generic;
using UnityEngine;


public class ClickToPlaceWater : MonoBehaviour
{
	public Water TheWater;
	public Camera Cam;

	public float Amplitude = 1.0f,
				 Period = 0.5f,
				 Speed = 10.0f,
				 Dropoff = 10.0f,
				 Lifetime = 4.0f;


	private void Start()
	{
		//TheWater.AddWave(new Water.Wave_Directional(1.0f, 0.5f, Time.time, new Vector2(10.0f, 10.0f)));
	}
	private void Update()
	{
		if (Input.GetMouseButtonDown(0))
		{
			TheWater.AddWave(new Water.Wave_Circular(Amplitude, Period, Speed,
													 Time.time, Dropoff,
													 Cam.ScreenToWorldPoint(Input.mousePosition),
													 Lifetime));
		}
	}
}