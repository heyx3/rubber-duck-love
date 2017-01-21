﻿using System;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(SpriteRenderer))]
public class WaterBobber : MonoBehaviour
{
	public Sprite Sprite_Normal, Sprite_BobLeft, Sprite_BobRight;
	public Vector2 NonRotatedDirection = new Vector2(1.0f, 0.0f);

	public float ZThreshold = 0.25f;

	private Transform tr;
	private SpriteRenderer spr;


	private void Awake()
	{
		NonRotatedDirection.Normalize();
		spr = GetComponent<SpriteRenderer>();
		tr = transform;
	}
	private void Update()
	{
		Vector3 pos3 = tr.position;
		
		var sample = Water.Instance.Sample(new Vector2(pos3.x, pos3.y));

		if (sample.Normal.z > ZThreshold)
			spr.sprite = Sprite_Normal;
		else
		{
			Vector3 newForward3 = Vector3.Cross(sample.Normal, new Vector3(0.0f, 0.0f, 1.0f));
			Vector2 newForward2 = new Vector2(newForward3.x, newForward3.y).normalized;

			if (Vector2.Dot(MyForward, newForward2) > 0.0f)
				MyForward = newForward2;
			else
				MyForward = -newForward2;

			//float angle1 = Vector2.Angle(NonRotatedDirection, newForward2),
			//	  angle2 = Vector2.Angle(NonRotatedDirection, -newForward2);
			//float angle = Math.Min(angle1, angle2);
			//tr.rotation = Quaternion.AngleAxis(angle, new Vector3(0.0f, 0.0f, 1.0f));

			if (Vector2.Dot(MyRight, sample.Normal) > 0.0f)
				spr.sprite = Sprite_BobRight;
			else
				spr.sprite = Sprite_BobLeft;
		}
	}

	private Vector2 MyForward
	{
		get
		{
			var forward3 = tr.right;
			return new Vector2(forward3.x, forward3.y);
		}
		set
		{
			tr.right = new Vector3(value.x, value.y, 0.0f);
		}
	}
	private Vector2 MyRight
	{
		get
		{
			var right3 = -tr.up;
			return new Vector2(right3.x, right3.y);
		}
		set
		{
			tr.up = -new Vector3(value.x, value.y, 0.0f);
		}
	}
}