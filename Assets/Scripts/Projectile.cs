using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ProjectileState
{
	Windup = 0,
	Airborne = 1,
	Landed = 2,
	Dead = 3
}

public class Projectile : MonoBehaviour
{
	[Header("State Variables")]
	public ProjectileState currState = ProjectileState.Windup;
	public float currTime;
	public float totalThrowTime;
	public float currThrowProgress;
	public Vector3 startPos = new Vector3();
	public Vector3 destPos = new Vector3();
	public Vector3 startScale = new Vector3();
	public Vector3 maxScale = new Vector3();
	
	[Header("Tuning Variables")]
	public float speed = 10f;
	public float minThrowTime = 0.5f;
	public float maxThrowTime = 2f;
	public float maxScaleMod = 1.5f;
	
	public AnimationCurve arc = new AnimationCurve();


	// Use this for initialization
	void Start ()
	{
		
	}
	
	public void StartThrow(float power)
	{
		//Debug.Log("Setting projectile power to " + power.ToString("f2"));
		totalThrowTime = Mathf.Lerp(minThrowTime, maxThrowTime, power);
		float realPower = totalThrowTime / maxThrowTime;
		currTime = 0f;
		currState = ProjectileState.Airborne;
		startPos = transform.position;
		destPos = transform.position + transform.up * totalThrowTime * speed;
		startScale = transform.localScale;
		maxScale = startScale * maxScaleMod * realPower;
	}

	// Update is called once per frame
	void Update ()
	{
		switch(currState)
		{
		case ProjectileState.Airborne:
			UpdateAirborne();
			break;
		}
	}

	void UpdateAirborne()
	{
		currTime = currTime + Time.deltaTime;
		currThrowProgress = currTime / totalThrowTime;
		float scaleProgress = Mathf.SmoothStep(0,1,currThrowProgress);
		float currScaleFactor = 1 - Mathf.Abs(scaleProgress - 0.5f) * 2;
		float posProgress = (Mathf.SmoothStep(0, 1, 0.5f + currThrowProgress / 2) - 0.5f) * 2;
		//Vector3 currPos = Vector3.Lerp(startPos,destPos,posProgress);
		Vector3 currPos = Vector3.Lerp(startPos,destPos,scaleProgress);
		Vector3 currScale = Vector3.Lerp(startScale,maxScale, currScaleFactor);
		transform.position = currPos;
		transform.localScale = currScale;
		//transform.Translate(Vector3.up * speed * Time.deltaTime);

		if (currTime >= totalThrowTime)
			currState = ProjectileState.Landed;
	}

	void OnDrawGizmos()
	{
		if (currState == ProjectileState.Airborne)
		{
			Gizmos.color = Color.blue;
			Gizmos.DrawWireSphere(destPos, 1f);
		}
	}
}
