using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathSpawnCollider : MonoBehaviour {

	public GameObject path;
	public GameObject pickUp;
	private Vector3[] pickUpPoints = new Vector3[10];
	public Transform colliderTransform;

	public GameObject player;
	private PlayerController playerController;
	private Vector3 currentPosition; // position of spawnPathCollider
	private int nextPathOrientation; //-1 if next path is left, 0 for straight, 1 for right of current path
	private Vector3 offsetForNextPath; //offset needed to position the next path correctly
	public GameObject rotateRightCollider;
	public GameObject rotateLeftCollider;

	private int numberOfObstacles;
	private Quaternion currentRotation;

	private List<Vector3> obstaclePoints = new List<Vector3> ();
	private Vector3 obstaclePoint;
	public GameObject obstacle;



	void Start()
	{
		currentPosition = colliderTransform.position;
		currentRotation = colliderTransform.rotation;
		playerController = player.GetComponent<PlayerController> ();
	}


	void OnTriggerEnter(Collider other)
	{
		nextPathOrientation = Random.Range (-1, 2); //decides if next path is straight ahead, to the right or to the left)
		GM.pathOrientation += nextPathOrientation;
		offsetForNextPath = determineOffset(GM.pathOrientation-nextPathOrientation, nextPathOrientation);
		if (other.gameObject.tag == "Player") {
			currentPosition += offsetForNextPath;
			currentRotation = playerController.calcOrientation (GM.pathOrientation%4) * Quaternion.identity;
			Debug.Log ("nextPathOrientation is " + nextPathOrientation + "GM.pathOrientation is " + GM.pathOrientation);
			Instantiate (path, currentPosition,currentRotation);
			if (nextPathOrientation == 1)
				rotateRightCollider.SetActive (true);
			else if (nextPathOrientation == -1)
				rotateLeftCollider.SetActive (true);
			numberOfObstacles = Random.Range (0, 10);
			for (int i = 0; i < numberOfObstacles; i++) {
				obstaclePoint = new Vector3 ();
				obstaclePoint.x = Random.Range (-1, 2) * 3;
				obstaclePoint.y = pickUp.transform.position.y;
				obstaclePoint.z = 90f + 10f * i;
				obstaclePoint = playerController.calcOrientation (GM.pathOrientation % 4) * obstaclePoint;
				Instantiate (obstacle, obstaclePoint + currentPosition, obstacle.transform.rotation);
				obstaclePoints.Add (obstaclePoint);
			}
			for (int i=0; i<10; i++){
				pickUpPoints [i].x = Random.Range (-1, 2) * 3;
				pickUpPoints [i].y = pickUp.transform.position.y;
				pickUpPoints [i].z = 90f + 10f * i;
				pickUpPoints [i] = playerController.calcOrientation (GM.pathOrientation % 4) * pickUpPoints [i];
//				while (obstaclePoints.Contains (pickUpPoints [i]))
//					pickUpPoints [i].x = Random.Range (-1, 2) * 3;
				Instantiate (pickUp, pickUpPoints [i], pickUp.transform.rotation);
			}

		}
	}

//	void OnTriggerEnter(Collider other)
//	{
//		if (other.gameObject.tag == "Player") {
//			offsetForNextPath = determineOffset (0, 1);
//			currentPosition += offsetForNextPath;
//			currentRotation = Quaternion.Euler (0, 90, 0)*currentRotation;
//			Instantiate (path, currentPosition, currentRotation);
//		}
//	}

	Vector3 determineOffset(int playerOrientation, int nextPath)
	//orientation refers to player orientation. nextPath is determined as follow: -1 for left, 0 for straight, 1 for right
	{
		int absOrientation = playerOrientation % 4;
		if (absOrientation < 0)
			absOrientation += 4;
		Quaternion rotationMatrix = playerController.calcOrientation (absOrientation);

		if (nextPath == 0) {
			Vector3 offset = new Vector3 (0f, -0.5f, 90f);
			offset = rotationMatrix * offset;
			return offset;
		} else if (nextPath == -1) {
			Vector3 offset = new Vector3 (-55f, -0.5f, 45f);
			offset = rotationMatrix * offset;
			return offset;
		} else if (nextPath == 1) {
			Vector3 offset = new Vector3 (55f, -0.5f, 45f);
			offset = rotationMatrix * offset;
			return offset;
		} else
			return new Vector3 (0f, 0f, 0f);
	}


			
}
