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
	public float timeInState;
	public float startDuration = 1f;
	public float winDuration = 2f;
	public float loseDuration = 2f;
	public float winTimeToContinue = 3f;
	public float loseTimeToContinue = 3f;


	public int startingRocks = 10;
	public int currRockInventory = 0;
	public int currRocksInAir = 0;

	private TransformResetter[] resetters;

	// event messages
	public delegate void GameStateChangeEvent(GameState oldState, GameState newState);
	public static event GameStateChangeEvent OnGameStateChange;
	public delegate void GameProjectileEvent(bool isThrowNotDead);
	public static event GameProjectileEvent OnProjectileEvent;

	protected override void Awake()
	{
		base.Awake();
	}

	// Use this for initialization
	void Start ()
	{
		resetters = GameObject.FindObjectsOfType<TransformResetter>();
		Debug.Log("Found " + resetters.Length.ToString() + " resetters");
		SetState(GameState.Startup);
	}
	
	void SetState(GameState newState)
	{
		GameState oldState = currState;
		Debug.Log("Changing Game State from '" + oldState.ToString() + "' to '" + newState.ToString() + "'");
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
		timeInState = 0f;

		if (OnGameStateChange != null)
		{
			OnGameStateChange(oldState, newState);
		}
	}

	void EnterStartup(GameState exitState)
	{
			ResetGameState(exitState);
	}

	void ResetGameState(GameState exitState)
	{
		currRockInventory = startingRocks;

		Water.Instance.ClearWaves();

		if (exitState == GameState.Lose || exitState == GameState.Win)
		{
			for (int i = 0; i < resetters.Length; i++)
			{
				resetters[i].ResetXForm();
			}
		}
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
		timeInState += Time.deltaTime;
		switch (currState)
		{
		case GameState.Startup:
			UpdateStartup();
			break;
		case GameState.Playing:
			UpdatePlaying();
			break;
		case GameState.Paused:
			UpdatePaused();
			break;
		case GameState.Win:
			UpdateWin();
			break;
		case GameState.Lose:
			UpdateLose();
			break;			
		}
	}

	void UpdateStartup()
	{
		if (timeInState >= startDuration)
		{
			SetState(GameState.Playing);
		}
	}

	void UpdatePlaying()
	{
		if (currRockInventory <= 0 && currRocksInAir <= 0)
		{
			SetState(GameState.Lose);
		}
	}

	void UpdatePaused()
	{

	}

	void UpdateWin()
	{
		if (timeInState >= winDuration)
		{
			SetState(GameState.Startup);
		}
	}

	void UpdateLose()
	{
		if (timeInState >= loseDuration)
		{
			SetState(GameState.Startup);
		}
	}

	public void HitTarget()
	{
		Debug.Log("You hit the target!");
		SetState(GameState.Win);
	}

	public void RockThrown()
	{
		currRockInventory--;
		currRocksInAir++;
		if (currRockInventory < 0)
		{
			Debug.Log("WTF WE THREW MORE ROCKS THAN WE OWN!");
		}
		if (OnProjectileEvent != null)
		{
			OnProjectileEvent(true);
		}
	}
	public void RockDead()
	{
		currRocksInAir--;
		if (OnProjectileEvent != null)
		{
			OnProjectileEvent(false);
		}
	}
}
