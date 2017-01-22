using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public class GraphNode : MonoBehaviour
{
	public List<GraphNode> Connections;

	public Transform MyTr { get; private set; }


	private void Awake()
	{
		MyTr = transform;
		foreach (GraphNode node in Connections)
			if (!node.Connections.Contains(this))
				node.Connections.Add(this);
	}
	private void OnDrawGizmos()
	{
		Gizmos.color = Color.white;
		Vector3 myPos = transform.position;

		Gizmos.DrawSphere(myPos, 1.0f);

		foreach (GraphNode node in Connections)
			Gizmos.DrawLine(myPos, node.transform.position);
	}
}