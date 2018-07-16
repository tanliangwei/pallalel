using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GarbageCollector : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	void OnTriggerEnter(Collider other){
		if (other.gameObject.CompareTag("Pick Up"))
		{
			Destroy (other.gameObject);
		}

		if (other.gameObject.CompareTag ("Obstacle")) {
			Destroy (other.gameObject);

		}

		if (other.gameObject.CompareTag ("CoinMagnet")) {
			Destroy (other.gameObject);

		}

		if (other.gameObject.CompareTag ("HazeScreen")) {
			Destroy (other.gameObject);

		}

		if (other.gameObject.CompareTag ("OilSpill")) {
			Destroy (other.gameObject);

		}
		if (other.gameObject.CompareTag ("AGVRampage")) {
			Destroy (other.gameObject);

		}
		if (other.gameObject.CompareTag ("Calefare")) {
			Destroy (other.gameObject);

		}
	}
}
