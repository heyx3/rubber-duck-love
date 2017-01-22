using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileImpactTrigger : MonoBehaviour {

	private Projectile parentProj;
	private bool readyForImpact = true;

	void Start()
	{
		parentProj = transform.parent.GetComponent<Projectile>();
	}

	void OnTriggerStay2D(Collider2D other)
	{
		if (parentProj.currState != ProjectileState.Landed || !readyForImpact)
			return;
		
		if (other.tag == "Player")
		{
			parentProj.ProcessImpact(ProjectileImpactType.RubberDuck);
			readyForImpact = false;
		}
		else if (other.tag == "RealDuck" )
		{
			parentProj.ProcessImpact(ProjectileImpactType.RealDuck);
			readyForImpact = false;
		}
	}
}
