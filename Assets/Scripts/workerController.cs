using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class workerController : MonoBehaviour {
	Rigidbody rb;
	int leftOrRight;
	public Vector3 currentPosition;
	public Vector3 offset = Vector3.zero;
	public bool controlLocked;
	private float horizontalVel = 0;
	public Vector3 tester;

	// Use this for initialization
	void Start () {
		rb = GetComponent<Rigidbody> ();
//		rb.constraints = RigidbodyConstraints.FreezeRotationX|RigidbodyConstraints.FreezeRotation;
		leftOrRight = Random.Range (0, 2);
	}
	
	// Update is called once per frame
	void Update () {
		rb.velocity = transform.rotation * new Vector3 (horizontalVel, 0f, 0f);
		tester = rb.velocity;
		if (!controlLocked && leftOrRight==0) {
			horizontalVel = 1.7f;
			leftOrRight += 1;

			StartCoroutine (stopSlideRight ());
			controlLocked = true;
		}
		else if (!controlLocked && leftOrRight==1) {
			horizontalVel = -1.7f;
			leftOrRight -= 1;

			StartCoroutine (stopSlideLeft ());
			controlLocked = true;
		}
	}

	IEnumerator stopSlideLeft()
	{
		yield return new WaitForSeconds (1.0f);
		horizontalVel = 0;
		GetComponent<Animator>().SetBool("Walk Left",false);
		GetComponent<Animator>().SetBool("Walk Right",true);
		yield return new WaitForSeconds (0.5f);
		controlLocked = false;
	}

	IEnumerator stopSlideRight()
	{
		yield return new WaitForSeconds (1.0f);
		horizontalVel = 0;
		GetComponent<Animator>().SetBool("Walk Right",false);
		GetComponent<Animator>().SetBool("Walk Left",true);
		yield return new WaitForSeconds (0.5f);
		controlLocked = false;
	}
}
