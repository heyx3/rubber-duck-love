using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class FinalScreen : MonoBehaviour {

    public bool minWaitDone;
    public Animator credits;
    public Canvas canvas;

    void Start()
    {
        minWaitDone = false;
        StartCoroutine(WaitAndStart(5.0f));
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

