using System;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(SpriteTr))]
public class RandomPathFollower : MonoBehaviour
{
	public GraphNode CurrentGoal;
	public float Speed = 10.0f;

    private Vector2 previousGoal;
	private SpriteTr tr;


	private void Start()
	{
		tr = GetComponent<SpriteTr>();

		previousGoal = tr.MyTr.position;
	}
	private void Update()
	{
		Vector2 toPos = (Vector2)CurrentGoal.MyTr.position - (Vector2)tr.MyTr.position;
		float dist = toPos.magnitude;
		toPos /= dist;

		float moveAmnt = Time.deltaTime * Speed;
		if (moveAmnt >= dist)
		{
			tr.MyTr.position += (Vector3)(toPos * dist);
			previousGoal = tr.MyTr.position;

			CurrentGoal =
				CurrentGoal.Connections[UnityEngine.Random.Range(0, CurrentGoal.Connections.Count)];
		}
		else
		{
			tr.MyTr.position += (Vector3)(toPos * moveAmnt);
		}

		tr.Forward = toPos;
	}
}