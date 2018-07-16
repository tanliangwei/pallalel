using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathSpawnCollider : MonoBehaviour {

	public GameObject path;
	public Transform colliderTransform;
	private Vector3 currentPosition;

	void Start()
	{
		currentPosition = colliderTransform.position;
	}


	void OnTriggerEnter(Collider other)
	{
		if (other.gameObject.tag == "Player") {
			currentPosition.z += 90f;
			currentPosition.y = 0;
			Instantiate (path, currentPosition,colliderTransform.rotation);
		}
	}
			
}
