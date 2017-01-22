using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreMgr : MonoBehaviour {

    public int currentScore;
    public int lastScore;
    public int scoreThisRound;
    public float timeThreshold;
    public float pointsPerSecond;
    public int pointsPerRock;
    public int rockBonus;
    public int timeBonus;
    public int completionBonus;
    public bool brokenShitIsFixed;
    public int highScore;

    public delegate void ScoreManagerWinEvent(int completionBonus, int rockBonus, int timeBonus, int scoreThisRound, int lastScore, int currentScore);
    public static event ScoreManagerWinEvent OnScoreManagerWinEvent;
    public delegate void ScoreManagerLoseEvent(int currentScore, int highScore);
    public static event ScoreManagerLoseEvent OnScoreManagerLoseEvent;

    void Awake()
    {
        DontDestroyOnLoad(transform.gameObject);
        GameManager.OnWin += TallyScore;
        GameManager.OnLose += ResetScore;
    }

    private void ResetScore()
    {
        if (currentScore > highScore)
        {
            highScore = currentScore;
        }
        if (OnScoreManagerLoseEvent != null)
        {
            OnScoreManagerLoseEvent(currentScore, highScore);
        }
        currentScore = 0;
        lastScore = 0;
    }

    private void TallyScore(float timeInPlay, int endingRocks, int startingRocks)
    {
        lastScore = currentScore;
        rockBonus = endingRocks * pointsPerRock;
        timeBonus = (int) Mathf.Clamp(((timeThreshold - timeInPlay) * pointsPerSecond), 0f, (timeThreshold * pointsPerSecond));
        scoreThisRound = completionBonus + timeBonus + rockBonus;
        currentScore = lastScore + scoreThisRound;
        if (OnScoreManagerWinEvent != null)
        {
            OnScoreManagerWinEvent(completionBonus, rockBonus, timeBonus, scoreThisRound, lastScore, currentScore);
        }
    }

}
