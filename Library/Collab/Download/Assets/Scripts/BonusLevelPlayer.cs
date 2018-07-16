using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class BonusLevelPlayer : MonoBehaviour {

	#region Defining initial variables
	private static BonusLevelPlayer instance;
	public static BonusLevelPlayer Instance {get{return instance;}} // Current main use is to get the player orientation
	public GameObject straightFab;
	public GameObject rightFab;
	public GameObject leftFab;
	Rigidbody rb;

	//To control switching of lanes. Lane 1-3 is left to right
	public float horizontalVel = 0;
	public int laneNum =2;

	// To control speeding up and slowing down
	public float forwardSpeed = 0.05f;
	public float currentSpeed;

	public Transform CameraView;
	public Canvas BoatIcon;
	public Transform ComboBar;

	//Used for scoring
	private int count=0;
	//Used for combo bar and combo multiplier
	public float comboStep=10;
	private float comboMultiplier =1;
	private int shipLevel;



	//Used for turning
	private static Quaternion turnRight;	//Serves as rotation "matrix" to rotate the player's velocity
	public int orientation = 0;	//0 for front, 1 for right, 2 for back, 3 for left;
	public Quaternion orientRotation;	//This will be the player's rotation relative to the world coordinate
	public Quaternion beforeRotation;
	public Quaternion Camerarotation;
	public float beforeOrient; // to store and check for changes in orientation;
	public float currentOrient;
	public float transitionOrientTime;
	public Vector3 offset;
	public float theBonusTime = 0;
	public float curTime;
	public float preTime;
	public float score;
	#endregion
	private bool isPaused = false;

	//Used for Audio
	private AudioSource shipBGMSource;
	private AudioSource clickSound;
	private AudioSource coinSound;

	//Used for UI
	public GameObject scoreBoard;
	public GameObject goldBoard;
	//public GameObject comboLevel;
	public CanvasGroup pauseCanvas;
	public CanvasGroup blockCanvas;
	public Button pauseButton;
	public Sprite pauseImage;
	public Sprite playImage;

	private Vector3 initialPosBoat;

	public Animator floatingShip;
	public Transform LifeText;


	// Use this for initialization
	void Start () {
		pauseCanvas.gameObject.SetActive (false);
		blockCanvas.gameObject.SetActive (false);
		LifeText.GetComponentInChildren<Text>().text = (GM.LifeRemain).ToString ();
		//LifeText.getComponentInChildren<Text>.text = (GM.LifeRemain).
		floatingShip.SetBool ("isBoatScene", true);
		initialPosBoat = BoatIcon.GetComponentInChildren<RectTransform> ().localPosition;
		//Debug.Log ("the boat initial vector is " + BoatIcon.GetComponentInChildren<RectTransform> ().position);
		GM.transitionFlag = 0;
		GM.BonusPathFlag = false;
		//comboLevel.GetComponentInChildren<RectTransform> ().sizeDelta = new Vector2 (50, 435);
		//Debug.Log (GM.initialSpeed);
		score = GM.currentScore;
		count = GM.goldScore;
		//GM.ToBonusFlag = 0;
		scoreBoard.GetComponentInParent<Text>().text = ((int)score).ToString();
		goldBoard.GetComponentInParent<Text>().text = ((int)count).ToString();
		GM.slowDown = false;
		transitionOrientTime = 0f;
		GM.bonusLevel = 1;
		instance = this;
		rb = GetComponent<Rigidbody> ();
		currentSpeed = GM.initialSpeed*1.5f;
		turnRight = Quaternion.AngleAxis (90f, Vector3.up);	
		orientRotation = Quaternion.identity;	
		beforeRotation = Quaternion.identity;
		Camerarotation = Quaternion.identity;
		currentOrient = 0;
		beforeOrient = 0;
		//to retain the quaternion data. Because the quaternion data changes despite being gotten from prefab
		GM.straightpath = straightFab.transform;
		GM.rightpath = rightFab.transform;
		GM.leftpath = leftFab.transform;
		rb.constraints = RigidbodyConstraints.FreezeRotationX|RigidbodyConstraints.FreezeRotation ;
		//Debug.Log (GM.bonusLevel);
		offset = CameraView.transform.position - transform.position;
		preTime = Time.time;

		shipBGMSource = GetComponents<AudioSource> ()[0];
		clickSound = GetComponents<AudioSource>()[1];
		coinSound = GetComponents<AudioSource> () [2];
		shipBGMSource.volume = SaveManager.Instance.getMusic ();

		pauseCanvas.interactable = false;
		pauseCanvas.blocksRaycasts = false;
		blockCanvas.interactable = false;
		blockCanvas.blocksRaycasts = false;
		shipLevel = SaveManager.Instance.getBoat ();
	}

	// Update is called once per frame
	void Update () {
		curTime = Time.time;
		theBonusTime += (curTime-preTime);

		DecrementCombo ();
		if (GM.slowDown != true) {
			if (theBonusTime >= 5.0f*(shipLevel-1) && GM.BonusPathFlag == false) {
				GM.transitionFlag = 1;
			}
			Vector3 dir = Vector3.zero;
			dir.x = -Input.acceleration.y;
			dir.z = Input.acceleration.x;
//		dir.z = Mathf.SmoothDamp(rb.position.x, Input.acceleration.x, ref xVel, 0.3F);
			float chicken = 3 * (dir.z);

			//if (dir.sqrMagnitude > 1)
			//	dir.Normalize();
			//Debug.Log ("y: " + dir.y + " x: " + dir.z);
			//to check for rotations
			rotatingFunction (beforeOrient, currentOrient, curTime, preTime);

			//to update camera position
			//CameraView.LookAt (transform);
			Vector3 Cameraposition = transform.position + offset;
			Cameraposition.x = 0;
			//CameraView.rotation = new Vector3 (100, 0, 0);
			CameraView.position = Cameraposition;
			//CameraView.rotation = 

			//Addition of orientRotation rotates the velocity to be in correct direction
			orientRotation = calcOrientation (orientation);
			rb.velocity = orientRotation * new Vector3 (0, GM.vertVel, currentSpeed);
			Vector3 ballposition = rb.position;
			ballposition.x = chicken;
//		rb.MovePosition (ballposition);
			rb.position = Vector3.Lerp (rb.position, ballposition, 0.1f);


			if (rb.position.x > 1.7f) {
				rb.velocity = new Vector3 (0, GM.vertVel, currentSpeed);
				Vector3 temp = rb.position;
				temp.x = 1;
				rb.MovePosition (temp);

			} else if (rb.position.x < -1.7f) {
				rb.velocity = new Vector3 (0, GM.vertVel, currentSpeed);
				Vector3 temp = rb.position;
				temp.x = -1;
				rb.MovePosition (temp);
			}
			if (!isPaused) {
				score += curTime * 0.01f;
				scoreBoard.GetComponentInParent<Text>().text = ((int)score).ToString();
			}
		} else {
			//Debug.Log (rb.velocity);
			BonusEndLer (curTime - preTime);
		}
			
		preTime = curTime;

	}

	void  DecrementCombo(){
		float temp = Mathf.Round((theBonusTime / ( 5.0f*(shipLevel-1)))*420f);

		ComboBar.GetComponentInChildren<RectTransform> ().anchoredPosition = new Vector2 (0f, (-79 - temp));

		Vector3 theTransform = new Vector3 (initialPosBoat.x, initialPosBoat.y - temp, initialPosBoat.z);
		//Debug.Log ("the boat vector is " + theTransform);

		BoatIcon.GetComponentInChildren<RectTransform> ().localPosition = theTransform;
		//ComboLevel.GetComponentInChildren<RectTransform> ().anchoredPosition = new Vector2 (0f, temp-490f);

		if (theBonusTime <= 0) {
			//ComboLevel.GetComponentInChildren<RectTransform> ().anchoredPosition = new Vector2 (0f, -490f);
			ComboBar.GetComponentInChildren<RectTransform>().anchoredPosition = new Vector2(0f, -79f);
		}
		if ((theBonusTime / ( 5.0f*(shipLevel-1))) >=1) {
			//ComboCanvas.alpha = 0.8f;
			//ComboLevel.GetComponentInChildren<RectTransform> ().anchoredPosition = new Vector2 (0f, -40);
			ComboBar.GetComponentInChildren<RectTransform>().anchoredPosition = new Vector2(0f,-499f);
			theTransform = new Vector3 (initialPosBoat.x, initialPosBoat.y - 420f, initialPosBoat.z);
			BoatIcon.GetComponentInChildren<RectTransform> ().localPosition = theTransform;
			//TODO enter the transition here
			//BonusTransition(); 

		} else {
			//ComboCanvas.alpha = 0.5f;
		}
	}

	void BonusEndLer(float deltaTime){
		GM.transitionFlag = 0;
		transitionOrientTime += deltaTime;

		if (transitionOrientTime >= 1) {
			transitionOrientTime = 0;
			rb.velocity = Vector3.zero;
			//GM.slowDown = false;
			GM.currentScore = (int)score;
			GM.goldScore = count;
			GM.ToBonusFlag = 0;

		} else {
			rb.velocity = Vector3.Lerp (rb.velocity, Vector3.zero, (1f*transitionOrientTime));

		}
	}


	void OnTriggerEnter(Collider other)
	{
		if (other.gameObject.CompareTag("Pick Up"))
		{
			Destroy(other.gameObject);
			count += 1*(int)comboMultiplier;
			forwardSpeed += 0.0f;
			goldBoard.GetComponentInParent<Text>().text = ((int)count).ToString ();
			coinSound.Play ();
		}

		if (other.gameObject.CompareTag ("Obstacle")) {
			Destroy (this.gameObject);
		}
		if (other.gameObject.CompareTag ("RotateLeft")) {
			orientation-=1;
			//Debug.Log ("turning left");

			currentOrient -= 90;	
			//rb.constraints = RigidbodyConstraints.FreezeRotationX|RigidbodyConstraints.FreezeRotationZ;

			other.gameObject.SetActive (false);
		}
		if (other.gameObject.CompareTag ("RotateRight")) {
			orientation += 1;
			//Debug.Log ("turning Right");
			currentOrient += 90;

			//code to rotate that shit
			other.gameObject.SetActive (false);
		}
		if (other.gameObject.CompareTag ("LoadingBay")) {
			comboMultiplier+=comboStep;
			//Debug.Log ("Yay loading bay works" + comboMultiplier);
		}
		if (other.gameObject.CompareTag("SlowDown")) {
			GM.slowDown = true;
		}
		if (other.gameObject.CompareTag("TransitNow")) {
			rb.velocity = Vector3.zero;
			//GM.slowDown = false;
			GM.currentScore = (int)score;
			GM.goldScore = count;
			GM.ToBonusFlag = 0;
			Debug.Log ("hello" + rb.velocity);
			//SceneManager.LoadScene ("MainGame");
		}

	}


	void rotatingFunction(float before,float current,float currentTime,float previousTime){
		if (current != before) {
			transitionOrientTime += currentTime - previousTime;
			if (transitionOrientTime < 0.5) {
				float temporary = beforeOrient + (2 * (transitionOrientTime) * (currentOrient - beforeOrient));
				transform.eulerAngles = new Vector3 (-90, temporary, 0);
			} else {
				beforeOrient = currentOrient;
				transform.eulerAngles = new Vector3 (-90, currentOrient, 0);
				transitionOrientTime = 0;
			}
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

	//ui stuff

	public void onPausePress(){
		clickSound.Play ();
		if (Time.timeScale == 1) {
			openPauseMenu ();
			Time.timeScale = 0.00f;
		} else {
			closePauseMenu ();
			Time.timeScale = 1;
		}
	}


	public void openPauseMenu(){
		Debug.Log ("pause press entered");
		isPaused = true;
		pauseCanvas.gameObject.SetActive (!pauseCanvas.gameObject.activeSelf);
		blockCanvas.gameObject.SetActive (!blockCanvas.gameObject.activeSelf);
		pauseButton.GetComponentInChildren<Image> ().sprite = playImage;
			
	}

	public void closePauseMenu(){
		clickSound.Play ();
		isPaused = false;
		pauseCanvas.gameObject.SetActive (!pauseCanvas.gameObject.activeSelf);
		blockCanvas.gameObject.SetActive (!blockCanvas.gameObject.activeSelf);
		pauseButton.GetComponentInChildren<Image> ().sprite = pauseImage;
	}
		




}
