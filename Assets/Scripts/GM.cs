using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GM : MonoBehaviour {

	public static bool tutorial;
	public static int tutorialPathCount=0;
	public static int tutorialCounter = 0;
	public static float vertVel = 0;
	public static int pathOrientation = 0;
	public static Transform straightpath;
	public static Transform rightpath;
	public static Transform leftpath;
	public static int bonusLevel; // for path generation purposes
	public static bool coinMagnet=false;
	public static bool oilSpill=false;
	public static bool hazeScreen = false;
	public static bool AGVRampage = false;
	public static bool containerLoaded = false;
	public static Quaternion nextPathRotation;
	public static float initialSpeed = 5f;
	public static int transitionFlag = 0;
	public static bool slowDown = false;
	public static int goldScore=0;
	public static int currentScore=0;
	public static int ToBonusFlag = 0;
	public static Quaternion currentOrientation;
	public static int settingsMenu = 0;
	public static bool forTut = false;
	public static bool BonusPathFlag = false;
	public static int LifeRemain = 0;
	public static bool FirstRun = true;

//	private float freqLevel;
//	private float powerUpTimer;
//	private float currTime=0.0f;
//	public float thresholdTime=30.0f;
//	public static bool spawnPowerUp=true;

	public static GM Instance;

	private void Awake()
	{
		if(Instance == null)
		{
			Instance = this;
			DontDestroyOnLoad(gameObject);
		}else if(Instance != this)
		{
			Destroy(gameObject);
		}
//		freqLevel = GetComponent<SaveManager> ().getPowerups ();
	}

//	void Update()
//	{
//		powerUpTimer = Time.time;
//		Debug.Log ("Timer is running " + powerUpTimer);
//		if (powerUpTimer - currTime > thresholdTime) {
//			spawnPowerUp = true;
//			currTime = powerUpTimer;
//			Debug.Log ("timer exceeds threshold");
//		}
//	}


}
