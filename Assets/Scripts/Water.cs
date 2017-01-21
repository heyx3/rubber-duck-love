using System;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(Renderer))]
public class Water : MonoBehaviour
{
	public struct Wave_Circular
	{
		public float Amplitude, Period, Speed,
					 StartTime, Dropoff;
		public Vector2 SourceWorldPos;

		public float Lifetime;

		public Wave_Circular(float amplitude, float period, float speed,
							 float startTime, float dropoff, Vector2 sourceWorldPos,
							 float lifeTime)
		{
			Amplitude = amplitude;
			Period = period;
			Speed = speed;
			StartTime = startTime;
			Dropoff = dropoff;
			SourceWorldPos = sourceWorldPos;
			Lifetime = lifeTime;
		}
	}

	public struct Wave_Directional
	{
		public float Amplitude, Period, StartTime;
		public Vector2 Velocity;

		public Wave_Directional(float amplitude, float period, float startTime, Vector2 velocity)
		{
			Amplitude = amplitude;
			Period = period;
			StartTime = startTime;
			Velocity = velocity;
		}
	}


	public static readonly int MaxWaves_Circular = 5,
							   MaxWaves_Directional = 5;


	public float WaveFadeTime = 1.0f;
	public float WaveDropoffRate = 3.0f,
				 WaveSharpness = 2.0f;

	public Renderer MyRenderer { get; private set; }

	private List<Wave_Circular> waves_circular = new List<Wave_Circular>();
	private List<Wave_Directional> waves_directional = new List<Wave_Directional>();
	private MaterialPropertyBlock waveArrayData;


	private float GetLifetimeMultiplier(int i)
	{
		float currentT = Time.time;

		float lifeLength = currentT - waves_circular[i].StartTime;
		float timeTillEnd = waves_circular[i].Lifetime - lifeLength;

		return Mathf.Clamp01(Mathf.InverseLerp(0.0f, WaveFadeTime, timeTillEnd));
	}


	public void AddWave(Wave_Circular circularWave)
	{
		while (waves_circular.Count >= MaxWaves_Circular)
		{
			//Remove the oldest wave.
			float currentT = Time.time;
			int oldest = 0;
			float oldestLife = currentT - waves_circular[oldest].StartTime;
			for (int i = 1; i < waves_circular.Count; ++i)
			{
				float tempLife = currentT - waves_circular[i].StartTime;
				if (tempLife > oldestLife)
				{
					oldestLife = tempLife;
					oldest = i;
				}
			}

			waves_circular.RemoveAt(oldest);
		}

		waves_circular.Add(circularWave);
	}
	public void AddWave(Wave_Directional directionalWave)
	{
		while (waves_directional.Count >= MaxWaves_Directional)
		{
			//Remove the oldest wave.
			float currentT = Time.time;
			int oldest = 0;
			float oldestLife = currentT - waves_directional[oldest].StartTime;
			for (int i = 1; i < waves_directional.Count; ++i)
			{
				float tempLife = currentT - waves_directional[i].StartTime;
				if (tempLife > oldestLife)
				{
					oldestLife = tempLife;
					oldest = i;
				}
			}

			waves_directional.RemoveAt(oldest);
		}

		waves_directional.Add(directionalWave);
	}
	public void ClearWaves()
	{
		waves_circular.Clear();
		waves_directional.Clear();
	}

