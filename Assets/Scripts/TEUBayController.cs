using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TEUBayController : MonoBehaviour {

	public GameObject loadingBay;
	public GameObject unloadingBay;

	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {
		if (GM.containerLoaded) {
			unloadingBay.SetActive (true);
			loadingBay.SetActive (false);
		}
		else if(!GM.containerLoaded){
			loadingBay.SetActive(true);
			unloadingBay.SetActive(false);
		}
	}

	void OnTriggerEnter(Collider other){
		if (other.CompareTag ("Player") && GM.AGVRampage) {
			if (loadingBay.activeSelf) {
				loadingBay.GetComponent<LoadingBayTimer> ().timeLeft = 2.0f;
			} else
				unloadingBay.GetComponent<LoadingBayTimer> ().timeLeft = 2.0f;
		}
	}
}
