using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DockScript : MonoBehaviour {
	public Rigidbody rb;
	private float transittime;
	private float prevTime;
	// Use this for initialization
	void Start () {
		transittime = 0;
		prevTime = Time.time;
	}
	
	// Update is called once per frame
	void Update () {
		float curTime = Time.time;



		if (GM.ToBonusFlag == 1 && GM.bonusLevel == 0) {
			if (transittime > 0.5f) {
				Quaternion temp = GM.currentOrientation;
				rb.velocity = temp * new Vector3 (0, 0, 5);
				transittime += (curTime - prevTime);
			} else {
				transittime += (curTime - prevTime);
			}

		} else if (GM.ToBonusFlag == 0 &&  GM.bonusLevel==1) {
			if (transittime > 0.5f) {
				rb.velocity = new Vector3 (0, 0, 5);
				transittime += (curTime - prevTime);
			} else {
				transittime += (curTime - prevTime);
			}
		}
		if (transittime >= 2) {
			if (GM.ToBonusFlag == 1 && GM.bonusLevel == 0) {
				//GM.bonusLevel = 1;
				SceneManager.LoadScene ("BonusStage");
			} else if (GM.ToBonusFlag == 0 &&  GM.bonusLevel ==1) {
				//GM.bonusLevel = 0;
				SceneManager.LoadScene ("MainGame");
			}
		}
		prevTime = curTime;
	}
}
