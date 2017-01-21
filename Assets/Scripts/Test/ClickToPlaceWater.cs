using System;
using System.Collections.Generic;
using UnityEngine;


public class ClickToPlaceWater : MonoBehaviour
{
	public Water TheWater;
	public Camera Cam;


	private void Update()
	{
		if (Input.GetMouseButtonDown(0))
		{
			TheWater.AddWave(new Water.Wave_Circular(1.0f, 0.5f, 5.0f, Time.time, 10.0f,
													 Cam.ScreenToWorldPoint(Input.mousePosition),
													 10.0f));
		}
	}
}