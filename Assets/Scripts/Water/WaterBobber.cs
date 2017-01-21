using System;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(WaterReader))]
public class WaterBobber : MonoBehaviour
{
	public Sprite Sprite_Normal, Sprite_BobLeft, Sprite_BobRight,
				  Sprite_BobForward, Sprite_BobBackward;

	public float ZThreshold = 0.25f;

	private Transform tr;
	private SpriteRenderer spr;
	private WaterReader reader;


	private void Awake()
	{
		spr = GetComponentInChildren<SpriteRenderer>();
		tr = transform;
		reader = GetComponent<WaterReader>();
	}
	
	private void Update()
	{
		Vector3 pos3 = tr.position;

		if (reader.normal.z > ZThreshold)
		{
			spr.sprite = Sprite_Normal;
		}
		else
		{
			float dotForward = Vector2.Dot(reader.normal, MyForward),
				  dotRightward = Vector2.Dot(reader.normal, MyRight);
			float absDotForward = Math.Abs(dotForward),
				  absDotRightward = Math.Abs(dotRightward);
			if (dotForward > 0.0f && dotForward > absDotRightward)
				spr.sprite = Sprite_BobForward;
			else if (dotForward < 0.0f && -dotForward > absDotRightward)
				spr.sprite = Sprite_BobBackward;
			else if (dotRightward > 0.0f && dotRightward > absDotForward)
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