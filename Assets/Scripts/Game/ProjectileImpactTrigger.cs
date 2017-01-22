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
		
		switch (other.tag)
		{
		case "Player":
			parentProj.ProcessImpact(ProjectileImpactType.RubberDuck);
			readyForImpact = false;
			break;
		case "RealDuck":
			parentProj.ProcessImpact(ProjectileImpactType.RealDuck);
			readyForImpact = false;
			break;
		case "RockLobstacle":
			parentProj.ProcessImpact(ProjectileImpactType.Rock);
			readyForImpact = false;
			break;
		case "Boat":
			parentProj.ProcessImpact(ProjectileImpactType.Boat);
			readyForImpact = false;
			break;
		case "Mine":
			ExplodingObstacle xo = other.GetComponent<ExplodingObstacle>();
			if (xo.readyToExplode)
			{
				parentProj.ProcessImpact(ProjectileImpactType.Mine);
				readyForImpact = false;
			}
			break;
		}
	}
}
