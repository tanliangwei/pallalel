using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class swipeManager : MonoBehaviour {

	private static swipeManager instance;
	public static swipeManager Instance {get{return instance;}}

	private bool tap, swipeLeft, swipeRight, swipeUp, swipeDown;
	private bool isDragin = false;
	private Vector2 startTouch, swipeDelta;

	private void Start(){
		instance = this;
	}

	private void Update(){
		tap = swipeUp = swipeRight = swipeLeft = swipeDown = false;

		#region stand alone input
		if (Input.GetMouseButtonDown (0)) {
			tap = true;
			isDragin = true;
			startTouch = Input.mousePosition;
		} else if (Input.GetMouseButtonUp (0)) {
			isDragin = false;
			Reset ();
		}
		#endregion

		#region Mobile Inputs
		if (Input.touches.Length != 0) {

			if (Input.touches [0].phase == TouchPhase.Began) {
				tap = true;
				isDragin = true;
				startTouch = Input.touches [0].position;
			} else if (Input.touches [0].phase == TouchPhase.Ended || Input.touches [0].phase == TouchPhase.Canceled) {
				isDragin = false ; 
				Reset ();
			}
		}

		#endregion

		//calculate distance
		swipeDelta = Vector2.zero;
		if (isDragin) {
			if (Input.touches.Length > 0)
				swipeDelta = Input.touches [0].position - startTouch;
			else if (Input.GetMouseButton (0))
				swipeDelta = (Vector2)Input.mousePosition - startTouch;
		}

		#region did we cross threshold
		if (swipeDelta.magnitude > 125) {

			//which direction
			float x = swipeDelta.x;
			float y = swipeDelta.y;
			if (Mathf.Abs (x) > Mathf.Abs (y)) {
				//left or right
				if (x < 0)
					swipeLeft = true;
				else
					swipeRight = true;
			} else {
				//up or down
				if (y < 0)
					swipeDown = true;
				else
					swipeUp = true;

			}
		}
		#endregion

	}
	private void Reset(){
		startTouch = swipeDelta = Vector2.zero;
		isDragin = false;
	}

	public Vector2 SwipeDelta{get{return swipeDelta;}}
	public bool SwipeLeft{get{ return swipeLeft;}}
	public bool SwipeRight{get{ return swipeRight;}}
	public bool SwipeUp{get{ return swipeUp;}}
	public bool SwipeDown{get{ return swipeDown;}}

}
