using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VolumeManagerMain : MonoBehaviour {

	public Slider MusicSlider;
	public Slider EffectsSlider;
	private AudioSource mainBGMSource;
	private AudioSource pickupSound;
	private AudioSource hazeSound;
	private AudioSource oilSpillSound;
	private AudioSource coinSound;
	private AudioSource slowDownSound;
	private AudioSource speedUpSound;
	private AudioSource carCrashSound;
	private AudioSource clickSound;

	public void Awake() {
		
//		Slider MusicSlider = FindObjectsOfType<Slider>()[0];
//		Slider EffectsSlider = FindObjectsOfType<Slider> () [1];
		mainBGMSource = GameObject.FindGameObjectWithTag ("MainCamera").GetComponent<AudioSource> ();
		pickupSound = GameObject.FindGameObjectWithTag ("Player").GetComponents<AudioSource>()[0];
		hazeSound = GameObject.FindGameObjectWithTag ("Player").GetComponents<AudioSource> ()[1];
		oilSpillSound = GameObject.FindGameObjectWithTag ("Player").GetComponents<AudioSource>() [2];
		coinSound = GameObject.FindGameObjectWithTag ("Player").GetComponents<AudioSource>()[3];
		slowDownSound = GameObject.FindGameObjectWithTag ("Player").GetComponents<AudioSource>()[4];
		speedUpSound = GameObject.FindGameObjectWithTag ("Player").GetComponents<AudioSource>()[5];
		carCrashSound = GameObject.FindGameObjectWithTag ("Player").GetComponents<AudioSource>()[6];
		clickSound = GameObject.FindGameObjectWithTag ("Player").GetComponents<AudioSource>()[7];
		InitVolume ();
	}

	public void InitVolume () {
//		Slider MusicSlider = FindObjectsOfType<Slider>()[0];
//		Slider EffectsSlider = FindObjectsOfType<Slider> () [1];
		//MusicSlider.value = SaveManager.Instance.getMusic();
		//EffectsSlider.value = SaveManager.Instance.getEffect();

		mainBGMSource.volume = MusicSlider.value;
		pickupSound.volume = EffectsSlider.value;
		hazeSound.volume = EffectsSlider.value;
		oilSpillSound.volume = EffectsSlider.value;
		coinSound.volume = EffectsSlider.value;
		slowDownSound.volume = EffectsSlider.value;
		speedUpSound.volume = EffectsSlider.value;
		carCrashSound.volume = EffectsSlider.value;
		clickSound.volume = EffectsSlider.value;

	}	


	public void volumeControllerMusic(float volumeControl) {
		
		Debug.Log ("ValueChanging");
		SaveManager.Instance.saveMusic (volumeControl);
		mainBGMSource.volume = MusicSlider.value;
	
	}

	public void volumeControllerEffects(float volumeControl) {
		Debug.Log ("ValueChanging");
		SaveManager.Instance.saveEffect(volumeControl);
		pickupSound.volume = EffectsSlider.value;
		hazeSound.volume = EffectsSlider.value;
		oilSpillSound.volume = EffectsSlider.value;
		coinSound.volume = EffectsSlider.value;
		slowDownSound.volume = EffectsSlider.value;
		speedUpSound.volume = EffectsSlider.value;
		carCrashSound.volume = EffectsSlider.value;
		clickSound.volume = EffectsSlider.value;
	}



}
