using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public class WinCelebration : MonoBehaviour
{
	public Rect RandomRippleRegion = new Rect(0.0f, 0.0f, 50.0f, 50.0f);
	public float MinAmplitude = 1.0f,
				 MaxAmplitude = 5.0f,
				 MinPeriod = 0.5f,
				 MaxPeriod = 1.0f,
				 MinSpeed = 15.0f,
				 MaxSpeed = 30.0f,
				 MinDropoff = 5.0f,
				 MaxDropoff = 20.0f,
				 MinLifetime = 1.5f,
				 MaxLifetime = 3.0f,
				 MinInterval = 0.25f,
				 MaxInterval = 0.75f,
				 AngleShiftSpeed = 1.0f;
	public Vector3 ColorShiftSpeed = Vector3.one;
	public Color MinColor = new Color(1.0f, 0.0f, 1.0f),
				 MaxColor = new Color(0.5f, 0.0f, 1.0f);

	private bool isRunning = false;


	private float Rand(float min, float max) { return UnityEngine.Random.Range(min, max); }

	private void Awake()
	{
		GameManager.OnWin += Callback_OnWin;
	}
	private void OnDestroy()
	{
		GameManager.OnWin -= Callback_OnWin;
	}
	private void OnDrawGizmos()
	{
		Gizmos.color = Color.green;
		Gizmos.DrawWireCube(RandomRippleRegion.center, RandomRippleRegion.size);
	}
	private void Update()
	{
		if (!isRunning)
			return;

		//Mess with the light's horizontal direction.

		Water.Instance.LightDir.z = Water.Instance.LightDir.normalized.z;
		//Did some algebraaaaaaaaaaaaa
		float xyLen = Mathf.Sqrt(1.0f - (Water.Instance.LightDir.z * Water.Instance.LightDir.z));

		float angle = Mathf.Atan2(Water.Instance.LightDir.y, Water.Instance.LightDir.x);
		angle += AngleShiftSpeed * Time.deltaTime;

		Water.Instance.LightDir.x = Mathf.Cos(angle) * xyLen;
		Water.Instance.LightDir.y = Mathf.Sin(angle) * xyLen;


		//Mess with the light's color.
		Vector3 t = new Vector3(MinColor.r == MaxColor.r ?
									0.5f :
									Mathf.InverseLerp(MinColor.r, MaxColor.r, Water.Instance.LightColor.r),
								MinColor.g == MaxColor.g ?
									0.5f :
									Mathf.InverseLerp(MinColor.g, MaxColor.g, Water.Instance.LightColor.g),
								MinColor.b == MaxColor.b ?
									0.5f :
									Mathf.InverseLerp(MinColor.b, MaxColor.b, Water.Instance.LightColor.b));
		Vector3 tChange = ColorShiftSpeed * Time.deltaTime;
		t = new Vector3(Mathf.PingPong(t.x + tChange.x, 1.0f),
						Mathf.PingPong(t.y + tChange.y, 1.0f),
						Mathf.PingPong(t.z + tChange.z, 1.0f));
		Water.Instance.LightColor = new Color(Mathf.Lerp(MinColor.r, MaxColor.r, t.x),
											  Mathf.Lerp(MinColor.g, MaxColor.g, t.y),
											  Mathf.Lerp(MinColor.b, MaxColor.b, t.z),
											  1.0f);
	}

	public void Callback_OnWin(float timeInPlay, int endingRocks, int startingRocks)
	{
		StartCoroutine(GameWinCoroutine());
	}
	
	private System.Collections.IEnumerator GameWinCoroutine()
	{
		isRunning = true;
		while (true)
		{
			yield return new WaitForSeconds(UnityEngine.Random.Range(MinInterval, MaxInterval));

			//Make a wave.
			Water.Instance.AddWave(new Water.Wave_Circular(Rand(MinAmplitude, MaxAmplitude),
														   Rand(MinPeriod, MaxPeriod),
														   Rand(MinSpeed, MaxSpeed),
														   Rand(MinDropoff, MaxDropoff),
														   new Vector2(Rand(RandomRippleRegion.xMin,
																			RandomRippleRegion.xMax),
																	   Rand(RandomRippleRegion.yMin,
																			RandomRippleRegion.yMax)),
														   Rand(MinLifetime, MaxLifetime)));

			//Randomize the light's hue.
			Water.Instance.LightColor = Color.Lerp(MinColor, MaxColor, Rand(0.0f, 1.0f));
		}
	}
}