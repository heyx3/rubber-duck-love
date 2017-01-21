using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
	public CanvasGroup startPanel;
	public CanvasGroup winPanel;
	public CanvasGroup losePanel;
	public CanvasGroup playPanel;
	public CanvasGroup continuePanel;
	public Text rockCountLabel;


	void OnEnable()
	{
		GameManager.OnGameStateChange += HandleGameStateChange;
		GameManager.OnProjectileEvent += HandleProjectileEvent;
	}

	void OnDisable()
	{
		GameManager.OnGameStateChange -= HandleGameStateChange;
		GameManager.OnProjectileEvent -= HandleProjectileEvent;
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

		UpdateProjectileCount();
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
}
