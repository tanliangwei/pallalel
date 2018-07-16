using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathSpawner : MonoBehaviour {

	#region define stuff
	//references to prefabs of game objects (future ones to be created)
	public GameObject tutorialStraight;
	public GameObject tutorialLeft;
	public GameObject straightPath;
	public GameObject rightPath;
	public GameObject leftPath;
	public GameObject dockPath;
	public GameObject pickUp;
	public GameObject workerObstacle;
	public GameObject coneObstacle;
	public GameObject barrierObstacle;
	public GameObject TEUBay;
	public GameObject coinMagnet;
	public GameObject hazePowerUp;
	public GameObject oilSpill;
	public GameObject AGVPower;
	public GameObject shipPower;

	//variables storing upgrade levels from save manager
	private SaveManager saveManager;
	private bool agvLevel;
	private bool magnetLevel;
	private int freqLevel;
	private bool shipLevel;


	//references to already present game objects to get position or rotation references
	public Transform nextSpawnTransform;

	//just storing of random data
	private GameObject oilSpillInstance;

	//data that are calculated
	private int nextPathOrientation;
	private Quaternion nextPathRotation;
	private Vector3[] pickUpPoints = new Vector3[10];
	private int numberOfObstacles;
	private List<Vector3> obstaclePoints = new List<Vector3> ();
	private Vector3 obstaclePoint;
	private Vector3 TEUBayPoint;
	private float powerUpTimer;
	private float currTime=0.0f;
	public float thresholdTime=30.0f;
	private bool spawnPowerUp=false;
	private Vector3 powerUpPoint;
	private float oilSpillPosX;
	#endregion

	// Use this for initialization
	void Start () {
		//saveManager = GetComponent<SaveManager> ();
		agvLevel = (SaveManager.Instance.getAGV () == 0) ? false : true;
		magnetLevel = (SaveManager.Instance.getMagnet () == 0) ? false : true;
		shipLevel = (SaveManager.Instance.getMagnet () == 0) ? false : true;
		freqLevel = SaveManager.Instance.getPowerups ();
		thresholdTime = 22.0f - freqLevel * 2.0f;
//		GM.tutorial = SaveManager.Instance.FirstTime();
		GM.tutorial = true;

		//Testing purpose. Still failing
//		if (GM.spawnPowerUp) {
//			Debug.Log ("GM spawn power up is true");
//			spawnPowerUp = true;
//			GM.spawnPowerUp = false;
//		}

//		spawnPowerUp = GameObject.FindGameObjectWithTag ("Player").GetComponent<PlayerController> ().spawnPowerUp;
//		GameObject.FindGameObjectWithTag ("Player").GetComponent<PlayerController> ().spawnPowerUp = false;
	}

	void Update(){

	}

	void OnTriggerEnter(Collider other){
		if (other.gameObject.CompareTag ("Player")) {
			if (GM.tutorial != true) {
				if (GM.transitionFlag == 0) {
					if (GM.bonusLevel == 0) {
						//this block of code generates the next path
						nextPathOrientation = Random.Range (0, 11);

						if (nextPathOrientation <= 8) {
							nextPathRotation = this.transform.rotation * GM.straightpath.rotation;
							Instantiate ((straightPath), nextSpawnTransform.position, nextPathRotation);
						} else if (nextPathOrientation == 9) {
							nextPathRotation = this.transform.rotation * GM.rightpath.rotation;
							Instantiate ((rightPath), nextSpawnTransform.position, nextPathRotation);
						} else if (nextPathOrientation == 10) {
							nextPathRotation = this.transform.rotation * GM.leftpath.rotation;
							Instantiate ((leftPath), nextSpawnTransform.position, nextPathRotation);
						}
						GM.nextPathRotation = nextPathRotation;
				

						//this portion of code generates the obstacles and pick up

						for (int i = 0; i < 5; i++) {
							if (Random.Range (0, 2) == 0) {
								pickUpPoints [i].x = Random.Range (-1, 2) * 1.7f;
								pickUpPoints [i].y = pickUp.transform.position.y;
								pickUpPoints [i].z = 6 * i;
								pickUpPoints [i] = nextPathRotation * pickUpPoints [i];
								Instantiate (pickUp, pickUpPoints [i] + nextSpawnTransform.position, pickUp.transform.rotation * nextPathRotation);
							}
						}

						for (int i = 2; i < 5; i++) {
							int chooseObstacle = Random.Range (0, 30);
							obstaclePoint = new Vector3 ();

							obstaclePoint.z = 6 * i - 2;
							if (chooseObstacle == 0) {
								obstaclePoint.x = 0;
								obstaclePoint.y = workerObstacle.transform.position.y;
								obstaclePoint = nextPathRotation * obstaclePoint;
								Instantiate (workerObstacle, obstaclePoint + nextSpawnTransform.position, workerObstacle.transform.rotation * nextPathRotation);
							} else if (chooseObstacle == 1) {
								obstaclePoint.x = Random.Range (-1, 2) * 1.8f;
								obstaclePoint.y = coneObstacle.transform.position.y;
								obstaclePoint = nextPathRotation * obstaclePoint;
								Instantiate (coneObstacle, obstaclePoint + nextSpawnTransform.position, coneObstacle.transform.rotation * nextPathRotation);
							} else if (chooseObstacle == 2) {
								if (Random.Range (-1, 1) == 0)
									obstaclePoint.x = 1;
								else
									obstaclePoint.x = -1;
								obstaclePoint.y = barrierObstacle.transform.position.y;
								obstaclePoint = nextPathRotation * obstaclePoint;
								Instantiate (barrierObstacle, obstaclePoint + nextSpawnTransform.position, barrierObstacle.transform.rotation * nextPathRotation);
							}
						}

						//Generates the loading point.
						//TODO Checking for collisions with other objects and tuning generation probability
						TEUBayPoint.x = TEUBay.transform.position.x;
						TEUBayPoint.y = TEUBay.transform.position.y;
						TEUBayPoint.z = 3 * Random.Range (2, 9);
						TEUBayPoint = nextPathRotation * TEUBayPoint + nextSpawnTransform.position;
						Instantiate (TEUBay, TEUBayPoint, nextPathRotation);

						if (spawnPowerUp) {
							powerUpPoint.x = Random.Range (-1, 2) * 1.7f;
							oilSpillPosX = powerUpPoint.x;
							powerUpPoint.z = 3 * Random.Range (0, 10);
							powerUpPoint = nextPathRotation * powerUpPoint;
							int decidePowerUp = Random.Range (1, 10);
							if (decidePowerUp < 5 && !GM.coinMagnet && magnetLevel) {
								powerUpPoint.y = coinMagnet.transform.position.y;
								Instantiate (coinMagnet, powerUpPoint + nextSpawnTransform.position, coinMagnet.transform.rotation * nextPathRotation);
							} else if (decidePowerUp < 7) {
								powerUpPoint.y = hazePowerUp.transform.position.y;
								Instantiate (hazePowerUp, powerUpPoint + nextSpawnTransform.position, hazePowerUp.transform.rotation * nextPathRotation);
							} else if (decidePowerUp < 9) {
								powerUpPoint.y = oilSpill.transform.position.y;
								oilSpillInstance = Instantiate (oilSpill, powerUpPoint + nextSpawnTransform.position, oilSpill.transform.rotation * nextPathRotation);
								oilSpillInstance.GetComponentInChildren<OilSpillInitiator> ().leftCentreRight = oilSpillPosX;
							} else if (decidePowerUp < 10 && agvLevel) {
								powerUpPoint.y = AGVPower.transform.position.y;
								Instantiate (AGVPower, powerUpPoint + nextSpawnTransform.position, AGVPower.transform.rotation * nextPathRotation);
							}
							else if (decidePowerUp < 11 && shipLevel) {
								powerUpPoint.y = shipPower.transform.position.y;
								Instantiate (shipPower, powerUpPoint + nextSpawnTransform.position, shipPower.transform.rotation * nextPathRotation);
							}
						}
						this.GetComponentInParent<Collider> ().enabled = false;
					} else if (GM.bonusLevel == 1) {
						nextPathRotation = this.transform.rotation * GM.straightpath.rotation;
						//straightPath.GetComponentInChildren<>
						Instantiate ((straightPath), nextSpawnTransform.position, nextPathRotation);
						for (int i = 0; i < 10; i++) {
							pickUpPoints [i].x = Random.Range (-1, 2) * 1.7f;
							pickUpPoints [i].y = pickUp.transform.position.y;
							pickUpPoints [i].z = 3 * i;
							pickUpPoints [i] = nextPathRotation * pickUpPoints [i];
							Instantiate (pickUp, pickUpPoints [i] + nextSpawnTransform.position, pickUp.transform.rotation * nextPathRotation);
						}
					}
				} else if (GM.transitionFlag == 1) {
				
					nextPathRotation = this.transform.rotation * GM.straightpath.rotation;
					Instantiate ((dockPath), nextSpawnTransform.position, nextPathRotation);
					GM.transitionFlag = 0;

				}
			} else {
				Debug.Log ("gm tutorial counter " + GM.tutorialCounter);
				if (GM.tutorialPathCount == 3) {
					TEUBayPoint.x = TEUBay.transform.position.x;
					TEUBayPoint.y = TEUBay.transform.position.y;
					TEUBayPoint.z = -22;
					TEUBayPoint = nextPathRotation * TEUBayPoint + nextSpawnTransform.position;
					Instantiate (TEUBay, TEUBayPoint, nextPathRotation);
					GM.forTut = true;
					TEUBayPoint.z = 18;
					TEUBayPoint = nextPathRotation * TEUBayPoint + nextSpawnTransform.position;
					Instantiate (TEUBay, TEUBayPoint, nextPathRotation);


				}

				if (GM.tutorialPathCount != 10) {
					nextPathRotation = this.transform.rotation * GM.straightpath.rotation;
					Instantiate ((tutorialStraight), nextSpawnTransform.position, nextPathRotation);
				} else {
					nextPathRotation = this.transform.rotation * GM.leftpath.rotation;
					Instantiate ((tutorialLeft), nextSpawnTransform.position, nextPathRotation);
				}
				if (GM.tutorialPathCount > 15) {
					nextPathRotation = this.transform.rotation * GM.straightpath.rotation;
					Instantiate ((straightPath), nextSpawnTransform.position, nextPathRotation);
				}
				GM.tutorialPathCount++;

				//Debug.Log (GM.tutorialPathCount);
				//make the freaking code here
			}
		}
	}
			
}
