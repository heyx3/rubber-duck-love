using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class TagLooker : MonoBehaviour {

	public string targetTag;
	private Transform tagTransform;
	private Rigidbody2D rb2d;

	// Use this for initialization
	void Start ()
	{
		tagTransform = GameObject.FindGameObjectWithTag(targetTag).transform;
		rb2d = GetComponent<Rigidbody2D>();
		StartRotate();
	}
	
	// Update is called once per frame
	public void StartRotate ()
	{
		if (rb2d != null)
			rb2d.angularVelocity = 0f;
		transform.LookAt(tagTransform);
	}
}
