using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimationManager : MonoBehaviour {

    [SerializeField]
    private ThrowControl throwControl;
    [SerializeField]
    private Animator animator;

    public void OnThrow(PlayerState oldState, PlayerState newState)
    {
        if (newState == PlayerState.Throwing)
        {
            animator.SetInteger("throw state", 1);
        }
        else
        {
            animator.SetInteger("throw state", 0);
        }
    }

	// Use this for initialization
	void Start () {
        ThrowControl.OnPlayerStateChange += OnThrow;
        if (animator == null)
        {
            animator = gameObject.GetComponent<Animator>();
        }
    }

    void OnDestroy()
    {
        ThrowControl.OnPlayerStateChange -= OnThrow;
    }
	
}
