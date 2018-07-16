using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OilSpillController : MonoBehaviour {
	public GameObject oilParent;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {

	}

	void OnTriggerEnter(Collider other){
		if (other.gameObject.CompareTag ("Player")) {
			GM.oilSpill = false;
			StartCoroutine (destroyOil ());
		}
	}

	IEnumerator destroyOil()
	{
		yield return new WaitForSeconds (1.0f);
		Destroy (oilParent);
	}
}