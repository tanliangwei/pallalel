using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadingBayTimer : MonoBehaviour {

	public int maxTime = 10;
	public int minTime = 5;

	public TextMesh timeDisp;
	private float timeLeft;
	private Collider scoreZone;
	public bool testFlag1=false;
	public bool testFlag2=false;

	// Use this for initialization
	void Start () {
		timeLeft = Random.Range (minTime, maxTime);
		timeDisp.text =((int) timeLeft).ToString();
		scoreZone = this.GetComponent<Collider> ();
		scoreZone.enabled = false;
	}

	// Update is called once per frame
	void Update () {

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
}