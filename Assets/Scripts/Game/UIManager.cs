using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager  : Singleton<UIManager>
{
	public CanvasGroup startPanel;
	public CanvasGroup winPanel;
	public CanvasGroup losePanel;
	public CanvasGroup playPanel;
	public CanvasGroup continuePanel;
	public CanvasGroup explosionPanel;
	public CanvasGroup outOfRocksPanel;

	public Text outofRocksText;
	public Text rockCountLabel;

	public Text levelPointsText;
	public Text timeLabelText;
	public Text timePointsText;
	public Text rocksLabelText;
	public Text rocksPointsText;
	public Text totalAwardText;
	public Text yourScoreText;
	private float startScore;
	private float targetScore;
	private float currScore;
	private Vector3 shakeStartPos;


	void OnEnable()
	{
		GameManager.OnGameStateChange += HandleGameStateChange;
		GameManager.OnProjectileEvent += HandleProjectileEvent;
		GameManager.OnWin += HandleGameManagerWin;
		ScoreMgr.OnScoreManagerWinEvent += HandleScoreManagerWin;
	}

	void OnDisable()
	{
		GameManager.OnGameStateChange -= HandleGameStateChange;
		GameManager.OnProjectileEvent -= HandleProjectileEvent;
		GameManager.OnWin -= HandleGameManagerWin;
		ScoreMgr.OnScoreManagerWinEvent -= HandleScoreManagerWin;
	}

	// Use this for initialization
	void Start ()
	{
		startPanel.alpha = 1;
		winPanel.alpha = 0;
		losePanel.alpha = 0;
		RectTransform rt = explosionPanel.transform as RectTransform;
		shakeStartPos = rt.anchoredPosition3D;
	}
	
	// Update is called once per frame
	void HandleGameStateChange (GameState oldState, GameState newState)
	{
		startPanel.alpha = newState == GameState.Startup ? 1 : 0;
		winPanel.alpha = newState == GameState.Win ? 1 : 0;
		losePanel.alpha = newState == GameState.Lose ? 1 : 0;
		playPanel.alpha = (newState == GameState.Playing || newState == GameState.Win) ? 1 : 0;
		outOfRocksPanel.alpha = (newState == GameState.OutOfRocks) ? 1 : 0;
		continuePanel.alpha = 0f;
		explosionPanel.alpha = 0f;

		UpdateProjectileCount();

		// if (oldState == GameState.Win)
		// {
			StopAllCoroutines();
		// }
	}

	void Update()
	{
		if (GameManager.Instance.currState == GameState.OutOfRocks)
			UpdateOutOfRocks();
	}
	void UpdateOutOfRocks()
	{
		float timeLeft = Mathf.Max(0, GameManager.Instance.maxOutOfRocksDuration - GameManager.Instance.timeInState);
		outofRocksText.text = "OUT OF ROCKS" +
							  ((timeLeft >= 11.0f) ? "" : ("\n" + timeLeft.ToString("n0") + " sec"));


		if (timeLeft >= 11.0f)
		{
		}
	}

	void UpdateProjectileCount()
	{
		// Debug.Log("ROCKS: " + GameManager.Instance.currRockInventory.ToString() + " held, "
		// 		  + GameManager.Instance.currRocksInAir.ToString() + "in air!");
		rockCountLabel.text = "ROCKS:" + GameManager.Instance.currRockInventory.ToString();
	}
	void HandleProjectileEvent (bool isThrowNotDead)
	{
		UpdateProjectileCount();
	}

	public void ShowContinuePanel()
	{
		continuePanel.alpha = 1;
	}

	void HandleGameManagerWin(float time, int rocksLeft, int rockStarted)
	{
		timeLabelText.text = "TIME: " + time.ToString("f1") + " sec.";
		rocksLabelText.text = "ROCKS LEFT: " + rocksLeft.ToString();

	}

	void HandleScoreManagerWin(int completionBonus, int rockBonus, int timeBonus,
							   int scoreThisRound, int lastScore, int currentScore)
	{
		Debug.Log("Got ScoreManager Win!");
		levelPointsText.text = "+ " + completionBonus.ToString("n0");
		rocksPointsText.text = "+ " + rockBonus.ToString("n0");
		timePointsText.text = "+ " + timeBonus.ToString("n0");
		totalAwardText.text = "+ " + scoreThisRound.ToString("n0");
		yourScoreText.text = lastScore.ToString("n0");
		startScore = lastScore;
		targetScore = currentScore;
		StartCoroutine(DoScoreRollup());
	}

	public void ExplosionResponse()
	{
		UpdateProjectileCount();
		StartCoroutine(DoExplosion());
	}

	IEnumerator DoExplosion()
	{
		explosionPanel.alpha = 1.0f;
		float maxMove = 25f;
		//Vector3 maxPos = rt.anchoredPosition3D + new Vector2(10,10,10);
		float time = 0f;
		float maxTime = 1f;
		RectTransform rt = explosionPanel.transform as RectTransform;
		while (time < maxTime)
		{
			time = Mathf.Min(1.0f, time + Time.deltaTime);
			float lerpFactor = time / maxTime;
			Vector3 shakeVec = new Vector3(Random.value * maxMove, Random.value * maxMove, 0f) * (1 - lerpFactor);
			explosionPanel.alpha = 1.0f - lerpFactor;
			rt.anchoredPosition3D = shakeVec;
			yield return null;
		}
		explosionPanel.alpha = 0f;
		rt.anchoredPosition3D = shakeStartPos;



	}

	IEnumerator DoScoreRollup()
	{
		float lerpTime = 0;
		float maxLerpTime = 1.0f;

		while (lerpTime < maxLerpTime)
		{
			lerpTime = Mathf.Min(maxLerpTime, lerpTime + Time.deltaTime);
			currScore = Mathf.Lerp(startScore, targetScore, lerpTime / maxLerpTime);
			yourScoreText.text = currScore.ToString("n0");
			yield return null;
		}
	}

}
