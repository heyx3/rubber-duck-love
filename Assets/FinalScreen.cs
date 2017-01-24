using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class FinalScreen : MonoBehaviour {

    public bool minWaitDone;
    public Animator credits;
    public Canvas canvas;
    public Text reporter;
    public ScoreMgr scoring;

    void Start()
    {
        minWaitDone = false;
        StartCoroutine(WaitAndStart(5.0f));
        GameObject s = GameObject.Find("ScoreManager");
        if (s != null)
        {
            scoring = s.GetComponent<ScoreMgr>();
            if (scoring != null)
            {
                reporter.text = "With " + scoring.currentScore + " points,";
            }
        }
    }

    void Update()
    {
        if (minWaitDone)
        {
            if (Input.anyKeyDown)
            {
                if (credits.GetBool("visible") == true)
                {
                    GameObject t = GameObject.Find("SoundManager");
                    if (t != null)
                    {
                        AudioMgr tAM = t.GetComponent<AudioMgr>();
                        if(tAM != null)
                        {
                            tAM.StopAll();
                        }
                    }
                    SceneManager.LoadScene(0);
                }
                if (credits.GetBool("visible") != true)
                {
                    credits.SetBool("visible", true);
                    canvas.enabled = false;
                    minWaitDone = false;
                    StartCoroutine(WaitAndStart(5.0f));
                }

            }
        }
    }

    private IEnumerator WaitAndStart(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        minWaitDone = true;
    }
}

