using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class TutorialPlayerController : MonoBehaviour {

	//	public KeyCode moveL;
	//	public KeyCode moveR;

	#region Defining initial variables

	//canvas
	public CanvasGroup dyingScene;
	public CanvasGroup settingsCanvas;
	public Transform Box;
	public GameObject scoreBar;
	private Vector3[] dieObject;
	private Vector3[] initialpos;
	private Vector3 initialBoxPos;
	public GameObject GoldBoard;
	public GameObject scoreBoard;
	public GameObject lifeBoard;
	public CanvasGroup ComboCanvas;
	public Transform ComboLevel;
	private float ComboCount;
	public float axialposition;
	private AudioSource brakeSound;
	private AudioSource speedupSound;
	private AudioSource clickSound;
	private VolumeManagerMain volumeManagerMain;

	private static TutorialPlayerController instance;
	public static TutorialPlayerController Instance {get{return instance;}} // Current main use is to get the player orientation
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

	//Used for save manager stuff
	private float comboDecreaseRate;
	private int containerMultiplier;
	private int lives;
	private float freqLevel;
	public float powerUpTimer=0.0f;
	private float currTime=0.0f;
	private float thresholdTime=30.0f;
	public bool spawnPowerUp=true;

	public Vector3 velocity;
	private bool isPaused;


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

	//for tutorial
	private int tutCount;
	private bool tutorialFlag = false;
	private int movementCount = 0;
	private float AnimateTime = 0;
	private bool animateflagLocked = false;

	//animator
	public Animator animOne;
	public Animator animTwo;
	public Animator animThree;
	public Animator animFour;
	public Animator animFive;
	public Animator animSix;
//	public GameObject TutorialCanvas;
//	public Animator[] animationClips;
	#endregion
	// Use this for initialization
	void Start () {
		//GameObject.FindGameObjectWithTag ("TutorialCanvas").gameObject.SetActive (true);
//		animationClips = new Animator[15];
//		animationClips = TutorialCanvas.GetComponentsInChildren<Animator> ();
		Time.timeScale = 1;
		GM.transitionFlag = 0;
		GM.ToBonusFlag = 0;
		GM.slowDown = false;
		ComboTime = 0;
		previousDirection = 0;
		axialposition = 0;
		//init the canvas
		#region init canvas
		brakeSound = GetComponents<AudioSource>()[4];
		speedupSound = GetComponents<AudioSource>()[5];
		clickSound = speedupSound = GetComponents<AudioSource>()[7];
//				volumeManagerMain = settingsCanvas.GetComponent<VolumeManagerMain>();
//				volumeManagerMain.InitVolume();

		dyingScene.alpha = 0;
		settingsCanvas.alpha = 0;
		settingsCanvas.interactable = false;
		settingsCanvas.blocksRaycasts = false;

		dieObject = new Vector3[5];
		initialBoxPos = new Vector3();
		initialBoxPos = Box.position;
		int i = 0;
		foreach (Transform t in Box) {
			dieObject [i] = t.position;
			t.position = Box.position;
			i++;
		}
		Box.position = new Vector3 (Screen.width/2, Screen.height*1.5f, 0);

		ComboCanvas.alpha = 0.5f;
		ComboLevel.GetComponentInChildren<RectTransform> ().sizeDelta = new Vector2 (50, 1);
		#endregion

		forwardSpeed = GM.initialSpeed;
		count = GM.goldScore;
		score = GM.currentScore;

		//All things save manager
		saveManager = gameManager.GetComponent<SaveManager> ();
		initialLaneSpeed = 1.0f + Mathf.Log(saveManager.getAgility ());
		changeLaneSpeed = initialLaneSpeed;
		containerMultiplier = saveManager.getContainerPoints ();
		lives = saveManager.getLife ();
		comboDecreaseRate = saveManager.getCombo ();
		freqLevel = saveManager.getPowerups ();
		thresholdTime = 22.0f - freqLevel * 2.0f;

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
		tutCount = 0;
	}

	// Update is called once per frame
	void Update () {
		//to check for rotations
		curTime = Time.time;
		if (GM.tutorial == true) { 
			if (tutCount < 10) {
				tutorialProgression ();
			} else {
				GM.tutorial = false;
				SaveManager.Instance.tutorialOver ();
				//TODO Add code for changing scenes back to the main game.
			}
		}
		previousTime = curTime;

		//for settings menu to work
		//		if (GM.settingsMenu == 1) {
		//			volumeManagerMain.InitVolume();
		//		}
	}



	#region for dieing and bonus level
	void BonusLer(float deltaTime){
		GM.transitionFlag = 0;
		transitionOrientTime += deltaTime;

		if (transitionOrientTime >= 10) {
			transitionOrientTime = 0;

		} else {
			rb.velocity = Vector3.Lerp (rb.velocity, Vector3.zero, (0.00001f*transitionOrientTime));

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
					t.position = Vector3.Lerp (Box.position, dieObject [i], (transitionOrientTime - 1));
					i++;
				}

			} else {
				dyingScene.alpha = 0.5f * (transitionOrientTime);
				Vector3 temp = new Vector3 (Screen.width/2, Screen.height*1.5f,0);
				Box.position = Vector3.Lerp (temp, initialBoxPos, transitionOrientTime);


			}
		}

		if (!hasAddedGold) {
			hasAddedGold = true;
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
			Invoke ("stopCrash", 2.0f);
		}
		if (other.gameObject.CompareTag ("RotateLeft") && (laneNum==-1|| GM.AGVRampage == true)) {
			orientation-=1;
			GetComponent<Animator>().SetBool("TurnLeft",true);
			currentOrient -= 90;
			rotationCollider = other.gameObject;
			laneNum = -1;
			//rb.constraints = RigidbodyConstraints.FreezeRotationX|RigidbodyConstraints.FreezeRotationZ;

			other.gameObject.SetActive (false);
		}
		if (other.gameObject.CompareTag ("RotateRight") && (laneNum == 1 || GM.AGVRampage == true)) {
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

		}

		if (other.gameObject.CompareTag ("UnloadingBay")) {
			count += 100 * containerMultiplier;
			GM.containerLoaded = false;
			IncrementCombo (250);
			GetComponent<Animator> ().SetBool ("Loaded", false);
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
			GM.AGVRampage = true;
			GM.coinMagnet = true;
			animateAndDestroy (other.gameObject);
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
				IncrementCombo (currentSpeed * (-1f) * (6 - comboDecreaseRate));
				ComboTime = 0;
			}
		}
	}

	void  IncrementCombo(float incrementer){
		ComboCount = Mathf.Clamp(ComboCount + incrementer, 0, 100);
		float temp = Mathf.Round((ComboCount / 100)*435);

		ComboLevel.GetComponentInChildren<RectTransform> ().sizeDelta = new Vector2 (50, temp);
		if (ComboCount <= 0) {
			ComboLevel.GetComponentInChildren<RectTransform> ().sizeDelta = new Vector2 (50, 1);
		}
		if (ComboCount >= 100) {
			ComboCanvas.alpha = 0.8f;
			ComboLevel.GetComponentInChildren<RectTransform> ().sizeDelta = new Vector2 (50, 435);
			//TODO enter the transition here
			BonusTransition(); 

		} else {
			ComboCanvas.alpha = 0.5f;
		}
	}

	void ResetCombo(){
		ComboCanvas.alpha = 0.5f;
		ComboLevel.GetComponentInChildren<RectTransform> ().sizeDelta = new Vector2 (50, 1);
	}

	void BonusTransition(){
		GM.initialSpeed = forwardSpeed;
		GM.transitionFlag = 1;

	}

	#endregion

	#region IEnumerators

	IEnumerator clipTime(float Atime){
		yield return new WaitForSeconds (Atime);
		AnimateTime++;
		animateflagLocked = false;
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
	IEnumerator finishstuff()
	{
		yield return new WaitForSeconds (1.7f/changeLaneSpeed);
		if (laneNum!=0)
			yield return new WaitForSeconds (0.1f/changeLaneSpeed);
		if (laneNum == 1) {
			movementCount = 2;
		}
		if (laneNum == -1) {
			movementCount = -2;
		}
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
		yield return new WaitForSeconds (10.0f);
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

	IEnumerator stopAGV()
	{
		yield return new WaitForSeconds (10.0f);
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
		SceneManager.LoadScene ("Menu");

		DestroyObject (this);
	}

	public void onPlayAgain(){
		GM.goldScore = 0;
		GM.currentScore = 0;
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

	public void onHelpPress(){
		clickSound.Play ();
		openHelpMenu ();
	}

	public void openHelpMenu(){

	}

	public void onQuitPress(){

		clickSound.Play ();
		GM.goldScore = 0;
		GM.currentScore = 0;
		onSettingsBackClick ();
		onMainMenu ();

		//DestroyObject (this);
	}

	public void tutorialProgression(){
		Debug.Log ("current counter" + GM.tutorialCounter);
		switch (GM.tutorialCounter) {
		case 0:
			rb.velocity = orientRotation * new Vector3 (horizontalVel, rb.velocity.y, 10);
			tutorialFlag = true;
			break;
		case 1:
			#region case 1
			if (tutorialFlag) {
				Debug.Log("case 1 in tutorial scenemanage");
				rb.velocity = orientRotation * new Vector3 (horizontalVel, rb.velocity.y, 0);
				tutorialSceneManager (curTime - previousTime);

			} else {
				rotatingFunction (beforeOrient, currentOrient, curTime, previousTime);
				changeLaneSpeed = initialLaneSpeed * (1.7f + 2.5f * Mathf.Clamp01 (Mathf.Log (curTime) / 10));

				orientRotation = calcOrientation (orientation);
				rb.velocity = orientRotation * new Vector3 (horizontalVel, rb.velocity.y,10);
				if (swipeManager.Instance.SwipeLeft && (laneNum > -1) && (controlLocked == false)) {
					horizontalVel = 0f - changeLaneSpeed;
					laneNum -= 1;
					GetComponent<Animator> ().SetBool ("TurnLeft", true);
					StartCoroutine (stopSlide ());
					controlLocked = true;
				}
				if (swipeManager.Instance.SwipeRight && (laneNum < 1) && (controlLocked == false)) {
					horizontalVel = changeLaneSpeed;
					GetComponent<Animator> ().SetBool ("TurnRight", true);
					laneNum += 1;
					StartCoroutine (stopSlide ());
					controlLocked = true;
				}
			}
			#endregion
			break;
		case 2:
			#region case 2
			if (!tutorialFlag) {
				rb.velocity = orientRotation * new Vector3 (horizontalVel, rb.velocity.y, 0);
				tutorialSceneManager (curTime - previousTime);
				tutorialFlag=true;

			} else {
				rotatingFunction (beforeOrient, currentOrient, curTime, previousTime);
				changeLaneSpeed = initialLaneSpeed * (1.7f + 2.5f * Mathf.Clamp01 (Mathf.Log (curTime) / 10));

				orientRotation = calcOrientation (orientation);
				rb.velocity = orientRotation * new Vector3 (horizontalVel, rb.velocity.y,currentSpeed);
				if (swipeManager.Instance.SwipeLeft && (laneNum > -1) && (controlLocked == false)) {
					horizontalVel = 0f - changeLaneSpeed;
					laneNum -= 1;
					GetComponent<Animator> ().SetBool ("TurnLeft", true);
					StartCoroutine (stopSlide ());
					controlLocked = true;
				}
				if (swipeManager.Instance.SwipeRight && (laneNum < 1) && (controlLocked == false)) {
					horizontalVel = changeLaneSpeed;
					GetComponent<Animator> ().SetBool ("TurnRight", true);
					laneNum += 1;
					StartCoroutine (stopSlide ());
					controlLocked = true;
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
			}
			#endregion
			break;
		case 3:
			#region case 3
			if (movementCount<2) {
				rb.velocity = orientRotation * new Vector3 (horizontalVel, rb.velocity.y, currentSpeed);
				if (laneNum < 1 && controlLocked == false) {
					horizontalVel = changeLaneSpeed;
					GetComponent<Animator> ().SetBool ("TurnRight", true);
					laneNum += 1;
					StartCoroutine (stopSlide ());
					StartCoroutine (finishstuff ());
					controlLocked = true;
				} 
			} else if (laneNum == 1 && tutorialFlag && movementCount ==2) {
				Debug.Log ("you needa swipe down");
				rb.velocity = orientRotation * new Vector3 (0, 0, 0);
				tutorialSceneManager (curTime - previousTime);
				if (swipeManager.Instance.SwipeDown && (speedShift == false)) {
					currentSpeed /= 3;
					StartCoroutine (stopBrake ());
					speedShift = true;
					brakeSound.Play ();
					tutorialFlag = false;
					movementCount =0;
				}

			} else if (!tutorialFlag) {
				Debug.Log ("i am ready to go");
				rotatingFunction (beforeOrient, currentOrient, curTime, previousTime);
				changeLaneSpeed = initialLaneSpeed * (1.7f + 2.5f * Mathf.Clamp01 (Mathf.Log (curTime) / 10));

				orientRotation = calcOrientation (orientation);
				rb.velocity = orientRotation * new Vector3 (horizontalVel, rb.velocity.y, currentSpeed);
				#region movement
				if (swipeManager.Instance.SwipeLeft && (laneNum > -1) && (controlLocked == false)) {
					horizontalVel = 0f - changeLaneSpeed;
					laneNum -= 1;
					GetComponent<Animator> ().SetBool ("TurnLeft", true);
					StartCoroutine (stopSlide ());
					controlLocked = true;
				}
				if (swipeManager.Instance.SwipeRight && (laneNum < 1) && (controlLocked == false)) {
					horizontalVel = changeLaneSpeed;
					GetComponent<Animator> ().SetBool ("TurnRight", true);
					laneNum += 1;
					StartCoroutine (stopSlide ());
					controlLocked = true;
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
				#endregion
			} else {
			}
			#endregion

			break;
		case 4:
			#region case 4
			if (movementCount>-2) {
				tutorialFlag=true;
				rb.velocity = orientRotation * new Vector3 (horizontalVel, rb.velocity.y, currentSpeed);
				if (laneNum > -1 && controlLocked == false) {
					horizontalVel = 0 - changeLaneSpeed;
					GetComponent<Animator> ().SetBool ("TurnLeft", true);
					laneNum -= 1;
					StartCoroutine (stopSlide ());
					StartCoroutine (finishstuff ());
					controlLocked = true;
				} 
			} else if (laneNum == -1 && tutorialFlag && movementCount ==-2) {
				Debug.Log ("you needa swipe down");
				rb.velocity = orientRotation * new Vector3 (0, 0, 0);
				tutorialSceneManager (curTime - previousTime);
				if (swipeManager.Instance.SwipeUp && (speedShift == false)) {
					currentSpeed *= 2;
					StartCoroutine (stopBoost ());
					speedShift = true;
					brakeSound.Play ();
					tutorialFlag = false;
				}

			} else if (!tutorialFlag) {
				Debug.Log ("i am ready to go");
				rotatingFunction (beforeOrient, currentOrient, curTime, previousTime);
				changeLaneSpeed = initialLaneSpeed * (1.7f + 2.5f * Mathf.Clamp01 (Mathf.Log (curTime) / 10));

				orientRotation = calcOrientation (orientation);
				rb.velocity = orientRotation * new Vector3 (horizontalVel, rb.velocity.y, currentSpeed);
				#region movement
				if (swipeManager.Instance.SwipeLeft && (laneNum > -1) && (controlLocked == false)) {
					horizontalVel = 0f - changeLaneSpeed;
					laneNum -= 1;
					GetComponent<Animator> ().SetBool ("TurnLeft", true);
					StartCoroutine (stopSlide ());
					controlLocked = true;
				}
				if (swipeManager.Instance.SwipeRight && (laneNum < 1) && (controlLocked == false)) {
					horizontalVel = changeLaneSpeed;
					GetComponent<Animator> ().SetBool ("TurnRight", true);
					laneNum += 1;
					StartCoroutine (stopSlide ());
					controlLocked = true;
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
				#endregion
			} else {
			}
			#endregion
			break;
		case  5:
			//left
			#region case 5
			if (!tutorialFlag) {
				tutorialSceneManager (curTime - previousTime);
			} else {
				rotatingFunction (beforeOrient, currentOrient, curTime, previousTime);
				changeLaneSpeed = initialLaneSpeed * (1.7f + 2.5f * Mathf.Clamp01 (Mathf.Log (curTime) / 10));

				orientRotation = calcOrientation (orientation);
				rb.velocity = orientRotation * new Vector3 (horizontalVel, rb.velocity.y,currentSpeed);
			}
			#endregion
			break;
		case 6:
			//oil

			if (tutorialFlag) {
				tutorialFlag = false;
			} else {
				

				rotatingFunction (beforeOrient, currentOrient, curTime, previousTime);
				changeLaneSpeed = initialLaneSpeed * (1.7f + 2.5f * Mathf.Clamp01 (Mathf.Log (curTime) / 10));

				orientRotation = calcOrientation (orientation);
				rb.velocity = orientRotation * new Vector3 (horizontalVel, rb.velocity.y, currentSpeed);
				#region movement
				if (swipeManager.Instance.SwipeLeft && (laneNum > -1) && (controlLocked == false)) {
					horizontalVel = 0f - changeLaneSpeed;
					laneNum -= 1;
					GetComponent<Animator> ().SetBool ("TurnLeft", true);
					StartCoroutine (stopSlide ());
					controlLocked = true;
				}
				if (swipeManager.Instance.SwipeRight && (laneNum < 1) && (controlLocked == false)) {
					horizontalVel = changeLaneSpeed;
					GetComponent<Animator> ().SetBool ("TurnRight", true);
					laneNum += 1;
					StartCoroutine (stopSlide ());
					controlLocked = true;
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
				#endregion
			}
			break;
		case 7:
			//haze
			if (!tutorialFlag) {
				tutorialFlag = true;
			} else {


				rotatingFunction (beforeOrient, currentOrient, curTime, previousTime);
				changeLaneSpeed = initialLaneSpeed * (1.7f + 2.5f * Mathf.Clamp01 (Mathf.Log (curTime) / 10));

				orientRotation = calcOrientation (orientation);
				rb.velocity = orientRotation * new Vector3 (horizontalVel, rb.velocity.y, currentSpeed);
				#region movement
				if (swipeManager.Instance.SwipeLeft && (laneNum > -1) && (controlLocked == false)) {
					horizontalVel = 0f - changeLaneSpeed;
					laneNum -= 1;
					GetComponent<Animator> ().SetBool ("TurnLeft", true);
					StartCoroutine (stopSlide ());
					controlLocked = true;
				}
				if (swipeManager.Instance.SwipeRight && (laneNum < 1) && (controlLocked == false)) {
					horizontalVel = changeLaneSpeed;
					GetComponent<Animator> ().SetBool ("TurnRight", true);
					laneNum += 1;
					StartCoroutine (stopSlide ());
					controlLocked = true;
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
				#endregion
			}
			break;
		case 8:
			//pause
			rotatingFunction (beforeOrient, currentOrient, curTime, previousTime);
			changeLaneSpeed = initialLaneSpeed * (1.7f + 2.5f * Mathf.Clamp01 (Mathf.Log (curTime) / 10));

			orientRotation = calcOrientation (orientation);
			rb.velocity = orientRotation * new Vector3 (horizontalVel, rb.velocity.y, currentSpeed);
			#region movement
			if (swipeManager.Instance.SwipeLeft && (laneNum > -1) && (controlLocked == false)) {
				horizontalVel = 0f - changeLaneSpeed;
				laneNum -= 1;
				GetComponent<Animator> ().SetBool ("TurnLeft", true);
				StartCoroutine (stopSlide ());
				controlLocked = true;
			}
			if (swipeManager.Instance.SwipeRight && (laneNum < 1) && (controlLocked == false)) {
				horizontalVel = changeLaneSpeed;
				GetComponent<Animator> ().SetBool ("TurnRight", true);
				laneNum += 1;
				StartCoroutine (stopSlide ());
				controlLocked = true;
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
			#endregion
			break;
		case 9:
			//pause
			rotatingFunction (beforeOrient, currentOrient, curTime, previousTime);
			changeLaneSpeed = initialLaneSpeed * (1.7f + 2.5f * Mathf.Clamp01 (Mathf.Log (curTime) / 10));

			orientRotation = calcOrientation (orientation);
			rb.velocity = orientRotation * new Vector3 (horizontalVel, rb.velocity.y, currentSpeed);
			#region movement
			if (swipeManager.Instance.SwipeLeft && (laneNum > -1) && (controlLocked == false)) {
				horizontalVel = 0f - changeLaneSpeed;
				laneNum -= 1;
				GetComponent<Animator> ().SetBool ("TurnLeft", true);
				StartCoroutine (stopSlide ());
				controlLocked = true;
			}
			if (swipeManager.Instance.SwipeRight && (laneNum < 1) && (controlLocked == false)) {
				horizontalVel = changeLaneSpeed;
				GetComponent<Animator> ().SetBool ("TurnRight", true);
				laneNum += 1;
				StartCoroutine (stopSlide ());
				controlLocked = true;
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
			#endregion
			break;
		
		default:
			break;
		}
	}

	public void tutorialSceneManager(float deltaTime){
		switch (GM.tutorialCounter) {
		case 1:
			if (AnimateTime==2 && animateflagLocked == false) { 
				tutorialFlag = false;
				animOne.gameObject.SetActive (false);
				animTwo.SetInteger ("curState", 3);

//				animationClips [0].gameObject.SetActive (false);
//				animationClips [0].gameObject.SetActive (false);

			} else if (AnimateTime ==1 && animateflagLocked == false) {
				Debug.Log ("Animating second state");
//				animationClips [0].gameObject.SetActive (true);
//				animationClips [0].SetBool ("Animate", true);
//				animationClips [1].gameObject.SetActive (true);
//				animationClips [1].SetBool ("Animate", true);
				animTwo.SetInteger ("curState", 2);
				animateflagLocked = true;
				StartCoroutine (clipTime (5));
			} else if(AnimateTime == 0 && animateflagLocked ==false){
				animTwo.gameObject.SetActive (true);
				animTwo.SetInteger ("curState", 1);
				StartCoroutine (clipTime (1));
				animateflagLocked = true;

			}
				
			break;
		case 2:
			if (AnimateTime > 1f) {
				AnimateTime = 0; 
				tutorialFlag = true;
//				animationClips [2].gameObject.SetActive (false);

			} else {
//				animationClips [2].gameObject.SetActive (true);
//				animationClips [2].SetBool ("Animate", true);
				//LeftRight.SetBool ("Animate", true);
				AnimateTime += deltaTime;
			}
			break;
		case 3:
			break;
		case 4:
			break;
		case 5:
			tutorialFlag = true;
			break;
		default:
			break;
		}
	}
}