using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class TagLooker : MonoBehaviour {

	public string targetTag;
	private float lerptime = 0.45f;
	private Transform tagTransform;
	private Rigidbody2D rb2d;
	private Vector3 startRight;
	private Vector3 targetRight;

	void OnEnable()
	{
		GameManager.OnWin += HandleWin;
	}

	void OnDisable()
	{
		GameManager.OnWin -= HandleWin;
	}

	// Use this for initialization
	void Start ()
	{
		tagTransform = GameObject.FindGameObjectWithTag(targetTag).transform;
		rb2d = GetComponent<Rigidbody2D>();
	}
	
	void HandleWin(float foo, int bar, int baz)
	{
		StartRotate();
	}
	// Update is called once per frame
	public void StartRotate ()
	{
		if (rb2d != null)
			rb2d.angularVelocity = 0f;

		Vector3 direction = (tagTransform.position - transform.position).normalized;
		startRight = transform.right;
		targetRight = direction;
		StartCoroutine(DoRotate());
		//transform.LookAt(tagTransform);
	}
	IEnumerator DoRotate()
	{
		float time = 0f;
		while (time < lerptime)
		{
			time = Mathf.Min(lerptime, time + Time.deltaTime);
			transform.right = Vector3.Lerp(startRight,targetRight, time / lerptime);
			yield return null;
		}
		transform.right = targetRight;
	}
}
