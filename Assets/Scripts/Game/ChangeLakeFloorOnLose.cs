using System;
using System.Collections.Generic;
using UnityEngine;


public class ChangeLakeFloorOnLose : MonoBehaviour
{
	public Texture2D NewFloor;
	public float Clearness = 0.84f;

	private Texture2D oldFloor;
	private float oldClearness;


	private void Awake()
	{
		GameManager.OnLose += Callback_OnLose;
		GameManager.OnGameStateChange += Callback_NewState;
	}
	private void Start()
	{
		oldFloor = (Texture2D)Water.Instance.MyRenderer.material.GetTexture("_LakeFloorTex");
		oldClearness = Water.Instance.MyRenderer.material.GetFloat("_Clearness");
	}
	private void OnDestroy()
	{
		GameManager.OnLose -= Callback_OnLose;
		GameManager.OnGameStateChange -= Callback_NewState;
	}

	private void Callback_OnLose()
	{
		Water.Instance.MyRenderer.material.SetTexture("_LakeFloorTex", NewFloor);
		Water.Instance.MyRenderer.material.SetFloat("_Clearness", Clearness);
	}
	private void Callback_NewState(GameState oldS, GameState newS)
	{
		if (oldS == GameState.Lose && newS == GameState.Startup)
		{
			Water.Instance.MyRenderer.material.SetTexture("_LakeFloorTex", oldFloor);
			Water.Instance.MyRenderer.material.SetFloat("_Clearness", oldClearness);
		}
	}
}