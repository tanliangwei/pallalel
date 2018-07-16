using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathSpawner : MonoBehaviour {

	#region define stuff
	//references to prefabs of game objects (future ones to be created)
//	public GameObject tutorialStraight;
//	public GameObject tutorialLeft;
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
	public GameObject[] calefares = new GameObject[6];


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
	private Vector3 pickUpPoint;
	private int numberOfObstacles;
	private Vector3 obstaclePoint;
	private Vector3 TEUBayPoint;
	private float powerUpTimer;
	public float thresholdTime=30.0f;
	private bool spawnPowerUp=false;
	private Vector3 powerUpPoint;
	private float oilSpillPosX;
	private Vector3 calefarePoint;
	#endregion

	// Use this for initialization
	void Start () {
		//saveManager = GetComponent<SaveManager> ();
		this.gameObject.SetActive(true);
		agvLevel = (SaveManager.Instance.getAGV () == 1) ? false : true;
		magnetLevel = (SaveManager.Instance.getMagnet () == 1) ? false : true;
		shipLevel = (SaveManager.Instance.getMagnet () == 1) ? false : true;
		freqLevel = SaveManager.Instance.getPowerups ();
		thresholdTime = 22.0f - freqLevel * 2.0f;
//		GM.tutorial = SaveManager.Instance.FirstTime();

		if (GM.bonusLevel != 1) {
			spawnPowerUp = GameObject.FindGameObjectWithTag ("Player").GetComponent<PlayerController> ().spawnPowerUp;
			GameObject.FindGameObjectWithTag ("Player").GetComponent<PlayerController> ().spawnPowerUp = false;
		}
	}

	void Update(){

	}

	void OnTriggerEnter(Collider other){
		if (other.gameObject.CompareTag ("Player")) {
//			if (GM.tutorial != true) {
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

//					for (int i = 0; i < 5; i++) {
//						if (Random.Range (0, 2) == 0) {
//							pickUpPoint.x = Random.Range (-1, 2) * 1.7f;
//							pickUpPoint.y = pickUp.transform.position.y;
//							pickUpPoint.z = 6 * i;
//							pickUpPoint = nextPathRotation * pickUpPoint;
//							Instantiate (pickUp, pickUpPoint + nextSpawnTransform.position, pickUp.transform.rotation * nextPathRotation);
//						}
//					}
					float x = 5;
					float numCoin = 5;
					//start from 5 so it wont generate at junctions, adjust range num spwn to adjust coin generation. the 3 is the space between coins
					while (x  < 30) {
						int numSpwn = Random.Range (5, 10);
						if (Random.Range (0, 4) < 3 ) {
							float tempX = Random.Range (-1, 2) * 1.7f;
							pickUpPoint.y = pickUp.transform.position.y;
							if ((numCoin + (3 * numSpwn)) < 30) {
								for (float j = 0; j < numSpwn; j++) {
									//if ((numCoin + (numSpwn * 3f)) < 30) {
										pickUpPoint.x = tempX;
										pickUpPoint.y = pickUp.transform.position.y;
										pickUpPoint.z = (x + j * 3f);
										pickUpPoint = nextPathRotation * pickUpPoint;
										Instantiate (pickUp, pickUpPoint + nextSpawnTransform.position, pickUp.transform.rotation * nextPathRotation);
									//}
									numCoin += 3f;
								}
							}

						} else {
							numCoin += (3f*numSpwn);
						}
						x+=(numSpwn*3f);
					}

					for (int i = 2; i < 5; i++) {
						int chooseObstacle = Random.Range (0, 20);
						obstaclePoint = new Vector3 ();

						obstaclePoint.z = 6 * i - 3;
						if (chooseObstacle == 0) {
							obstaclePoint.x = 0;
							obstaclePoint.y = workerObstacle.transform.position.y;
							obstaclePoint = nextPathRotation * obstaclePoint;
							i += 1;
							Instantiate (workerObstacle, obstaclePoint + nextSpawnTransform.position, workerObstacle.transform.rotation * nextPathRotation);
						} else if (chooseObstacle == 1) {
							obstaclePoint.x = Random.Range (-1, 2) * 1.8f;
							obstaclePoint.y = coneObstacle.transform.position.y;
							obstaclePoint = nextPathRotation * obstaclePoint;
							i += 1;
							Instantiate (coneObstacle, obstaclePoint + nextSpawnTransform.position, coneObstacle.transform.rotation * nextPathRotation);
						} else if (chooseObstacle == 2) {
							if (Random.Range (-1, 1) == 0)
								obstaclePoint.x = 1;
							else
								obstaclePoint.x = -1;
							i += 1;
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

					int choice = Random.Range (0, 6);
					calefarePoint = calefares [choice].transform.position;
					calefarePoint.z = 8 + Random.value * 15;
					calefarePoint = nextPathRotation * calefarePoint;
					Instantiate (calefares [choice], calefarePoint + nextSpawnTransform.position, calefares [choice].transform.rotation*nextPathRotation);

					if (spawnPowerUp) {
						powerUpPoint.x = Random.Range (-1, 2) * 1.7f;
						oilSpillPosX = powerUpPoint.x;
						powerUpPoint.z = 3 * Random.Range (0, 10);
						powerUpPoint = nextPathRotation * powerUpPoint;
						int decidePowerUp = Random.Range (1, 11);
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
						} else if (decidePowerUp < 11 && shipLevel) {
							powerUpPoint.y = shipPower.transform.position.y;
							Instantiate (shipPower, powerUpPoint + nextSpawnTransform.position, shipPower.transform.rotation * nextPathRotation);
						}
					}
					this.GetComponentInParent<Collider> ().enabled = false;
				} else if (GM.bonusLevel == 1) {
					nextPathRotation = this.transform.rotation * GM.straightpath.rotation;
					//straightPath.GetComponentInChildren<>
					Instantiate ((straightPath), nextSpawnTransform.position, nextPathRotation);
					for (int i = 0; i < 30; i+=15) {
						pickUpPoint.x = Random.Range (-1, 2) * 1.7f;

						pickUpPoint.y = pickUp.transform.position.y;
						pickUpPoint.z = i;
						pickUpPoint = nextPathRotation * pickUpPoint;
						Instantiate (pickUp, pickUpPoint + nextSpawnTransform.position, pickUp.transform.rotation * nextPathRotation);
						pickUpPoint.z = i + 3;
						Instantiate (pickUp, pickUpPoint + nextSpawnTransform.position, pickUp.transform.rotation * nextPathRotation);
						pickUpPoint.z = i + 6;
						Instantiate (pickUp, pickUpPoint + nextSpawnTransform.position, pickUp.transform.rotation * nextPathRotation);
						pickUpPoint.z = i + 9;
						Instantiate (pickUp, pickUpPoint + nextSpawnTransform.position, pickUp.transform.rotation * nextPathRotation);
						pickUpPoint.z = i + 12;
						Instantiate (pickUp, pickUpPoint + nextSpawnTransform.position, pickUp.transform.rotation * nextPathRotation);
//						pickUpPoint.z = i + 15;
//						Instantiate (pickUp, pickUpPoint + nextSpawnTransform.position, pickUp.transform.rotation * nextPathRotation);
					}
				}
			} else if (GM.transitionFlag == 1) {

				if (GM.bonusLevel == 1) {
				
					nextPathRotation = this.transform.rotation * GM.straightpath.rotation;
					Vector3 temp = new Vector3 (nextSpawnTransform.position.x, nextSpawnTransform.position.y + 1f, nextSpawnTransform.position.z);
					Instantiate ((dockPath), temp, nextPathRotation);
					GM.transitionFlag = 0;
					GM.BonusPathFlag = true;
				} else if (GM.bonusLevel == 0) {
					nextPathRotation = this.transform.rotation * GM.straightpath.rotation;
					Instantiate ((dockPath), nextSpawnTransform.position, nextPathRotation);
					GM.transitionFlag = 0;
					GM.BonusPathFlag = true;
				}

			}
			this.gameObject.SetActive (false);

		}
	}
}
			

