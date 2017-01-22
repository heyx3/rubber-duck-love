using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BounceTrigger : MonoBehaviour {

	public delegate void BounceEvent(string col_tag, string my_tag);
	public static event BounceEvent OnBounce;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void OnCollisionEnter2D (Collision2D col)
	{
		string col_tag = col.gameObject.tag;
		//Debug.Log("bounce off '" + col_tag + "'");
		if (col_tag == "DuckBoundary" || col_tag == "Boat" || col_tag == "RockLobstacle")
		{
			if (OnBounce != null)
				OnBounce(col_tag, gameObject.tag);
		}

	}
}
