using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinController : MonoBehaviour {

	// Use this for initialization
	void Start () {
	}

	// Update is called once per frame
	void Update () {
		if (GM.coinMagnet) {
			if (Vector3.Distance (transform.position, GameObject.FindGameObjectWithTag("Player").transform.position) < 4)
				transform.position = Vector3.MoveTowards (transform.position, GameObject.FindGameObjectWithTag("Player").transform.position, Time.deltaTime * 60);
		}
	}
}
