using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimationManager : MonoBehaviour {

    [SerializeField]
    private Animator animator;

    public void OnThrow(PlayerState oldState, PlayerState newState)
    {
        if (newState == PlayerState.Throwing)
        {
            animator.SetTrigger("Throw Trigger");
        }
        else if ( newState == PlayerState.Lose)
        {
                animator.SetTrigger("Lose Trigger");
        }
        else if ( newState == PlayerState.Startup)
        {
            animator.SetTrigger("Startup Trigger");
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
