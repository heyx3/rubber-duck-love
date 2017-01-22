using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplodingObstacle : MonoBehaviour {

	private SpriteRenderer sRend;
	
	public float waveSpeed = 20f;
	public float waveAmplitude = 1.0f;
	public float wavePeriod = 0.5f;
	public float waveDropoffRadius = 10f;
	public float waveLifetime = 2.0f;

	void Start()
	{
		sRend = GetComponentInChildren<SpriteRenderer>();
	}

	void OnTriggerEnter2D (Collider2D other)
	{
		if (other.tag == "Player" && sRend.enabled)
		{
			Debug.Log("boom");
			Water.Instance.AddWave(new Water.Wave_Circular(waveAmplitude, wavePeriod, waveSpeed, Time.time, waveDropoffRadius,
														transform.position,
														waveLifetime));
			sRend.enabled = false;

		}
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
