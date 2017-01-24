using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


[RequireComponent(typeof(MakeQuadFitCamera))]
public class QuadSizeHack : MonoBehaviour
{
	public Camera Cam;

	private void Start()
	{
		if (Mathf.Approximately(Cam.aspect, 16.0f / 9.0f))
			GetComponent<MakeQuadFitCamera>().Scale.x = 1.0f;
		else if (Mathf.Approximately(Cam.aspect, 16.0f / 10.0f))
			GetComponent<MakeQuadFitCamera>().Scale.x = 1.1f;
		else
			Debug.LogError("Unexpected aspect ratio");

		Destroy(this);
	}
}