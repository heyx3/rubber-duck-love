using System;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// Assumes the sprite faces to the right by default.
/// </summary>
public class SpriteTr : MonoBehaviour
{
	public Transform MyTr { get; private set; }
	
	public Vector2 Forward
	{
		get
		{
			var forward3 = MyTr.right;
			return new Vector2(forward3.x, forward3.y);
		}
		set
		{
			MyTr.right = new Vector3(value.x, value.y, 0.0f);
		}
	}
	public Vector2 Right
	{
		get
		{
			var right3 = -MyTr.up;
			return new Vector2(right3.x, right3.y);
		}
		set
		{
			MyTr.up = new Vector3(-value.x, -value.y, 0.0f);
		}
	}

	
	private void Awake()
	{
		MyTr = transform;
	}
}