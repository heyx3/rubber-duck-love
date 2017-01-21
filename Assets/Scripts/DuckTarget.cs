﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DuckTarget : MonoBehaviour
{
	void OnTriggerEnter2D(Collider2D other)
	{
		if (other.tag == "Player" && GameManager.Instance.currState == GameState.Playing)
		{
			GameManager.Instance.HitTarget();
		}
	}
}