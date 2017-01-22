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


	public static readonly int MaxWaves_Circular = 20;


	public float WaveFadeTime = 1.0f;
	public float WaveDropoffRate = 3.0f,
				 WaveSharpness = 2.0f;
	public Vector3 LightDir = new Vector3(1.0f, 1.0f, -1.0f).normalized;

	public AnimationCurve SimplePushDropoff = AnimationCurve.Linear(0.0f, 1.0f, 1.0f, 0.0f);

	public Camera WorldCam;

	public Renderer MyRenderer { get; private set; }

	private List<Wave_Circular> waves_circular = new List<Wave_Circular>();
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
	public void ClearWaves()
	{
		waves_circular.Clear();
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

		Sample(pos, out sample.Height, out sample.SimplePushDir);

		//Get the height at nearby fragments and compute the normal via cross-product.S
		const float epsilon = 0.0001f;
		Vector2 one_zero = new Vector2(pos.x + epsilon, pos.y),
				nOne_zero = new Vector2(pos.x - epsilon, pos.y),
				zero_one = new Vector2(pos.x, pos.y + epsilon),
				zero_nOne = new Vector2(pos.x, pos.y - epsilon);
		Vector3 p_zero_zero = new Vector3(pos.x, pos.y, sample.Height),
				p_one_zero = new Vector3(one_zero.x, one_zero.y, SampleHeightOnly(one_zero)),
				p_nOne_zero = new Vector3(nOne_zero.x, nOne_zero.y, SampleHeightOnly(nOne_zero)),
				p_zero_one = new Vector3(zero_one.x, zero_one.y, SampleHeightOnly(zero_one)),
				p_zero_nOne = new Vector3(zero_nOne.x, zero_nOne.y, SampleHeightOnly(zero_nOne));
		Vector3 norm1 = Vector3.Cross((p_one_zero - p_zero_zero).normalized,
									  (p_zero_one - p_zero_zero).normalized),
				norm2 = Vector3.Cross((p_nOne_zero - p_zero_zero).normalized,
									  (p_zero_nOne - p_zero_zero).normalized),
				normFinal = ((norm1 * -Math.Sign(norm1.z)) +
							 (norm2 * -Math.Sign(norm2.z))).normalized;

		sample.Normal = normFinal;
		sample.WavePushDir = new Vector2(sample.Normal.x, sample.Normal.y);
		sample.Normal = new Vector3(sample.WavePushDir.x, sample.WavePushDir.y, 0.0001f).normalized;

		return sample;
	}
	public void Sample(Vector2 pos, out float height, out Vector2 simplePushDir)
	{
		height = 0.0f;
		simplePushDir = Vector2.zero;

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
			if (dist >= outerCutoff)
				continue;
			outerCutoff = (outerCutoff - dist) / outerCutoff;

			float innerCutoff = wave.Period * wave.Speed * wave.TimeSinceCutoff;
			if (dist <= innerCutoff)
				continue;
			innerCutoff = 1.0f - Mathf.Clamp01((innerCutoff - dist) / innerCutoff);
			innerCutoff = Mathf.Pow(innerCutoff, 8.0f);

			float cutoff = outerCutoff * innerCutoff;

			float innerVal = (dist / wave.Period) + (-timeSinceCreated * wave.Speed);
			float waveScale = wave.Amplitude * wave.LifetimeMultiplier * heightScale * cutoff;

			float heightOffset = Mathf.Sin(innerVal);
			heightOffset = -1.0f + (2.0f * Mathf.Pow(0.5f + (0.5f * heightOffset),
													 WaveSharpness));

			height += waveScale * heightOffset;
			simplePushDir -= toCenter * SimplePushDropoff.Evaluate(1.0f - outerCutoff);
		}
	}
	public float SampleHeightOnly(Vector2 pos)
	{
		float height = 0.0f;

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

			height += waveScale * heightOffset;
		}

		return height;
	}

	protected override void Awake()
	{
		base.Awake();
		MyRenderer = GetComponent<Renderer>();
		waveArrayData = new MaterialPropertyBlock();
	}
	private void Start()
	{
		//AddWave(new Wave_Directional(1.0f, 0.5f, Time.time, Vector2.left * 1.0f));
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
		waveArrayData.SetVectorArray("circular_AmpPerSpdStt", arr_circ_AmpPerSpdStt);
		waveArrayData.SetVectorArray("circular_PosDropTsc", arr_circ_PosDropTsc);
		MyRenderer.material.SetInt("circular_number", waves_circular.Count);
		MyRenderer.SetPropertyBlock(waveArrayData);
		MyRenderer.material.SetFloat("waveDropoffRate", WaveDropoffRate);
		MyRenderer.material.SetFloat("waveSharpness", WaveSharpness);
		MyRenderer.material.SetVector("lightDir", LightDir.normalized);
	}


	//Arrays for storing the values that go into the Material:
	private static Vector4[] arr_circ_AmpPerSpdStt = new Vector4[MaxWaves_Circular],
							 arr_circ_PosDropTsc = new Vector4[MaxWaves_Circular];
}