using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour {
	private static GameController instance;


	void Awake(){
		if (instance == null) 
		{
			instance = this;
		} 
		else 
		{
			DestroyImmediate (this);
		}
	}


	public static GameController Instance {
		get {
			if (instance == null) {
				instance = new GameController ();
			}
			return instance;
		}
	}
}
