using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GameState
{
	Startup = 0,
	Playing = 1,
	Paused = 2,
	Win = 3,
	Lose = 4
}

public class GameManager : Singleton<GameManager>
{

	public GameState currState;

	// event messages
	public delegate void GameStateChangeEvent(GameState oldState, GameState newState);
	public static event GameStateChangeEvent OnGameStateChange;

	protected override void Awake()
	{
		base.Awake();
	}

	// Use this for initialization
	void Start ()
	{
		SetState(GameState.Startup);
	}
	
	void SetState(GameState newState)
	{
		GameState oldState = currState;
		switch (oldState)
		{
		case GameState.Startup:
			ExitStartup(newState);
			break;
		case GameState.Playing:
			ExitPlaying(newState);
			break;
		case GameState.Paused:
			ExitPaused(newState);
			break;
		case GameState.Win:
			ExitWin(newState);
			break;
		case GameState.Lose:
			ExitLose(newState);
			break;			
		}
		switch (newState)
		{
		case GameState.Startup:
			EnterStartup(oldState);
			break;
		case GameState.Playing:
			EnterPlaying(oldState);
			break;
		case GameState.Paused:
			EnterPaused(oldState);
			break;
		case GameState.Win:
			EnterWin(oldState);
			break;
		case GameState.Lose:
			ExitWin(oldState);
			break;			
		}
		currState = newState;

		if (OnGameStateChange != null)
		{
			OnGameStateChange(oldState, newState);
		}
	}

	void EnterStartup(GameState exitState)
	{

	}

	void ExitStartup(GameState enterState)
	{
		
	}

	void EnterPlaying(GameState exitState)
	{

	}

	void ExitPlaying(GameState enterState)
	{
		
	}

	void EnterPaused(GameState exitState)
	{

	}

	void ExitPaused(GameState enterState)
	{
		
	}

	void EnterWin(GameState exitState)
	{

	}

	void ExitWin(GameState enterState)
	{
		
	}

	void EnterLose(GameState exitState)
	{

	}

	void ExitLose(GameState enterState)
	{
		
	}

	// Update is called once per frame
	void Update ()
	{
		
	}

	void UpdateStartup()
	{

	}

	void UpdatePlaying()
	{

	}

	void UpdatePaused()
	{

	}

	void UpdateWin()
	{

	}

	void UpdateLose()
	{

	}
}
