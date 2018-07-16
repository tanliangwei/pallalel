using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Preloader : MonoBehaviour {

	/*private CanvasGroup fadeGroup;
	private float loadTime;
	private float minimumLogoTime = 3.0f; //minimum logo time*/
	public AudioSource slashFX;
//	private bool myFlag = false;
//	private float preTime;
//	private float timePassed = 0;

	// Use this for initialization
	void Start () {
		/*fadeGroup = FindObjectOfType<CanvasGroup> (); //grab the only canvasgroup object

		fadeGroup.alpha = 0; // start off with white screen

		//preloading but in our case, nothing 

		//get a timestamp of completion time
		if (Time.time < minimumLogoTime) {
			loadTime = minimumLogoTime;
		} else {
			loadTime = Time.time;
		}*/
		//Handheld.PlayFullScreenMovie("Preloader_Mobile2.mp4", Color.black, FullScreenMovieControlMode.Full, FullScreenMovieScalingMode.AspectFill);
		StartCoroutine(streamVideo("Preloader_Mobile2.mp4"));	
				//Handheld.PlayFullScreenMovie("Preloader_Mobile2.mp4", Color.black, FullScreenMovieControlMode.Full, FullScreenMovieScalingMode.AspectFill);
		//preTime = Time.time;

	}
	
	// Update is called once per frame
//	void Update () {
//		timePassed += (Time.time - preTime);
//		if (timePassed > 10.19f && timePassed < 10.20f) {
//			slashFX.Play ();
//			Debug.Log ("playsound");
//		}
//		//fade in 
////		if (Time.time < minimumLogoTime) {
////			fadeGroup.alpha = 1 - Time.time;
////		}
////
////		//fade out
////		if (Time.time > minimumLogoTime && loadTime != 0) {
////			fadeGroup.alpha = Time.time - minimumLogoTime;
////			if (fadeGroup.alpha >= 1) {
////				Debug.Log ("change the scene");
////				SceneManager.LoadScene ("Menu");
////			}
////		}
//		preTime = Time.time;
//
//
//	}

	private IEnumerator streamVideo(string video)
	{
		Handheld.PlayFullScreenMovie("Preloader_Mobile2.mp4", Color.black, FullScreenMovieControlMode.CancelOnInput, FullScreenMovieScalingMode.AspectFill);
		yield return new WaitForSeconds (2.19f);
		slashFX.Play ();
		Debug.Log("The Video playback is now completed.");
		SceneManager.LoadScene ("Menu");
	}
}
