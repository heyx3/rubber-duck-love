using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ProjectileType
{
	Rock = 0,
	Bread = 1
}

public enum ProjectileState
{
	Windup = 0,
	Airborne = 1,
	Landed = 2,
	Dead = 3
}

public enum ProjectileImpactType
{
	Grass = 0,
	Rock = 1,
	Boat = 2,
	Mine = 3,
	RubberDuck = 4,
	RealDuck = 5
}

public class Projectile : MonoBehaviour
{
	[Header("State Variables")]
	public ProjectileState currState = ProjectileState.Windup;
	public float timeInState;
	public float totalThrowTime;
	public float currStateProgress;
	public Vector3 startPos = new Vector3();
	public Vector3 destPos = new Vector3();
	public Vector3 startScale = new Vector3();
	public Vector3 maxScale = new Vector3();
	
	[Header("Tuning Variables")]
	public float speed = 10f;
	public float minThrowTime = 0.5f;
	public float maxThrowTime = 2f;
	public float maxScaleMod = 1.5f;
	public float sinkTime = 1.0f;
	public ProjectileType type = ProjectileType.Rock;
	
	public float waveSpeed = 20f;
	public float waveAmplitude = 1.0f;
	public float wavePeriod = 0.5f;
	public float waveDropoffRadius = 10f;
	public float waveLifetime = 2.0f;

	public AnimationCurve arc = new AnimationCurve();

	// event messages
	public delegate void ProjectileStateChangeEvent(ProjectileState oldState,
													ProjectileState newState,
													ProjectileType type);
	public static event ProjectileStateChangeEvent OnProjectileStageChange;
	public delegate void ProjectileImpactEvent(ProjectileImpactType impactType, ProjectileType type);
	public static event ProjectileImpactEvent OnProjectileImpact;

	// Use this for initialization
	void Start ()
	{
		//SetState(ProjectileState.Windup);
	}
	
	void SetState(ProjectileState newState)
	{
		ProjectileState oldState = currState;
		//Debug.Log("Changing Projectile State from '" + oldState.ToString() + "' to '" + newState.ToString() + "'");

		switch(oldState)
		{
		case ProjectileState.Windup:
			ExitWindup(newState);
			break;
		case ProjectileState.Airborne:
			ExitAirborne(newState);
			break;
		case ProjectileState.Landed:
			ExitLanded(newState);
			break;
		case ProjectileState.Dead:
			ExitDead(newState);
			break;
		}

		switch(newState)
		{
		case ProjectileState.Windup:
			EnterWindup(oldState);
			break;
		case ProjectileState.Airborne:
			EnterAirborne(oldState);
			break;
		case ProjectileState.Landed:
			EnterLanded(oldState);
			break;
		case ProjectileState.Dead:
			EnterDead(oldState);
			break;
		}

		currState = newState;
		timeInState = 0f;

		if (OnProjectileStageChange != null)
		{
			OnProjectileStageChange(oldState, newState, type);
		}

	}

	void EnterWindup(ProjectileState exitState)
	{

	}

	void ExitWindup(ProjectileState enterState)
	{
		
	}

	void EnterAirborne(ProjectileState exitState)
	{

	}

	void ExitAirborne(ProjectileState enterState)
	{
		
	}

	void EnterLanded(ProjectileState exitState)
	{
		Water water = Water.Instance;
		if (water == null)
		{
			Debug.Log("No water!");
		}
		else
		{
			// Water.Instance.AddWave(new Water.Wave_Circular(1.0f, 0.5f, 20.0f, 10.0f,
			// 											transform.position,
			// 											2.0f));
			Water.Instance.AddWave(new Water.Wave_Circular(waveAmplitude, wavePeriod, waveSpeed,
														   waveDropoffRadius, transform.position,
														   waveLifetime));
		}

	}
	
	void ExitLanded(ProjectileState enterState)
	{
		
	}

	void EnterDead(ProjectileState exitState)
	{
		GameManager.Instance.RockDead();
		GameObject.Destroy(gameObject);
	}

	void ExitDead(ProjectileState enterState)
	{
		
	}

	public void StartThrow(float power)
	{
		//Debug.Log("Setting projectile power to " + power.ToString("f2"));
		totalThrowTime = Mathf.Lerp(minThrowTime, maxThrowTime, power);
		float realPower = totalThrowTime / maxThrowTime;

		// set position/scale factors for lerped animation
		startPos = transform.position;
		destPos = transform.position + transform.up * totalThrowTime * speed;
		startScale = transform.localScale;
		maxScale = startScale * maxScaleMod * realPower;

		SetState(ProjectileState.Airborne);
	}

	// Update is called once per frame
	void Update ()
	{
		timeInState += Time.deltaTime;

		switch(currState)
		{
		case ProjectileState.Windup:
			UpdateWindup();
			break;
		case ProjectileState.Airborne:
			UpdateAirborne();
			break;
		case ProjectileState.Landed:
			UpdateLanded();
			break;
		case ProjectileState.Dead:
			UpdateDead();
			break;
		}
	}

	void UpdateWindup()
	{

	}

	void UpdateAirborne()
	{
		//timeInState = timeInState + Time.deltaTime;
		currStateProgress = timeInState / totalThrowTime;
		float scaleProgress = Mathf.SmoothStep(0,1,currStateProgress);
		float currScaleFactor = 1 - Mathf.Abs(scaleProgress - 0.5f) * 2;
		// float posProgress = (Mathf.SmoothStep(0, 1, 0.5f + currThrowProgress / 2) - 0.5f) * 2;

		Vector3 currPos = Vector3.Lerp(startPos,destPos,scaleProgress);
		Vector3 currScale = Vector3.Lerp(startScale,maxScale, currScaleFactor);
		transform.position = currPos;
		transform.localScale = currScale;


		if (timeInState >= totalThrowTime)
			SetState(ProjectileState.Landed);
	}

	void UpdateLanded()
	{
		currStateProgress = timeInState / sinkTime;
		transform.localScale = Vector3.Lerp(startScale,Vector3.zero,currStateProgress);
		if (timeInState >= sinkTime)
		{
			SetState(ProjectileState.Dead);
		}
	}

	void UpdateDead()
	{

	}

	void OnDrawGizmos()
	{
		if (currState == ProjectileState.Airborne)
		{
			Gizmos.color = Color.blue;
			Gizmos.DrawWireSphere(destPos, 1f);
		}
	}

	void OnTriggerEnter2D(Collider2D other)
	{
		if (other.tag == "ProjectileBoundary" &&
			currState == ProjectileState.Airborne)
		{
			ProcessImpact(ProjectileImpactType.Grass);
			SetState(ProjectileState.Landed);
		}
	}

	public void ProcessImpact(ProjectileImpactType impactType)
	{
		// Debug.Log("PROJECTILE impact type '" + impactType.ToString() + "'");
		if (OnProjectileImpact != null)
		{
			OnProjectileImpact(impactType, type);
		}
	}
}
