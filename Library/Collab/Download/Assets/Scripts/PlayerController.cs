using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class PlayerController : MonoBehaviour {

//	public KeyCode moveL;
//	public KeyCode moveR;

	#region Defining initial variables

	//canvas
	public CanvasGroup dyingScene;
	public CanvasGroup settingsCanvas;
	public CanvasGroup quitCanvas;
	public Transform Box;
	public GameObject scoreBar;
	private Vector3[] dieObject;
	private Vector3[] initialpos;
	private Vector3 initialBoxPos;
	public GameObject GoldBoard;
	public GameObject scoreBoard;
	public GameObject lifeBoard;
	//public CanvasGroup ComboCanvas;
	public Transform ComboLevel;
	private float ComboCount;
	public float axialposition;
	private AudioSource brakeSound;
	private AudioSource speedupSound;
	private AudioSource clickSound;
	private AudioSource containerPickUpSound;
	private AudioSource containerDropOffSound;
	private AudioSource scoreBoardSound;
	private AudioSource shipHornSound;
	private VolumeManagerMain volumeManagerMain;
	private bool scoreSoundPlay = false;
	public TextMesh plusTwenty;


	private static PlayerController instance;
	public static PlayerController Instance {get{return instance;}} // Current main use is to get the player orientation
	public GameObject straightFab;
	public GameObject rightFab;
	public GameObject leftFab;
	public GameObject hazeScreen;
	public GameObject gameManager;
	private SaveManager saveManager;
	Rigidbody rb;

	//To control switching of lanes. Lane 1-3 is left to right
	private float horizontalVel = 0;
	public int laneNum =0;
	private bool controlLocked = false;
	public float changeLaneSpeed;
	private float initialLaneSpeed;

	// To control speeding up and slowing down
	public float forwardSpeed = 10f;
	private float initialSpeed = 10f;
	private float currentSpeed;
	private bool speedShift = false;

	public Transform CameraView;

	//Used for scoring
	private int count=0;
	public float score;
	private int tempScore = 0;

	//Used for save manager stuff
	private float comboDecreaseRate;
	private int containerMultiplier;
	private int lives;
	private float freqLevel;
	public float powerUpTimer=0.0f;
	private float thresholdTime=30.0f;
	public bool spawnPowerUp=true;
	private int magnetLevel;
	private int AGVLevel;

	public Vector3 velocity;
	private bool isPaused;
	private bool isTutorial = false;

	//Used for turning
	private static Quaternion turnRight;	//Serves as rotation "matrix" to rotate the player's velocity
	public int orientation = 0;	//0 for front, 1 for right, 2 for back, 3 for left;
	private int previousDirection;
	public Quaternion orientRotation;	//This will be the player's rotation relative to the world coordinate
	private int beforeOrient; // to store and check for changes in orientation;
	private int currentOrient;
	private float transitionOrientTime;
	private float previousTime;
	private float curTime;
	private GameObject rotationCollider;
	private float ComboTime;
	private bool isRotating=false;

	//USED FOR DYING
	private bool isDead = false;
	private bool hasAddedGold = false;
	private float scoreTime = 0;
	private int highScore;

	public Animator ShipFullPower;
	private float fade = 0;

	#endregion

	// Use this for initialization
	void Start () {
		ShipFullPower.SetBool ("isFull", false);
		Time.timeScale = 1;
		GM.transitionFlag = 0;
		GM.ToBonusFlag = 0;
		GM.slowDown = false;
		ComboTime = 0;
		previousDirection = 0;
		axialposition = 0;
		highScore = SaveManager.Instance.getHighScore();
		//init the canvas
		#region init canvas
		brakeSound = GetComponents<AudioSource>()[4];
		speedupSound = GetComponents<AudioSource>()[5];
		clickSound = GetComponents<AudioSource>()[7];
		containerPickUpSound = GetComponents<AudioSource>()[8];
		containerDropOffSound = GetComponents<AudioSource>()[9];
		scoreBoardSound = GetComponents<AudioSource>()[10];
		shipHornSound = GetComponents<AudioSource>()[11];
		volumeManagerMain = settingsCanvas.GetComponent<VolumeManagerMain>();
		volumeManagerMain.InitVolume();

		dyingScene.alpha = 0;
		settingsCanvas.alpha = 0;
		settingsCanvas.interactable = false;
		settingsCanvas.blocksRaycasts = false;
		quitCanvas.alpha = 0;
		quitCanvas.interactable = false;
		quitCanvas.blocksRaycasts = false;



		dieObject = new Vector3[6];
		initialBoxPos = new Vector3();
		initialBoxPos = Box.position;
		int i = 0;
		foreach (Transform t in Box) {
			dieObject [i] = t.position;
			//t.position = Box.position;
			i++;
		}
		Box.position = new Vector3 (Screen.width/2, Screen.height*1.5f, 0);

		//ComboCanvas.alpha = 0.5f;
		//ComboLevel.GetComponentInChildren<RectTransform> ().sizeDelta = new Vector2 (50, 1);
		#endregion

		forwardSpeed = GM.initialSpeed;
		count = GM.goldScore;
		score = GM.currentScore;

		//All things save manager
		saveManager = gameManager.GetComponent<SaveManager> ();
		initialLaneSpeed = 1.0f + Mathf.Log(1+saveManager.getAgility ());
		changeLaneSpeed = initialLaneSpeed;
		containerMultiplier = saveManager.getContainerPoints ();

		if (GM.FirstRun) {
			lives = saveManager.getLife ();
			GM.LifeRemain = lives;
			GM.FirstRun = false;
		} else {
			lives = GM.LifeRemain;
		}
		comboDecreaseRate = saveManager.getCombo ();
		freqLevel = saveManager.getPowerups ();
		thresholdTime = 22.0f - freqLevel * 2.0f;
		magnetLevel = saveManager.getMagnet ();
		AGVLevel = saveManager.getAGV ();

		#region initiatioin for movement
		GM.bonusLevel = 0;
		instance = this;
		rb = GetComponent<Rigidbody> ();
		currentSpeed = forwardSpeed;
		turnRight = Quaternion.AngleAxis (90f, Vector3.up);	
		orientRotation = Quaternion.identity;	
		currentOrient = 0;
		beforeOrient = 0;
		//to retain the quaternion data. Because the quaternion data changes despite being gotten from prefab
		GM.straightpath = straightFab.transform;
		GM.rightpath = rightFab.transform;
		GM.leftpath = leftFab.transform;
		rb.constraints = RigidbodyConstraints.FreezeRotationX|RigidbodyConstraints.FreezeRotation;
		lifeBoard.GetComponent<Text> ().text = lives.ToString ();
		#endregion
	}

	// Update is called once per frame
	void Update () {
		//to check for rotations
		curTime = Time.time;
		velocity = rb.velocity;
		if (rb.position.y > 0.5) {
			if (isDead != true) {
				if (!GM.slowDown) {

					if (plusTwenty.gameObject.activeSelf) {
						fade += (curTime - previousTime);
						Color tempCol = new Color (plusTwenty.color.r, plusTwenty.color.g, plusTwenty.color.b, 1 - fade*fade);
						plusTwenty.color = tempCol;
						Vector3 tempPos = new Vector3 (plusTwenty.GetComponentInChildren<Transform> ().localPosition.x, plusTwenty.GetComponentInChildren<Transform> ().localPosition.y, (fade * 5f)-5);
						plusTwenty.GetComponentInChildren<Transform> ().localPosition = tempPos;
						//Debug.Log ("if");
					} 

					NaturalDecrease (curTime - previousTime);
					GoldBoard.GetComponentInParent<Text> ().text = count.ToString ();
					rotatingFunction (beforeOrient, currentOrient, curTime, previousTime);
					changeLaneSpeed = initialLaneSpeed * (1.7f + 2.5f * Mathf.Clamp01 (Mathf.Log (curTime) / 10));

					//Addition of orientRotation rotates the velocity to be in correct direction
					orientRotation = calcOrientation (orientation);
					if (!speedShift)
						currentSpeed = forwardSpeed;
					rb.velocity = orientRotation * new Vector3 (horizontalVel, rb.velocity.y, currentSpeed);


					#region controls for player
					if (!GM.AGVRampage && !isRotating) {
						if (!GM.oilSpill) {
							if (swipeManager.Instance.SwipeLeft && (laneNum > -1) && (controlLocked == false)) {
								horizontalVel = 0f - changeLaneSpeed;
								laneNum -= 1;
								GetComponent<Animator> ().SetBool ("TurnLeft", true);
								StartCoroutine (stopSlide ());
								controlLocked = true;
							}
							//Input.GetKeyDown (moveR)
							if (swipeManager.Instance.SwipeRight && (laneNum < 1) && (controlLocked == false)) {
								horizontalVel = changeLaneSpeed;
								GetComponent<Animator> ().SetBool ("TurnRight", true);
								laneNum += 1;
								StartCoroutine (stopSlide ());
								controlLocked = true;
							}
						} else {
							if (swipeManager.Instance.SwipeRight && (laneNum > -1) && (controlLocked == false)) {
								horizontalVel = 0f - changeLaneSpeed;
								GetComponent<Animator> ().SetBool ("TurnLeft", true);
								laneNum -= 1;
								StartCoroutine (stopSlide ());
								controlLocked = true;
							}
							//Input.GetKeyDown (moveR)
							if (swipeManager.Instance.SwipeLeft && (laneNum < 1) && (controlLocked == false)) {
								horizontalVel = changeLaneSpeed;
								GetComponent<Animator> ().SetBool ("TurnRight", true);
								laneNum += 1;
								StartCoroutine (stopSlide ());
								controlLocked = true;
							}
						}

						if (swipeManager.Instance.SwipeUp && (speedShift == false)) {
							currentSpeed *= 2;
							StartCoroutine (stopBoost ());
							speedShift = true;
							speedupSound.Play ();
						}

						if (swipeManager.Instance.SwipeDown && (speedShift == false)) {
							currentSpeed /= 3;
							StartCoroutine (stopBrake ());
							speedShift = true;
							brakeSound.Play ();
						}
						forwardSpeed = initialSpeed + Mathf.Log (curTime + 1);
					
					} else if (GM.AGVRampage && !isRotating) {
						rb.velocity = orientRotation * new Vector3 (horizontalVel, rb.velocity.y, 20f);
						if (GM.containerLoaded && (laneNum != -1) && (controlLocked == false)) {
							horizontalVel = 0f - changeLaneSpeed;
							laneNum -= 1;
							GetComponent<Animator> ().SetBool ("TurnLeft", true);
							StartCoroutine (stopSlide ());
							controlLocked = true;
						}
						if (!GM.containerLoaded && (laneNum != 1) && (controlLocked == false)) {
							horizontalVel = changeLaneSpeed;
							GetComponent<Animator> ().SetBool ("TurnRight", true);
							laneNum += 1;
							StartCoroutine (stopSlide ());
							controlLocked = true;
						}
						forwardSpeed = 20f;

					}
					#endregion


					//Decreases the combo bar with ti						me
					if (!isPaused) {
						score += Time.timeSinceLevelLoad * 0.01f;
						scoreBoard.GetComponent<Text> ().text = ((int)score).ToString ();
					}

				} else {

					//todo enter the transition shit
					BonusLer (curTime - previousTime);


				}
				
			}
			if (curTime - powerUpTimer > thresholdTime) {
				spawnPowerUp = true;
				powerUpTimer = curTime;
			}

		} else if (rb.position.y <= 0.5) {
			lives -= 1;
			lifeBoard.GetComponent<Text> ().text = lives.ToString ();
			if (lives <= 0) {
				scoreBar.GetComponentInChildren<Text> ().text = score.ToString ();
				isDead = true;
			} else if (lives > 0)
				orientRotation = calcOrientation (orientation);
			rb.position = new Vector3 (rb.position.x, 1.0f, rb.position.z) + transform.TransformDirection (new Vector3 (0f, 30f, 0f));
		}

		if (isDead) {
			GM.containerLoaded = false;
			this.transform.position = this.transform.position;
			Text[] tempText = scoreBar.GetComponentsInChildren<Text> ();

			//highscoreupdates here
//			if (tempScore <= count) {
//				//tempScore += 533;
//				tempText [0].text = tempScore.ToString ();
//			}
			//tempText [0].text = count.ToString ();
			//tempText [1].text = ((int)score).ToString ();
			if (((int)score) > highScore) {
				highScore = ((int)score);
				SaveManager.Instance.updateHighScore (highScore);
			}

			#region count increment
			if(!scoreBoardSound.isPlaying){
				scoreBoardSound.PlayDelayed(0.6f);
			}
			if (((int)score) > 2000) {

				int diff = ((int)score) / 167;

				if (tempScore < ((int)score)) {
					if (scoreTime > 0.01f) {
						tempScore += diff + 7;
						tempText [1].text = tempScore.ToString ();
						scoreTime = 0;
					}
				} else {
					tempText [1].text = ((int)score).ToString ();
					scoreBoardSound.Stop();
				}
			} else if(((int)score)>500){
				int diff = ((int)score) / 33;

				if (tempScore < ((int)score)) {
					if (scoreTime > 0.05f) {
						tempScore += diff + 3;
						tempText [1].text = tempScore.ToString ();
						scoreTime = 0;
					}

				} else {
					tempText [1].text = ((int)score).ToString ();
					scoreBoardSound.Stop();
				}
				
			}else{
				if (tempScore < ((int)score)) {
					if (scoreTime > 0.01) {
						tempScore += 17;
						tempText [1].text = tempScore.ToString ();
						scoreTime = 0;

					}
				}else {
					tempText [1].text = ((int)score).ToString ();
					scoreBoardSound.Stop();

				}
			}
			#endregion

			scoreTime += (curTime - previousTime);
			tempText [0].text = count.ToString ();
			tempText [2].text = highScore.ToString ();
			//tempText [1].text = ((int)score).ToString ();
			Dieler (curTime - previousTime);
			//Vector3 temp = orientRotation * new Vector3 (horizontalVel, rb.velocity.y, currentSpeed);
			rb.velocity = Vector3.zero;
			rb.useGravity = false;
			GM.oilSpill = false;
			GM.goldScore = count;
			GM.coinMagnet = false;
			GM.AGVRampage = false;

			//sound stuff


			//			TODO Add the stuff upon death
		}
//		if (currentOrient == beforeOrient) {
//			if (controlLocked == false) {
//
//				Debug.Log("orientation is "+orientation );
//				if (orientation == 0) {
//					transform.position = new Vector3 ((axialposition + laneNum * 1.700f), transform.position.y, transform.position.z);
//					Debug.Log ("front");
//				} else if (orientation == 1) {
//					transform.position = new Vector3 (transform.position.x, transform.position.y, (axialposition + laneNum * -1.700f));
//					Debug.Log ("right");
//				} else if (orientation == 2) {
//					transform.position = new Vector3 ((axialposition + laneNum * -1.700f), transform.position.y, transform.position.z);
//					Debug.Log ("behind");
//				} else if (orientation == 3) {
//					transform.position = new Vector3 (transform.position.x, transform.position.y, (axialposition + laneNum * 1.700f));
//					Debug.Log ("right");
//				} else {
//					Debug.Log ("nope");
//				}
//			}
//		}
//		} 
		previousTime = curTime;

		//for settings menu to work
		if (GM.settingsMenu == 1) {
			volumeManagerMain.InitVolume ();
		}
	}



	#region for dieing and bonus level
	void BonusLer(float deltaTime){
		GM.transitionFlag = 0;
		transitionOrientTime += deltaTime;

		if (transitionOrientTime >= 1) {
			transitionOrientTime = 0;
			GM.currentScore = (int)score;
			GM.goldScore = count;
			GM.coinMagnet = false;
			GM.AGVRampage = false;
			GM.oilSpill = false;
			GM.hazeScreen = false;
			//GM.bonusLevel = 1;
			rb.velocity = Vector3.zero;
			GM.ToBonusFlag = 1;
			GM.initialSpeed = forwardSpeed;
			GM.LifeRemain = lives;
			GM.currentOrientation = calcOrientation (orientation);
		} else {
			//Debug.Log (rb.velocity);
			rb.velocity = Vector3.Lerp (rb.velocity, Vector3.zero, (1*transitionOrientTime));

		}
	}

	void Dieler(float deltaTime){
		transitionOrientTime += deltaTime;

		if (transitionOrientTime >= 2) {
			int i = 0;
			foreach (Transform t in Box) {
				t.position = dieObject [i];
				i++;
			}
			//transitionOrientTime = 0;
		} else {
			if (transitionOrientTime >= 1) {
				int i = 0;
				Box.position = initialBoxPos;
				dyingScene.alpha = 0.5f;
				foreach (Transform t in Box) {
					//t.position = Vector3.Lerp (Box.position, dieObject [i], (transitionOrientTime - 1));
					i++;
				}

			} else {
				dyingScene.alpha = 0.5f * (transitionOrientTime);
//				Vector3 temp = new Vector3 (Screen.width/2, Screen.height*1.5f,0);
				//Box.position = Vector3.Lerp (temp, initialBoxPos, transitionOrientTime);
			}
		}

		if (!hasAddedGold) {
			hasAddedGold = true;
			GM.goldScore = 0;
			//count = 0;
			SaveManager.Instance.addGold (count);

		}
			
	}

	#endregion

	void OnTriggerEnter(Collider other)
	{
		if (other.gameObject.CompareTag("Pick Up"))
		{
			animateAndDestroy (other.gameObject);
			count += 1;
			//IncrementCombo (50);
		}

		if (other.gameObject.CompareTag ("Obstacle") && !GM.AGVRampage) {
			lives -= 1;
			if (lives <= 0) {
				scoreBar.GetComponentInChildren<Text>().text = score.ToString();
				isDead = true;
			}
			else if (lives > 0)
				Destroy (other.gameObject);
			GetComponent<Animator>().SetBool("Crash",true);
			lifeBoard.GetComponent<Text> ().text = lives.ToString ();
			Invoke ("stopCrashAnim", 2.0f);
		}
		if (other.gameObject.CompareTag ("RotateLeft") && (laneNum==-1|| GM.AGVRampage)) {
			orientation-=1;
			GetComponent<Animator>().SetBool("TurnLeft",true);
			currentOrient -= 90;
			rotationCollider = other.gameObject;
			laneNum = -1;
			//rb.constraints = RigidbodyConstraints.FreezeRotationX|RigidbodyConstraints.FreezeRotationZ;

			other.gameObject.SetActive (false);
		}
		if (other.gameObject.CompareTag ("RotateRight") && (laneNum == 1 || GM.AGVRampage)) {
			orientation += 1;
			GetComponent<Animator>().SetBool("TurnRight",true);

			currentOrient += 90;
			rotationCollider = other.gameObject;
			laneNum = 1;
			//code to rotate that shit
			other.gameObject.SetActive (false);
		}
		if ((other.gameObject.CompareTag ("RotateLeft0") || other.gameObject.CompareTag ("RotateLeft1")) && GM.AGVRampage ) {
			orientation-=1;
			GetComponent<Animator>().SetBool("TurnLeft",true);
			currentOrient -= 90;
			rotationCollider = other.gameObject;
			if (other.gameObject.CompareTag ("RotateLeft0"))
				laneNum = 0;
			else
				laneNum = 1;
			//rb.constraints = RigidbodyConstraints.FreezeRotationX|RigidbodyConstraints.FreezeRotationZ;

			other.gameObject.SetActive (false);
		}
		if ((other.gameObject.CompareTag ("RotateRight0") || other.gameObject.CompareTag ("RotateRight-1")) && GM.AGVRampage) {
			orientation += 1;
			GetComponent<Animator>().SetBool("TurnRight",true);

			currentOrient += 90;
			rotationCollider = other.gameObject;
			if (other.gameObject.CompareTag ("RotateRight0"))
				laneNum = 0;
			else
				laneNum = -1;
			other.gameObject.SetActive (false);
		}
		if (other.gameObject.CompareTag ("LoadingBay")) {
			GM.containerLoaded = true;
			GetComponent<Animator> ().SetBool ("Loaded", true);
			containerPickUpSound.Play ();
		}

		if (other.gameObject.CompareTag ("UnloadingBay")) {
			GM.containerLoaded = false;
			int temp  = Mathf.RoundToInt(1000f * containerMultiplier);
			score += temp;
			//do the shit here
			IncrementCombo (25);
			StartCoroutine (scorePoint (temp));
			GetComponent<Animator> ().SetBool ("Loaded", false);
			containerDropOffSound.Play ();
		}

		if (other.gameObject.CompareTag ("CoinMagnet") && !GM.AGVRampage) {
			GM.coinMagnet = true;
			animateAndDestroy (other.gameObject);
			StartCoroutine (stopMagnet ());

		}

		if (other.gameObject.CompareTag ("HazeScreen") && !GM.AGVRampage) {
			GM.hazeScreen = true;
			hazeScreen.SetActive (true);
			animateAndDestroy (other.gameObject);
			StartCoroutine (stopHaze ());
		}

		if (other.gameObject.CompareTag ("AGVRampage")) {
			animateAndDestroy (other.gameObject);
			StartCoroutine (startAGV ());
			GetComponent<Animator>().SetBool("AGV",true);
			StartCoroutine (stopAGV ());
		}

		if (other.gameObject.CompareTag ("ShipBonanza")) {
			IncrementCombo (100);
			animateAndDestroy (other.gameObject);
		}

		if (other.gameObject.CompareTag("SlowDown")) {
			GM.slowDown = true;
		}
		if (other.gameObject.CompareTag("TransitNow")) {
			GM.currentScore = (int)score;
			GM.goldScore = count;
			GM.coinMagnet = false;
			GM.AGVRampage = false;
			GM.oilSpill = false;
			GM.hazeScreen = false;
			rb.velocity = Vector3.zero;
			GM.ToBonusFlag = 1;
			GM.initialSpeed = forwardSpeed;
			GM.LifeRemain = lives;
			GM.currentOrientation = calcOrientation (orientation);
			//SceneManager.LoadScene ("BonusStage");
		}

	}

	//to increment the combo

	#region combo bar stuff

	void NaturalDecrease(float transition){
		if (ComboCount < 100) {
			ComboTime += transition;
			if (ComboTime > 0.5f) {
				IncrementCombo (currentSpeed * (-0.02f) * (2 - 1.14f*Mathf.Log10(comboDecreaseRate)));
				ComboTime = 0;
			}
		}
	}

	void  IncrementCombo(float incrementer){
		ComboCount = Mathf.Clamp(ComboCount + incrementer, 0, 100);
		float temp = Mathf.Round((ComboCount / 100)*450f);

		ComboLevel.GetComponentInChildren<RectTransform> ().anchoredPosition = new Vector2 (0f, temp-490f);

		if (ComboCount <= 0) {
			//ComboCount = 0;
			ComboLevel.GetComponentInChildren<RectTransform> ().anchoredPosition = new Vector2 (0f, -490f);
		}
		if (ComboCount >= 100) {
			//ComboCanvas.alpha = 0.8f;
			ComboLevel.GetComponentInChildren<RectTransform> ().anchoredPosition = new Vector2 (0f, -40);
			ShipFullPower.SetBool ("isFull", true);
			shipHornSound.Play ();
			
			//TODO enter the transition here
			BonusTransition(); 

		} else {
			//ComboCanvas.alpha = 0.5f;
		}
	}

	void ResetCombo(){
		//ComboCanvas.alpha = 0.5f;
		//ComboLevel.GetComponentInChildren<RectTransform> ().sizeDelta = new Vector2 (50, 1);
	}

	void BonusTransition(){
		GM.initialSpeed = forwardSpeed;
		GM.transitionFlag = 1;

	}

	#endregion

	#region IEnumerators

	IEnumerator scorePoint(int points){
		plusTwenty.text = string.Concat ("+", points.ToString ());
		plusTwenty.gameObject.SetActive (true);
		yield return new WaitForSeconds (1f);
		plusTwenty.gameObject.SetActive (false);
		//Debug.Log ("point function");
		fade = 0;
	}

	IEnumerator stopSlide()
	{
		yield return new WaitForSeconds (1.7f/changeLaneSpeed);
		if (laneNum!=0)
			yield return new WaitForSeconds (0.1f/changeLaneSpeed);
		horizontalVel = 0;
		controlLocked = false;
		GetComponent<Animator>().SetBool("TurnLeft",false);
		GetComponent<Animator>().SetBool("TurnRight",false);
	}


	IEnumerator stopBoost()
	{
		yield return new WaitForSeconds (1.7f/changeLaneSpeed);
		currentSpeed = forwardSpeed;
		speedShift = false;
	}

	IEnumerator stopBrake()
	{
		yield return new WaitForSeconds (0.5f);
		currentSpeed = forwardSpeed;
		speedShift = false;
	}

	IEnumerator stopMagnet()
	{
		yield return new WaitForSeconds (2.0f*magnetLevel);
		GM.coinMagnet = false;
	}

	IEnumerator stopOil()
	{
		yield return new WaitForSeconds (10.0f);
		GM.oilSpill = false;
	}
	IEnumerator stopHaze()
	{
		yield return new WaitForSeconds (10.0f);
		GM.hazeScreen = false;
		hazeScreen.SetActive (false);
	}

	IEnumerator startAGV()
	{
		yield return new WaitForSeconds (0.1f);
		GM.AGVRampage = true;
		GM.coinMagnet = true;
	}

	IEnumerator stopAGV()
	{
		yield return new WaitForSeconds (2.0f*AGVLevel-1.5f);
		GetComponent<Animator>().SetBool("Countdown",true);
		yield return new WaitForSeconds (1.5f);
		GetComponent<Animator>().SetBool("Countdown",false);
		GM.AGVRampage = false;
		GM.coinMagnet = false;
		GetComponent<Animator>().SetBool("AGV",false);
	}

	IEnumerator destroyAfter(GameObject toDestroy, float time)
	{
		yield return new WaitForSeconds (time);
		Destroy (toDestroy);
	}

	#endregion
		

	#region rotation functions
	void rotatingFunction(float before,float current,float currentTime,float previousTime){
		if (current != before) {
			isRotating = true;
			transitionOrientTime += currentTime - previousTime;
			if (transitionOrientTime < 0.25) {
				float temporary = beforeOrient + (4 * (transitionOrientTime) * (currentOrient - beforeOrient));
				transform.eulerAngles = new Vector3 (-90, temporary, 0);
			} else {
				beforeOrient = currentOrient;
				transform.eulerAngles = new Vector3 (-90, currentOrient, 0);
				transitionOrientTime = 0;
			}
			if (current < before)
				GetComponent<Animator> ().SetBool ("TurnLeft", true);
			else if (current > before)
				GetComponent<Animator> ().SetBool ("TurnRight", true);
			transform.position = rotationCollider.transform.position;
			alignMe ();


		} else {
			GetComponent<Animator> ().SetBool ("TurnLeft", false);
			GetComponent<Animator> ().SetBool ("TurnRight", false);
			isRotating = false;
		}

	}

	//to obtain the right orientation
	public Quaternion calcOrientation(int numberOfTimes){
		Quaternion orientRotation = Quaternion.identity;
		numberOfTimes = numberOfTimes % 4;
		if (numberOfTimes < 0)
			numberOfTimes += 4;
		for (int i = 0; i < numberOfTimes; i++)
			orientRotation *= turnRight;			// To get the correct player orientation, we successively apply 90 degree rotations
		return orientRotation;
	}
	#endregion

	private void animateAndDestroy(GameObject toDestroy){
		toDestroy.GetComponent<Animator> ().SetBool ("Collected", true);
		StartCoroutine(destroyAfter(toDestroy,0.5f));
	}
		

	#region buttons to go

	public void onMainMenu(){

		GM.goldScore = 0;
		GM.currentScore = 0;
		count = 0;
		SceneManager.LoadScene ("Menu");

		DestroyObject (this);
	}

	public void onPlayAgain(){
		GM.goldScore = 0;
		GM.currentScore = 0;
		count = 0;
		SceneManager.LoadScene(SceneManager.GetActiveScene().name);
	}

	#endregion

	void alignMe(){
		if (previousDirection != orientation) {
			if (previousDirection == 1) {
				axialposition = transform.position.x + 1.700f;
			} else if (previousDirection == 3) {
				axialposition = transform.position.x - 1.700f;
			} else if (previousDirection == 0) {
				axialposition = transform.position.z + 1.700f;
			} else if (previousDirection == 2) {
				axialposition = transform.position.z - 1.700f;
			} else {
			}
			previousDirection = orientation;
		}
	}

	void stopCrashAnim(){
		GetComponent<Animator>().SetBool("Crash",false);
	}

//open settings menu
	public void onPausePress(){
		clickSound.Play ();
		openPauseMenu ();
	}

	public void openPauseMenu(){
		isPaused = true;
		settingsCanvas.interactable = true;
		settingsCanvas.blocksRaycasts = true;
		settingsCanvas.alpha = 1;
		Time.timeScale = 0;
		GM.settingsMenu = 1;

	}

	public void onSettingsBackClick(){
		isPaused = false;
		clickSound.Play ();
		settingsCanvas.interactable = false;
		settingsCanvas.blocksRaycasts = false;
		settingsCanvas.alpha = 0;
		Time.timeScale = 1;
		GM.settingsMenu = 0;
	}

//press tutorial button in settings
	public void onTutorialPress(){
		isTutorial = true;
		clickSound.Play ();
		openQuitMenu ();
	}

	public void openQuitMenu(){
		quitCanvas.alpha = 1;
		quitCanvas.interactable = true;
		quitCanvas.blocksRaycasts = true;

	}

	public void quitMenuYes(){
		clickSound.Play ();
		if (isTutorial) {
			SceneManager.LoadScene ("Tutorial");
			GM.goldScore = 0;
			GM.currentScore = 0;
		} else {
			GM.goldScore = 0;
			GM.currentScore = 0;
			onSettingsBackClick ();
			SceneManager.LoadScene ("Menu");
		}
	}

	public void quitMenuNo(){
		clickSound.Play ();
		quitCanvas.alpha = 0;
		quitCanvas.interactable = false;
		quitCanvas.blocksRaycasts = false;
	}

//press quit button in settings
	public void onQuitPress(){

		clickSound.Play ();
		isTutorial = false;
		openQuitMenu ();
		//onSettingsBackClick ();
		//onMainMenu ();

		//DestroyObject (this);
	}

}