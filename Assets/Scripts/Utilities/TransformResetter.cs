using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransformResetter : MonoBehaviour {

	private Vector3 startPos;
	private Quaternion startRot;
	private Vector3 startLocalScale;
	private Rigidbody2D rb2d;

	// Use this for initialization
	void Start ()
	{
		startPos = transform.position;
		startRot = transform.rotation;
		startLocalScale = transform.localScale;
		rb2d = GetComponent<Rigidbody2D>();
	}
	
	// Update is called once per frame
	public void ResetXForm ()
	{
		transform.position = startPos;
		transform.rotation = startRot;
		transform.localScale = startLocalScale;
		if (rb2d != null)
		{
			rb2d.velocity = Vector2.zero;
			rb2d.angularVelocity = 0f;
		}

	}
}
