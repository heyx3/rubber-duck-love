using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreMgr : MonoBehaviour {

    public int score;
    public bool brokenShitIsFixed;

    void Awake()
    {
        DontDestroyOnLoad(transform.gameObject);
    }


}
