using UnityEngine;
using System;
using System.Collections;

public class MonoBehaviourInstance<T> : MonoBehaviour where T : MonoBehaviourInstance<T>
{
	private static T _instance; 
	public static T Instance {
		get {
			if (_instance == null) {
				Type type = typeof(T);
				_instance = UnityEngine.Object.FindObjectOfType(type) as T;
			}
			return _instance;
		}
	}

	public static T instance {
		get {
			return Instance;
		}
	}
	public static T inst {
		get {
			return Instance;
		}
	}

	protected virtual void _Awake() {}
	protected virtual void _OnDestroy() {}

	void Awake()
	{
		if (_instance == null)
		{
			_instance = this as T;
		}
		else if (_instance != this)
		{
			_instance = this as T;
			return;
		}
		_Awake();
	}

	void OnDestroy()
	{
		if (_instance == this)
		{
			_instance = null;
			_OnDestroy();
		}
	}
}