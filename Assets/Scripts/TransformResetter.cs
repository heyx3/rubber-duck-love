using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransformResetter : MonoBehaviour {

	private Vector3 startPos;
	private Quaternion startRot;
	private Vector3 startLocalScale;

	// Use this for initialization
	void Start ()
	{
		startPos = transform.position;
		startRot = transform.rotation;
		startLocalScale = transform.localScale;
	}
	
	// Update is called once per frame
	public void ResetXForm ()
	{
		transform.position = startPos;
		transform.rotation = startRot;
		transform.localScale = startLocalScale;
	}
}
