using System;
using UnityEngine;


public class Singleton<T> : MonoBehaviour
	where T : Singleton<T>
{
	public static T Instance { get; private set; }

	protected virtual void Awake()
	{
		if (Instance != null)
		{
			string myPath = "";
			Transform tr = transform;
			while (tr != null)
			{
				myPath = myPath + "/" + tr.gameObject.name;
				tr = tr.parent;
			}

			string theirPath = "";
			tr = Instance.transform;
			while (tr != null)
			{
				theirPath = theirPath + "/" + tr.gameObject.name;
				tr = tr.parent;
			}

			Debug.LogError("More than one instance of " + typeof(T).ToString() + ": \"" +
						       myPath + "\" and \"" + theirPath + "\"");
		}
		UnityEngine.Assertions.Assert.IsNull(Instance, gameObject.name);
		Instance = (T)this;
	}
	protected virtual void OnDestroy()
	{
		UnityEngine.Assertions.Assert.AreEqual(this, Instance, gameObject.name);
		Instance = null;
	}
}