using System;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(Camera))]
public class WaterCameraController : MonoBehaviour
{
	public Camera MainCam;

	private Camera myCam;

	private Transform mainCamTr, myTr;


	private void Awake()
	{
		mainCamTr = MainCam.transform;

		myCam = GetComponent<Camera>();
		myTr = transform;
	}
	private void Update()
	{
		myTr.position = mainCamTr.position;
		myCam.orthographicSize = MainCam.orthographicSize;
	}
}