﻿using System;
using System.Collections.Generic;
using UnityEngine;


public class ClickToPlaceWater : MonoBehaviour
{
	public Camera MainCam, WaterCam;

	public float Amplitude = 1.0f,
				 Period = 0.5f,
				 Speed = 10.0f,
				 Dropoff = 10.0f,
				 Lifetime = 4.0f;

	
	private void Update()
	{
		if (Input.GetMouseButtonDown(0))
		{
			Vector2 posT = new Vector2(Input.mousePosition.x / MainCam.pixelWidth,
									   Input.mousePosition.y / MainCam.pixelHeight);
			Vector2 posWaterCam = new Vector2(posT.x * WaterCam.pixelWidth,
											  posT.y * WaterCam.pixelHeight);
			Water.Instance.AddWave(new Water.Wave_Circular(Amplitude, Period, Speed, Dropoff,
														   WaterCam.ScreenToWorldPoint(posWaterCam),
														   Lifetime));
		}
	}
}