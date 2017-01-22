using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleToggle : MonoBehaviour {

    [SerializeField]
    private ParticleSystem particles;

    public void OnWin( GameState oldState, GameState newState )
    {
        if (oldState == GameState.Win || newState == GameState.Startup || newState == GameState.Playing)
        {
            particles.gameObject.SetActive(false);
        }
        if (newState == GameState.Win)
        {
            particles.gameObject.SetActive(true);
        }
    }

	// Use this for initialization
	void Start () {
        GameManager.OnGameStateChange += OnWin;
	}
	
    void OnDestroy()
    {
        GameManager.OnGameStateChange -= OnWin;
    }

}
