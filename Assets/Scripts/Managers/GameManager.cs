using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum GameState
{
	Startup = 0,
	Playing = 1,
	OutOfRocks = 2,
	Win = 3,
	Lose = 4
}

public class GameManager : Singleton<GameManager>
{

	public GameState currState;
	public float timeInState;
	public float startDuration = 2f;
	public float maxOutOfRocksDuration = 15f;
	public float minWinDuration = 3f;
	public float maxWinDuration = 10f;
	public float minLoseDuration = 3f;
	public float maxLoseDuration = 10f;
	public bool promptedContinue = false;

	public int startingRocks = 10;
	public int currRockInventory = 0;
	public int currRocksInAir = 0;

	private TransformResetter[] resetters;
	//private Rigidbody2D playerRb2d;
	public float loseVelocityThreshold = 0.01f;

	// event messages
	public delegate void GameStateChangeEvent(GameState oldState, GameState newState);
	public static event GameStateChangeEvent OnGameStateChange;
	public delegate void GameProjectileEvent(bool isThrowNotDead);
	public static event GameProjectileEvent OnProjectileEvent;
	public delegate void GameWinEvent(float timeInPlay, int endingRocks, int startingRocks);
	public static event GameWinEvent OnWin;
	public delegate void GameLoseEvent();
	public static event GameLoseEvent OnLose;

	protected override void Awake()
	{
		base.Awake();
	}

	// Use this for initialization
	void Start ()
	{
		resetters = GameObject.FindObjectsOfType<TransformResetter>();
		// WaterRider rider = GameObject.FindGameObjectWithTag("Player").GetComponent<WaterRider>();
		// playerRb2d = rider.GetComponent<Rigidbody2D>();
		// Debug.Log("Found " + resetters.Length.ToString() + " resetters");
		SetState(GameState.Startup);
	}
	
	void SetState(GameState newState)
	{
		GameState oldState = currState;
		// Debug.Log("Changing Game State from '" + oldState.ToString() + "' to '" + newState.ToString() + "'");
		switch (oldState)
		{
		case GameState.Startup:
			ExitStartup(newState);
			break;
		case GameState.Playing:
			ExitPlaying(newState);
			break;
		case GameState.OutOfRocks:
			ExitOutOfRocks(newState);
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
		case GameState.OutOfRocks:
			EnterOutOfRocks(oldState);
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
		promptedContinue = false;

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

	void EnterOutOfRocks(GameState exitState)
	{

	}

	void ExitOutOfRocks(GameState enterState)
	{
		
	}

	void EnterWin(GameState exitState)
	{
		// Debug.Log("WIN EVENT WOO HOO");
		if (OnWin != null)
		{
			OnWin(timeInState,currRockInventory,startingRocks);
		}
	}

	void ExitWin(GameState enterState)
	{
		
	}

	void EnterLose(GameState exitState)
	{
		if (OnLose != null)
		{
			OnLose();
		}
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
		case GameState.OutOfRocks:
			UpdateOutOfRocks();
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
		// if (currRockInventory <= 0 && currRocksInAir <= 0
		// 	&& playerRb2d.velocity.magnitude < loseVelocityThreshold)
		// {
		// 	SetState(GameState.Lose);
		// }
		if (currRockInventory <= 0)
		{
			SetState(GameState.OutOfRocks);
		}
	}

	void UpdateOutOfRocks()
	{
		if (currRocksInAir <= 0
			// && (playerRb2d.velocity.magnitude < loseVelocityThreshold || timeInState >= maxOutOfRocksDuration))
			&& timeInState >= maxOutOfRocksDuration)
		{
			SetState(GameState.Lose);
		}
	}

	void UpdateWin()
	{
        int nextscene = 1 + SceneManager.GetActiveScene().buildIndex;
        if (timeInState >= maxWinDuration)
		{
			SceneManager.LoadScene(nextscene);
		}
		if (timeInState >= minWinDuration && !promptedContinue)
		{
			UIManager.Instance.ShowContinuePanel();
			if (Input.GetButtonDown("Throw"))
			{
                SceneManager.LoadScene(nextscene);
            }
		}
	}

	void UpdateLose()
	{
		if (timeInState >= maxLoseDuration)
		{
			SetState(GameState.Startup);
		}
		if (timeInState >= minLoseDuration && !promptedContinue)
		{
			UIManager.Instance.ShowContinuePanel();
			if (Input.GetButtonDown("Throw"))
			{
				SetState(GameState.Startup);
			}
		}
	}

	public void HitTarget()
	{
		// Debug.Log("You hit the target!");
		SetState(GameState.Win);
	}

	public void MineExploded()
	{
		currRockInventory = Mathf.Max(0, currRockInventory - 1);
		UIManager.Instance.ExplosionResponse();
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
