using System;
using System.Collections.Generic;
using UnityEngine;


public class MakeQuadFitCamera : MonoBehaviour
{
	public Vector2 Scale = Vector2.one;

	public Transform Quad;
	public Camera Cam;

	private Transform camTr;



	private void Awake()
	{
		camTr = Cam.transform;
	}
	private void Update()
	{
		Vector3 camPos = camTr.position;
		Quad.position = new Vector3(camPos.x, camPos.y, Quad.position.z);
		Quad.rotation = Quaternion.identity;
		Quad.localScale = new Vector3(2.0f * Cam.orthographicSize * Cam.aspect * Scale.x,
									  2.0f * Cam.orthographicSize * Scale.y,
									  1.0f);
	}
}