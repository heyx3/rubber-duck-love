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
			float waveScale = wave.Amplitude * heightScale * cutoff;

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
			arr_circ_amp[i] = waves_circular[i].Amplitude * GetLifetimeMultiplier(i);
			arr_circ_per[i] = waves_circular[i].Period;
			arr_circ_spd[i] = waves_circular[i].Speed;
			arr_circ_stt[i] = waves_circular[i].StartTime;
			arr_circ_dro[i] = waves_circular[i].Dropoff;
			arr_circ_pos[i] = waves_circular[i].SourceWorldPos;
		}
		for (int i = 0; i < waves_directional.Count; ++i)
		{
			arr_dir_amp[i] = waves_directional[i].Amplitude;
			arr_dir_per[i] = waves_directional[i].Period;
			arr_dir_stt[i] = waves_directional[i].StartTime;
			arr_dir_vel[i] = waves_directional[i].Velocity;
		}
		waveArrayData.SetFloatArray("circular_amplitude", arr_circ_amp);
		waveArrayData.SetFloatArray("circular_period", arr_circ_per);
		waveArrayData.SetFloatArray("circular_speed", arr_circ_spd);
		waveArrayData.SetFloatArray("circular_startTime", arr_circ_stt);
		waveArrayData.SetFloatArray("circular_dropoff", arr_circ_dro);
		waveArrayData.SetVectorArray("circular_startWorldPos", arr_circ_pos);
		MyRenderer.material.SetInt("circular_number", waves_circular.Count);
		waveArrayData.SetFloatArray("directional_amplitude", arr_dir_amp);
		waveArrayData.SetFloatArray("directional_period", arr_dir_per);
		waveArrayData.SetFloatArray("directional_startTime", arr_dir_stt);
		waveArrayData.SetVectorArray("directional_velocity", arr_dir_vel);
		MyRenderer.material.SetInt("directional_number", waves_directional.Count);
		MyRenderer.SetPropertyBlock(waveArrayData);
		MyRenderer.material.SetFloat("waveDropoffRate", WaveDropoffRate);
		MyRenderer.material.SetFloat("waveSharpness", WaveSharpness);
	}


	//Arrays for storing the values that go into the Material:
	private static float[] arr_circ_amp = new float[MaxWaves_Circular],
						   arr_circ_per = new float[MaxWaves_Circular],
						   arr_circ_spd = new float[MaxWaves_Circular],
						   arr_circ_stt = new float[MaxWaves_Circular],
						   arr_circ_dro = new float[MaxWaves_Circular],
						   arr_dir_amp = new float[MaxWaves_Directional],
						   arr_dir_per = new float[MaxWaves_Directional],
						   arr_dir_stt = new float[MaxWaves_Directional];
	private static Vector4[] arr_circ_pos = new Vector4[MaxWaves_Circular],
							 arr_dir_vel = new Vector4[MaxWaves_Directional];
}