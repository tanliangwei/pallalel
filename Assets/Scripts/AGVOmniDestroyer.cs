using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AGVOmniDestroyer : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	void OnTriggerStay(Collider other){

		if (GM.AGVRampage) {
			if (other.gameObject.CompareTag ("Obstacle")) {
				Destroy (other.gameObject);
			}

			if (other.gameObject.CompareTag ("HazeScreen")) {
				Destroy (other.gameObject);
			}

			if (other.gameObject.CompareTag ("OilSpill")) {
				Destroy (other.gameObject);
			}
			if(other.gameObject.CompareTag("ShipBonanza"))
				Destroy(other.gameObject);
		}
	}

	void OnTriggerEnter(Collider other){
		if (GM.AGVRampage) {
			if (other.gameObject.CompareTag ("AGVRampage")) {
				Destroy (other.gameObject);
			}
		}
	}
}
