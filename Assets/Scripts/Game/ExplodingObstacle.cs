using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplodingObstacle : MonoBehaviour {

	private SpriteRenderer sRend;
	public GameObject explosion;
	public float explosionDelay = 0.5f;
	public bool readyToExplode = true;

	public float waveSpeed = 20f;
	public float waveAmplitude = 1.0f;
	public float wavePeriod = 0.5f;
	public float waveDropoffRadius = 10f;
	public float waveLifetime = 2.0f;

	public delegate void ExplosionStartEvent();
	public static event ExplosionStartEvent OnExplosionStart;

	void Start()
	{
		sRend = GetComponentInChildren<SpriteRenderer>();
		Reset();
	}

	public void Reset()
	{
		sRend.enabled = true;
		readyToExplode = true;
	}

	void OnTriggerEnter2D (Collider2D other)
	{
		if (other.tag == "Player" && readyToExplode)
		{
			readyToExplode = false;
			StartCoroutine(DelayedExplosion());
		}
	}
	
	void CompleteExplosion()
	{
			GameObject.Instantiate(explosion,transform.position, Quaternion.identity);
			Water.Instance.AddWave(new Water.Wave_Circular(waveAmplitude, wavePeriod, waveSpeed, Time.time, waveDropoffRadius,
														transform.position,
														waveLifetime));
			sRend.enabled = false;
			GameManager.Instance.MineExploded();
	}

	IEnumerator DelayedExplosion ()
	{
		if (OnExplosionStart != null)
			OnExplosionStart();
			
		yield return new WaitForSeconds(explosionDelay);
		CompleteExplosion();
	}
}
