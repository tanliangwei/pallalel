using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathDestroyCollider : MonoBehaviour {

	GameObject path;

	void OnTriggerEnter(Collider other)
	{
		path = this.gameObject;
		if (other.CompareTag("Player"))
			Destroy (path.transform.root.gameObject);
	}

}
