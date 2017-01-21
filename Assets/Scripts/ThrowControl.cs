﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum PlayerState
{
	Dead = 0,
	Aiming = 1,
	Windup = 2,
	Throwing = 3,
	PostThrow = 4
}

public class ThrowControl : MonoBehaviour
{
	[Header("State Variables")]
	public PlayerState currState = PlayerState.Dead;
	public float currAngle = 0;
	public float currRawFill = 0.0f;
	public float currFill = 0.0f;
	public bool fillIsUp = true;

	[Header("Input State Variables")]
	public bool windupButtonPressed;


	[Header("Objects")]
	public GameObject objectToThrow;
	public Slider arrowSlider;

	[Header("Tuning Variables")]
	public float maxAngle = 90f;
	public float maxDegPerSec = 90;
	public float maxFillPerSec = 1f;

	// Use this for initialization
	void Start ()
	{
		SetState(PlayerState.Aiming);
	}
	
	void SetState(PlayerState newState)
	{
		PlayerState oldState = currState;
		switch (oldState)
		{
		case PlayerState.Dead:
			ExitDead(newState);
			break;
		case PlayerState.Aiming:
			ExitAiming(newState);
			break;
		case PlayerState.Windup:
			ExitWindup(newState);
			break;
		case PlayerState.Throwing:
			ExitThrowing(newState);
			break;
		case PlayerState.PostThrow:
			ExitPostThrow(newState);
			break;			
		}
		switch (newState)
		{
		case PlayerState.Dead:
			EnterDead(oldState);
			break;
		case PlayerState.Aiming:
			EnterAiming(oldState);
			break;
		case PlayerState.Windup:
			EnterWindup(oldState);
			break;
		case PlayerState.Throwing:
			EnterThrowing(oldState);
			break;
		case PlayerState.PostThrow:
			EnterPostThrow(oldState);
			break;			
		}
		currState = newState;
	}

	void EnterDead(PlayerState exitState)
	{

	}

	void ExitDead(PlayerState enterState)
	{

	}

	void EnterAiming(PlayerState exitState)
	{

	}

	void ExitAiming(PlayerState enterState)
	{

	}

	void EnterWindup(PlayerState exitState)
	{
		SetFill(0.0f);
		fillIsUp = true;
	}

	void ExitWindup(PlayerState enterState)
	{
		SetFill(0.0f);
		fillIsUp = true;
	}

	void EnterThrowing(PlayerState exitState)
	{

	}

	void ExitThrowing(PlayerState enterState)
	{

	}

	void EnterPostThrow(PlayerState exitState)
	{

	}

	void ExitPostThrow(PlayerState enterState)
	{

	}

	void ProcessInput()
	{
		windupButtonPressed = Input.GetAxisRaw("Vertical") != 0.0f;
	}

	void SetFill(float rawFill)
	{
		currRawFill = rawFill;
		currFill = Mathf.SmoothStep(0, 1, rawFill);
	}

	void UpdateUI()
	{
		arrowSlider.transform.localRotation = Quaternion.Euler(0, 0, currAngle);
		arrowSlider.value = currFill;
	}
	// Update is called once per frame
	void Update ()
	{
		ProcessInput();

		switch (currState)
		{
		case PlayerState.Dead:
			UpdateDead();
			break;
		case PlayerState.Aiming:
			UpdateAiming();
			break;
		case PlayerState.Windup:
			UpdateWindup();
			break;
		case PlayerState.Throwing:
			UpdateThrowing();
			break;
		case PlayerState.PostThrow:
			UpdateThrowing();
			break;			
		}

		UpdateUI();

	}

	void UpdateDead()
	{

	}

	void UpdateAiming()
	{
		// allow aiming unless windup button is pressed
		if (!windupButtonPressed)
		{
			float newAngle = Mathf.Clamp(currAngle
				+ Input.GetAxis("Horizontal") * -maxDegPerSec * Time.deltaTime,
				-maxAngle, maxAngle);

			currAngle = newAngle;

			//arrowSlider.transform.localRotation = Quaternion.Euler(0, 0, currAngle);
		}
		else
		{
			SetState(PlayerState.Windup);
		}
	}

	void UpdateWindup()
	{
		if (windupButtonPressed)
		{
			float newRawFill = Mathf.Clamp(currRawFill + (fillIsUp ? 1 : -1)
				* maxFillPerSec * Time.deltaTime,0, 1);
			SetFill(newRawFill);

			if ((fillIsUp && currRawFill == 1.0f)
				|| (!fillIsUp && currRawFill == 0.0f))
			{
				fillIsUp = !fillIsUp;
			}
			// arrowSlider.value = currFill;
		}
		else
		{
			SetState(PlayerState.Aiming);
		}
	}

	void UpdateThrowing()
	{

	}

	void UpdatePostThrow()
	{

	}
}
