using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour {

	public GameObject player;
	private Vector3 offset;
	private Quaternion rotator;
	private Quaternion curRotator;
	private float temp;
	private float prevTime;
	private float curTime;
	private PlayerController playerController; 
	private float apple;

	// Use this for initialization
	void Start () {
		offset = transform.position - player.transform.position;
		playerController = player.GetComponent<PlayerController> ();
		curRotator = Quaternion.identity;
	}

	// Update is called once per frame
	void LateUpdate () {
		rotator = playerController.orientRotation; // show in degrees the rotation 
		curTime = Time.time;
//		transform.position = Vector3.Slerp(transform.position, player.transform.position + rotator* offset, Time.deltaTime);
		if (rotator != curRotator) {
			//transition for 0.5s
			if (apple < 1) {
				apple = 4f* (curTime - prevTime) + apple;
				transform.position = player.transform.position + rotator * offset * (apple)+ curRotator*offset*(1-apple);
				transform.rotation = Quaternion.Slerp (curRotator, rotator, apple);
			} else {
				apple = 1;
				transform.position = player.transform.position + rotator * offset;
				transform.rotation = Quaternion.Slerp (curRotator, rotator, 1);
				curRotator = rotator;
				apple = 0;
				curRotator = rotator;
			}
		}else{
			transform.position = player.transform.position + rotator * offset;
			transform.rotation = Quaternion.Slerp (curRotator, rotator, 1);
		}
		prevTime = curTime;
		//transform.position = player.transform.position + rotator*offset;
		//transform.rotation = Quaternion.Slerp(transform.rotation, rotator, Time.time*0.01f);

	}
}
