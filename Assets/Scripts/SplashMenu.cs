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
    public Animator instruction;
    public Animator credits;

    // Update is called once per frame
    void Start()
    {
        StartCoroutine(WaitAndStart(3.0f));
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
                if (instruction.GetBool("visible") == true)
                {
                    if (Input.GetButtonDown("Throw"))
                    {
                        SceneManager.LoadScene(1);
                    }

                }
                if (Input.GetButtonDown("Quack"))
                {
                    bool cred = credits.GetBool("visible");
                    credits.SetBool("visible", !cred);


                }
                if (instruction.GetBool("visible") != true)
                {
                    instruction.SetBool("visible", true);
                }

            }
        }

    }

    private IEnumerator WaitAndStart(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        splashDone = true;
    }
}
