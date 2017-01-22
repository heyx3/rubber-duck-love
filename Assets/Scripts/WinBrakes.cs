using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]

public class WinBrakes : MonoBehaviour
{
	public float winDrag = 5f;
	public float winAngularDrag = 5f;
	private float startDrag;
	private float startAngularDrag;
	private Rigidbody2D rb2d;

	void OnEnable()
	{
		GameManager.OnGameStateChange += HandleGameStateChange;
	}

	void OnDisable()
	{
		GameManager.OnGameStateChange -= HandleGameStateChange;
	}

	// Use this for initialization
	void Start ()
	{
		rb2d = GetComponent<Rigidbody2D>();
		startDrag = rb2d.drag;
		startAngularDrag = rb2d.angularDrag;
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	void HandleGameStateChange(GameState oldState, GameState newState)
	{
		if (newState == GameState.Win)
		{
			rb2d.drag = winDrag;
			rb2d.angularDrag = winAngularDrag;
		}
		else if (newState == GameState.Startup)
		{
			rb2d.drag = startDrag;
			rb2d.angularDrag = startAngularDrag;
		}
	}
}
