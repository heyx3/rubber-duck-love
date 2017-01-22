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
	}
	
	// Update is called once per frame
	void HandleGameStateChange (GameState oldState, GameState newState)
	{
		startPanel.alpha = newState == GameState.Startup ? 1 : 0;
		winPanel.alpha = newState == GameState.Win ? 1 : 0;
		losePanel.alpha = newState == GameState.Lose ? 1 : 0;
		playPanel.alpha = (newState == GameState.Playing || newState == GameState.Win) ? 1 : 0;
		continuePanel.alpha = 0f;

		UpdateProjectileCount();

		if (oldState == GameState.Win)
		{
			StopAllCoroutines();
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
