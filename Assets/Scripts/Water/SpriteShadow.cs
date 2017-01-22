using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


[RequireComponent(typeof(SpriteRenderer))]
public class SpriteShadow : MonoBehaviour
{
	public Sprite ShadowSprite;
	public float ShadowDist = 0.1f;
	public bool UseParentSpr = true;


	public Transform MyTr { get; private set; }
	public SpriteRenderer MySpr { get; private set; }

	public SpriteRenderer ParentSpr { get; private set; }


	private void Awake()
	{
		MyTr = transform;

		ParentSpr = MyTr.parent.GetComponent<SpriteRenderer>();

		MySpr = GetComponent<SpriteRenderer>();
	}
	private void LateUpdate()
	{
		if (!ParentSpr.enabled)
		{
			MySpr.enabled = false;
			return;
		}

		Vector2 lightDir = ((Vector2)Water.Instance.LightDir).normalized;
		MyTr.position = MyTr.parent.position - (Vector3)(lightDir * ShadowDist);

		if (UseParentSpr)
			ShadowSprite = ParentSpr.sprite;
		MySpr.sprite = ShadowSprite;
	}
}