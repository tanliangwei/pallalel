using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//let path spawner pass in a parameter to say whether the spill shld be left or right to be centralised
public class OilSpillInitiator : MonoBehaviour {

	private PlayerController playerController;
	public GameObject oilSpillEffect;
	public GameObject parent;
	private Vector3 oilSpillScale;
	private Vector3 oilSpillPos;
	public float lifeTime=10f;
	public float leftCentreRight = 0f; //-1.7 to 1.7 for left to right
	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	void OnTriggerEnter(Collider other)
	{
		if (other.gameObject.CompareTag ("Player") && !GM.AGVRampage) {
			GM.oilSpill = true;
			//TODO add oil spill 
			playerController=other.gameObject.GetComponent<PlayerController>();
			oilSpillScale = oilSpillEffect.transform.localScale;
			oilSpillScale.y= (playerController == null) ? 30.0f : playerController.forwardSpeed * 5.0f;
			oilSpillEffect.transform.localScale = oilSpillScale;
			oilSpillPos = oilSpillEffect.transform.localPosition;
			oilSpillPos.z = oilSpillEffect.transform.localScale.y / 2;
			oilSpillPos.x = -leftCentreRight;
			oilSpillEffect.transform.localPosition = oilSpillPos;
			oilSpillEffect.SetActive (true);
			animateAndDestroy(this.gameObject);

		}
	}

	private void animateAndDestroy(GameObject toDestroy){
		toDestroy.GetComponent<Animator> ().SetBool ("Collected", true);
		StartCoroutine(destroyAfter(toDestroy,1.0f));
	}

	IEnumerator destroyAfter(GameObject toDestroy, float time)
	{
		yield return new WaitForSeconds (time);
		Destroy (toDestroy);
	}
		
}
