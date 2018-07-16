using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSoundController : MonoBehaviour {

	public AudioSource pickupSound;
	public AudioSource hazeSound;
	public AudioSource oilSpillSound;
	public AudioSource coinSound;
	public AudioSource crashSound;

	public AudioSource[] source;

	// Use this for initialization
	void Start () {
		source = GetComponents<AudioSource> ();
		pickupSound = source [0];
		hazeSound = source [1];
		oilSpillSound = source [2];
		coinSound = source [3];
		crashSound = source [6];
	}
	
	// Update is called once per frame
	void OnTriggerEnter(Collider other){
		if (other.gameObject.CompareTag("Pick Up")){
			coinSound.Play();
		}

		else if (other.gameObject.CompareTag("OilSpill")){
			oilSpillSound.Play();
		}
		else if (other.gameObject.CompareTag("HazeScreen")){
			hazeSound.Play ();
		}
		else if (other.gameObject.CompareTag("AGVRampage") || other.gameObject.CompareTag("CoinMagnet") || other.gameObject.CompareTag("ShipBonanza")){
			pickupSound.Play ();

		}
		else if (other.gameObject.CompareTag("Obstacle")){
			crashSound.Play ();
		}
}
}
