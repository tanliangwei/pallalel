using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeDestroyer : MonoBehaviour {

	public float LifeTime = 10f;
	// Use this for initialization
	void Start () {
		Invoke ("DestroyObject", LifeTime);
		
	}
	

	void DestroyObject()
	{
		Destroy (gameObject);
	}
}
