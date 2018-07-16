using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadingBayTimer : MonoBehaviour {

	public int maxTime = 10;
	public int minTime = 5;

	public TextMesh timeDisp;
	private float timeLeft;
	private Collider scoreZone;

	// Use this for initialization
	void Start () {
		timeLeft = Random.Range (minTime, maxTime);
		timeDisp.text =((int) timeLeft).ToString();
		scoreZone = this.GetComponent<Collider> ();
		scoreZone.enabled = false;
	}

	// Update is called once per frame
	void Update () {
		if (GM.containerLoaded) {
			if (this.gameObject.CompareTag ("UnloadingBay")) {
				this.gameObject.SetActive(true);
			} else
				this.gameObject.SetActive(false);
		}
		else if(!GM.containerLoaded){
			if (this.gameObject.CompareTag ("LoadingBay"))
				this.gameObject.SetActive(true);
			else
				this.gameObject.SetActive(false);
		}
		timeLeft -= Time.deltaTime;
		//			Debug.Log (timeLeft);
		timeDisp.text = ((int)timeLeft).ToString ();
		if (timeLeft < 1 && timeLeft>-1) {
			scoreZone.enabled = true;
		} else if (timeLeft < -1) {
			this.gameObject.SetActive (false);
			Destroy (this.gameObject);
		}
	}

	void loweringTheString(){
		
	}
}