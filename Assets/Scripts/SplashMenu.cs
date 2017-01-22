using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SplashMenu : MonoBehaviour
{

    public AudioSource music;
    public bool splashDone;
    public bool musicPlaying;
    public SpriteRenderer splash;

    // Update is called once per frame
    void Start()
    {
        StartCoroutine(WaitAndStart(5.0f));
    }

    void Update()
    {
        if (splashDone)
        {
            if (!musicPlaying)
            {
                splash.enabled = false;
                music.Play();
                musicPlaying = true;
            }
            if (Input.anyKeyDown)
            {
                SceneManager.LoadScene(1);
            }
        }

    }

    private IEnumerator WaitAndStart(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        splashDone = true;
    }
}