	public void Sample(Vector2 pos, out float height, out Vector3 normal)
	{
		//Note that this exact same code is in the shader.

		height = 0.0f;
		normal = new Vector3(0.0f, 0.0f, 1.0f);

		float currentT = Time.time;
		for (int i = 0; i < waves_circular.Count; ++i)
		{
			var wave = waves_circular[i];

			float timeSinceCreated = wave.StartTime - currentT;

			float dist = Vector2.Distance(wave.SourceWorldPos, pos);
			float heightScale = Mathf.Lerp(0.0f, 1.0f, 1.0f - (dist / wave.Dropoff));
			heightScale = Mathf.Pow(heightScale, WaveDropoffRate);

			float cutoff = wave.Period * wave.Speed * timeSinceCreated;
			cutoff = Math.Max(0.0f, (cutoff - dist) / cutoff);

			float innerVal = (dist / wave.Period) + (-timeSinceCreated * wave.Speed);
			float waveScale = wave.Amplitude * GetLifetimeMultiplier(i) * heightScale * cutoff;

			float heightOffset = Mathf.Sin(innerVal);
			heightOffset = -1.0f + (2.0f * Mathf.Pow(0.5f + (0.5f * heightOffset),
												     WaveSharpness));

			height += waveScale * heightOffset;
		}
		for (int i = 0; i < waves_directional.Count; ++i)
		{
			var wave = waves_directional[i];
			float timeSinceCreated = wave.StartTime - currentT;

			Vector2 flowDir = wave.Velocity;
			float speed = flowDir.magnitude;
			flowDir /= speed;

			float dist = Vector2.Dot(flowDir, pos);

			float innerVal = (dist / wave.Period) + (-timeSinceCreated * speed);
			float heightOffset = Mathf.Sin(innerVal);
			heightOffset = -1.0f + (2.0f * Mathf.Pow(0.5f + (0.5f * heightOffset),
													 WaveSharpness));

			height += wave.Amplitude * heightOffset;
		}
	}

	private void Awake()
	{
		MyRenderer = GetComponent<Renderer>();
		waveArrayData = new MaterialPropertyBlock();
	}
	private void Update()
	{
		//Remove any dead ripples.
		float currentT = Time.time;
		for (int i = 0; i < waves_circular.Count; ++i)
		{
			float lifeLength = currentT - waves_circular[i].StartTime;
			if (lifeLength >= waves_circular[i].Lifetime)
			{
				waves_circular.RemoveAt(i);
				i -= 1;
			}
		}

		//Set material parameters.
		for (int i = 0; i < waves_circular.Count; ++i)
		{
			var wave = waves_circular[i];
			arr_circ_AmpPerSpdStt[i] = new Vector4(wave.Amplitude * GetLifetimeMultiplier(i),
												   wave.Period,
												   wave.Speed, wave.StartTime);
			arr_circ_PosDrop[i] = new Vector4(wave.SourceWorldPos.x, wave.SourceWorldPos.y,
											  wave.Dropoff);
		}
		for (int i = 0; i < waves_directional.Count; ++i)
		{
			var wave = waves_directional[i];
			arr_dir_AmpPerStt[i] = new Vector3(wave.Amplitude, wave.Period, wave.StartTime);
			arr_dir_Vel[i] = wave.Velocity;
		}
		waveArrayData.SetVectorArray("circular_AmpPerSpdStt", arr_circ_AmpPerSpdStt);
		waveArrayData.SetVectorArray("circular_PosDrop", arr_circ_PosDrop);
		MyRenderer.material.SetInt("circular_number", waves_circular.Count);
		waveArrayData.SetVectorArray("directional_AmpPerStt", arr_dir_AmpPerStt);
		waveArrayData.SetVectorArray("directional_Velocity", arr_dir_Vel);
		MyRenderer.material.SetInt("directional_number", waves_directional.Count);
		MyRenderer.SetPropertyBlock(waveArrayData);
		MyRenderer.material.SetFloat("waveDropoffRate", WaveDropoffRate);
		MyRenderer.material.SetFloat("waveSharpness", WaveSharpness);
	}


	//Arrays for storing the values that go into the Material:
	private static Vector4[] arr_circ_AmpPerSpdStt = new Vector4[MaxWaves_Circular],
							 arr_circ_PosDrop = new Vector4[MaxWaves_Circular],
							 arr_dir_AmpPerStt = new Vector4[MaxWaves_Directional],
							 arr_dir_Vel = new Vector4[MaxWaves_Directional];
}