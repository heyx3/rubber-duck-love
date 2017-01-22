using System;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(WaterReader))]
[RequireComponent(typeof(SpriteTr))]
public class WaterBobber : MonoBehaviour
{
	public Sprite Sprite_Normal, Sprite_BobLeft, Sprite_BobRight,
				  Sprite_BobForward, Sprite_BobBackward;

	public float ZThreshold = 0.25f;

	private SpriteTr tr;
	private SpriteRenderer spr;
	private WaterReader reader;


	private void Awake()
	{
		spr = GetComponentInChildren<SpriteRenderer>();
		tr = GetComponent<SpriteTr>();
		reader = GetComponent<WaterReader>();
	}
	
	private void Update()
	{
		Vector3 pos3 = tr.MyTr.position;

		if (reader.normal.z > ZThreshold)
		{
			spr.sprite = Sprite_Normal;
		}
		else
		{
			float dotForward = Vector2.Dot(reader.normal, tr.Forward),
				  dotRightward = Vector2.Dot(reader.normal, tr.Right);
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
}