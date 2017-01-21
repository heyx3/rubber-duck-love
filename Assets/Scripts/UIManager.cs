using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
	public CanvasGroup startPanel;
	public CanvasGroup winPanel;
	public CanvasGroup losePanel;

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
	}
}
