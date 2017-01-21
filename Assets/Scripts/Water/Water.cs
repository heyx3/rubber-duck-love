using System;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(Renderer))]
public class Water : Singleton<Water>
{
	public struct Wave_Circular
	{
		public float Amplitude, Period, Speed,
					 StartTime, Dropoff;
		public Vector2 SourceWorldPos;

		public float Lifetime;

		public float LifetimeMultiplier { get; private set; }
		public float TimeSinceCutoff { get; private set; }


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

			LifetimeMultiplier = 1.0f;
			TimeSinceCutoff = -10.0f;
		}


		public Wave_Circular AtTime(float currentT, float waveFadeTime)
		{
			Wave_Circular wave = this;

			float lifeLength = currentT - wave.StartTime;
			float timeTillEnd = wave.Lifetime - lifeLength;
			wave.LifetimeMultiplier = Mathf.Clamp01(Mathf.InverseLerp(0.0f, waveFadeTime,
																	  timeTillEnd));

			float cutoffTime = wave.StartTime + wave.Lifetime -
							       (2.0f * wave.Dropoff / wave.Speed);
			wave.TimeSinceCutoff = Math.Max(0.0f, currentT - cutoffTime);

			return wave;
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
	public Vector3 LightDir = new Vector3(1.0f, 1.0f, -1.0f).normalized;

	public AnimationCurve SimplePushDropoff = AnimationCurve.Linear(0.0f, 1.0f, 1.0f, 0.0f);

	public Renderer MyRenderer { get; private set; }

	private List<Wave_Circular> waves_circular = new List<Wave_Circular>();
	private List<Wave_Directional> waves_directional = new List<Wave_Directional>();
	private MaterialPropertyBlock waveArrayData;
	

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

	public struct PositionSample
	{
		public float Height;
		public Vector3 Normal;
		public Vector2 WavePushDir, SimplePushDir;
	}
	public PositionSample Sample(Vector2 pos)
	{
		//Note that this exact same code is in the shader.

		PositionSample sample = new PositionSample();
		sample.Height = 0.0f;
		sample.SimplePushDir = Vector2.zero;
		sample.WavePushDir = Vector2.zero;

		float currentT = Time.time;
		for (int i = 0; i < waves_circular.Count; ++i)
		{
			var wave = waves_circular[i];

			float timeSinceCreated = currentT - wave.StartTime;

			Vector2 toCenter = (wave.SourceWorldPos - pos);
			float dist = toCenter.magnitude;
			toCenter /= dist;

			float heightScale = Mathf.Lerp(0.0f, 1.0f, 1.0f - (dist / wave.Dropoff));
			heightScale = Mathf.Pow(heightScale, WaveDropoffRate);

			float outerCutoff = Math.Min(wave.Dropoff, wave.Period * wave.Speed * timeSinceCreated);
			if (dist > outerCutoff)
				continue;
			outerCutoff = Math.Max(0.0f, (outerCutoff - dist) / outerCutoff);

			float innerCutoff = wave.Period * wave.Speed * wave.TimeSinceCutoff;
			if (dist < innerCutoff)
				continue;
			innerCutoff = 1.0f - Mathf.Clamp01((innerCutoff - dist) / innerCutoff);
			innerCutoff = Mathf.Pow(innerCutoff, 8.0f);

			float cutoff = outerCutoff * innerCutoff;

			float innerVal = (dist / wave.Period) + (-timeSinceCreated * wave.Speed);
			float waveScale = wave.Amplitude * wave.LifetimeMultiplier * heightScale * cutoff;

			float heightOffset = Mathf.Sin(innerVal);
			heightOffset = -1.0f + (2.0f * Mathf.Pow(0.5f + (0.5f * heightOffset),
												     WaveSharpness));

			sample.Height += waveScale * heightOffset;

			float derivative = waveScale * Mathf.Cos(innerVal);
			sample.WavePushDir += toCenter * derivative;
			sample.SimplePushDir -= toCenter * SimplePushDropoff.Evaluate(1.0f - outerCutoff);
		}
		for (int i = 0; i < waves_directional.Count; ++i)
		{
			var wave = waves_directional[i];
			float timeSinceCreated = currentT - wave.StartTime;

			Vector2 flowDir = wave.Velocity;
			float speed = flowDir.magnitude;
			flowDir /= speed;

			float dist = Vector2.Dot(flowDir, pos);

			float innerVal = (dist / wave.Period) + (-timeSinceCreated * speed);
			float heightOffset = Mathf.Sin(innerVal);
			heightOffset = -1.0f + (2.0f * Mathf.Pow(0.5f + (0.5f * heightOffset),
													 WaveSharpness));

			sample.Height += wave.Amplitude * heightOffset;

			float derivative = wave.Amplitude * Mathf.Cos(innerVal);
			sample.WavePushDir += flowDir * derivative;
			sample.SimplePushDir -= flowDir;
		}

		sample.Normal = new Vector3(sample.WavePushDir.x, sample.WavePushDir.y, 0.0001f).normalized;

		return sample;
	}

	protected override void Awake()
	{
		base.Awake();
		MyRenderer = GetComponent<Renderer>();
		waveArrayData = new MaterialPropertyBlock();
	}
	private void Update()
	{
		//Update circular ripples and remove any dead ones.
		float currentT = Time.time;
		for (int i = 0; i < waves_circular.Count; ++i)
		{
			float lifeLength = currentT - waves_circular[i].StartTime;
			if (lifeLength >= waves_circular[i].Lifetime)
			{
				waves_circular.RemoveAt(i);
				i -= 1;
			}
			else
			{
				waves_circular[i] = waves_circular[i].AtTime(currentT, WaveFadeTime);
			}
		}

		//Set material parameters.
		for (int i = 0; i < waves_circular.Count; ++i)
		{
			var wave = waves_circular[i];

			arr_circ_AmpPerSpdStt[i] = new Vector4(wave.Amplitude * wave.LifetimeMultiplier,
												   wave.Period,
												   wave.Speed, wave.StartTime);
			
			arr_circ_PosDropTsc[i] = new Vector4(wave.SourceWorldPos.x, wave.SourceWorldPos.y,
											     wave.Dropoff, wave.TimeSinceCutoff);
		}
		for (int i = 0; i < waves_directional.Count; ++i)
		{
			var wave = waves_directional[i];
			arr_dir_AmpPerStt[i] = new Vector3(wave.Amplitude, wave.Period, wave.StartTime);
			arr_dir_Vel[i] = wave.Velocity;
		}
		waveArrayData.SetVectorArray("circular_AmpPerSpdStt", arr_circ_AmpPerSpdStt);
		waveArrayData.SetVectorArray("circular_PosDropTsc", arr_circ_PosDropTsc);
		MyRenderer.material.SetInt("circular_number", waves_circular.Count);
		waveArrayData.SetVectorArray("directional_AmpPerStt", arr_dir_AmpPerStt);
		waveArrayData.SetVectorArray("directional_Velocity", arr_dir_Vel);
		MyRenderer.material.SetInt("directional_number", waves_directional.Count);
		MyRenderer.SetPropertyBlock(waveArrayData);
		MyRenderer.material.SetFloat("waveDropoffRate", WaveDropoffRate);
		MyRenderer.material.SetFloat("waveSharpness", WaveSharpness);
		MyRenderer.material.SetVector("lightDir", LightDir.normalized);
	}


	//Arrays for storing the values that go into the Material:
	private static Vector4[] arr_circ_AmpPerSpdStt = new Vector4[MaxWaves_Circular],
							 arr_circ_PosDropTsc = new Vector4[MaxWaves_Circular],
							 arr_dir_AmpPerStt = new Vector4[MaxWaves_Directional],
							 arr_dir_Vel = new Vector4[MaxWaves_Directional];
}