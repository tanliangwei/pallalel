using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadingBayTimer : MonoBehaviour {

	public Transform cableLoad;
	public Transform HookLoad;
	public GameObject container;

	public float maxTime = 6f;
	public float minTime = 2f;

	public TextMesh timeDisp;
	public float timeLeft;
	private Collider scoreZone;
	public GameObject parent;

	private float transitionTime;
	private bool transitFlag;
	private float curTime;
	private float PrevTime;

	// Use this for initialization
	void Start () {
		PrevTime = Time.time;
		timeLeft = minTime + Random.value * (maxTime - minTime);
		if (GM.tutorial == true) {
			if (GM.forTut == false) {
				timeLeft = 7;

			} else {
				timeLeft = 8;
			}

		}
		timeDisp.text =((int) timeLeft).ToString();
		scoreZone = this.GetComponent<Collider> ();
		scoreZone.enabled = false;
		transitFlag = true;
		transitionTime = 0;
		Debug.Log ("I am alive");
	}

	// Update is called once per frame
	void Update () {
		curTime = Time.time;
		if (GM.containerLoaded == true) {
			container.SetActive (false);
		} else {
			container.SetActive (true);
		}
		if (GM.tutorial != true) {
			timeLeft -= Time.deltaTime;
			//			Debug.Log (timeLeft);
			timeDisp.text = ((int)timeLeft).ToString ();
			if (timeLeft < 1 && timeLeft > -1) {
				scoreZone.enabled = true;
			} else if (timeLeft < -1) {
				Destroy (parent.gameObject);
			}
			if (transitFlag == true && (timeLeft <= 3)) {
				if (GM.containerLoaded == false) {
					loweringTheString (curTime - PrevTime);
				} else {
					lowerTheHook (curTime - PrevTime);
				}
			}
		} else {
			timeLeft -= Time.deltaTime;
			//			Debug.Log (timeLeft);
			if (GM.containerLoaded == false) {
				timeDisp.text = ((int)timeLeft).ToString ();
				if (timeLeft < 1) {
					scoreZone.enabled = true;
				} else if (timeLeft<-10) {
					Destroy (parent.gameObject);
				}
				if (transitFlag == true && (timeLeft <= -2)) {
					loweringTheString (curTime - PrevTime);
				}
			} else {
				timeDisp.text = ((int)timeLeft).ToString ();
				if (timeLeft < 1) {
					scoreZone.enabled = true;
				} else if (timeLeft<-10) {
					Destroy (parent.gameObject);
				}
				if (transitFlag == true && (timeLeft <= 2)) {
					lowerTheHook (curTime - PrevTime);
				}
			}
		}
		PrevTime = curTime;
	}

	void loweringTheString(float deltaTime){

		transitionTime += deltaTime;

		if (transitionTime > 0.01f) {
			Vector3 temp = new Vector3 (cableLoad.localPosition.x, cableLoad.localPosition.y - 0.0045f, cableLoad.localPosition.z);
			Vector3 temp2 = new Vector3 (HookLoad.localPosition.x, HookLoad.localPosition.y - 0.009f, HookLoad.localPosition.z);
			cableLoad.localPosition = temp;
			HookLoad.localPosition = temp2;
			cableLoad.localScale += new Vector3 (0, 0.0045f, 0);
			transitionTime = 0;
		} 

		if (GM.tutorial == true) {
			if (cableLoad.localScale.y >= 0.6f) {
				transitFlag = false;
			}
			
		} else {
			if (cableLoad.localScale.y >= 1f) {
				transitFlag = false;
			}
		}

	}

	void lowerTheHook(float deltaTime){
		transitionTime += deltaTime;

		if (transitionTime > 0.01f) {
			Vector3 temp = new Vector3 (cableLoad.localPosition.x, cableLoad.localPosition.y - 0.0075f, cableLoad.localPosition.z);
			Vector3 temp2 = new Vector3 (HookLoad.localPosition.x, HookLoad.localPosition.y - 0.015f, HookLoad.localPosition.z);
			cableLoad.localPosition = temp;
			HookLoad.localPosition = temp2;
			cableLoad.localScale += new Vector3 (0, 0.0075f, 0);
			transitionTime = 0;
		} 

		if (cableLoad.localScale.y >= 1.7f) {
			transitFlag = false;
		}
	}

}
