using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterReader : MonoBehaviour {

	private Transform tr;

	public float height;
	public Vector3 normal;
	public Vector2 wavePushDir;
	public Vector2 simplePushDir;
	public Vector2 tunedPushDir;

	public float waveFactor = 1f;
	public float pushFactor = 0.0f;

	// Use this for initialization
	void Start ()
	{
		tr = transform;
	}

	// Update is called once per frame
	void Update ()
	{
		Water.PositionSample sample = Water.Instance.Sample(tr.position);
		height = sample.Height;
		normal = sample.Normal;
		wavePushDir = sample.WavePushDir;
		simplePushDir = sample.SimplePushDir;

		tunedPushDir = wavePushDir * waveFactor + simplePushDir * pushFactor;

		float oldMag = tunedPushDir.magnitude;
		if (System.Math.Abs(oldMag) > 0.0001f)
		{
			tunedPushDir = tunedPushDir / oldMag * Mathf.Clamp01(oldMag);
		}
	}
}
