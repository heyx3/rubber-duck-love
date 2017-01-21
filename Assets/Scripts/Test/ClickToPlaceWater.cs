using System;
using System.Collections.Generic;
using UnityEngine;


public class ClickToPlaceWater : MonoBehaviour
{
	public Water TheWater;
	public Camera Cam;


	private void Start()
	{
		//TheWater.AddWave(new Water.Wave_Directional(1.0f, 0.5f, Time.time, new Vector2(10.0f, 10.0f)));
	}
	private void Update()
	{
		if (Input.GetMouseButtonDown(0))
		{
			TheWater.AddWave(new Water.Wave_Circular(1.0f, 0.5f, 20.0f, Time.time, 10.0f,
													 Cam.ScreenToWorldPoint(Input.mousePosition),
													 4.0f));
		}
	}
}