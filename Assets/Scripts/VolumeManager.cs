using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VolumeManager : MonoBehaviour {

	public Slider MusicSlider;
	public Slider EffectsSlider;
	public AudioSource MusicSource;
	public AudioSource boughtSound;
	public AudioSource failSound;
	public AudioSource clickSound;
	public AudioSource playSound;

	public void Start() {
		InitVolume ();
		Slider MusicSlider = FindObjectsOfType<Slider>()[0];
		Slider EffectsSlider = FindObjectsOfType<Slider> () [1];
		boughtSound = GameObject.FindGameObjectWithTag ("MenuScene").GetComponents<AudioSource>()[0];
		failSound = GameObject.FindGameObjectWithTag ("MenuScene").GetComponents<AudioSource> ()[1];
		clickSound = GameObject.FindGameObjectWithTag ("MenuScene").GetComponents<AudioSource>() [2];
		playSound = GameObject.FindGameObjectWithTag ("MenuScene").GetComponents<AudioSource>()[3];
	}

	public void InitVolume () {
		MusicSource = GetComponent<AudioSource> ();
		MusicSlider = FindObjectsOfType<Slider>()[0];
		EffectsSlider = FindObjectsOfType<Slider> () [1];
		boughtSound = GameObject.FindGameObjectWithTag ("MenuScene").GetComponents<AudioSource>()[0];
		failSound = GameObject.FindGameObjectWithTag ("MenuScene").GetComponents<AudioSource> ()[1];
		clickSound = GameObject.FindGameObjectWithTag ("MenuScene").GetComponents<AudioSource>() [2];
		playSound = GameObject.FindGameObjectWithTag ("MenuScene").GetComponents<AudioSource>()[3];
		MusicSlider.value = SaveManager.Instance.getMusic();
		EffectsSlider.value = SaveManager.Instance.getEffect();
		MusicSource.volume = MusicSlider.value;
		boughtSound.volume = EffectsSlider.value;
		failSound.volume = EffectsSlider.value;
	}


	public void volumeControllerMusic(float volumeControl) {
		Debug.Log ("ValueChanging");
		SaveManager.Instance.saveMusic (volumeControl);
		MusicSource.volume = volumeControl;
	}

	public void volumeControllerEffects(float volumeControl) {
		Debug.Log ("ValueChanging");
		SaveManager.Instance.saveEffect(volumeControl);
		failSound.volume = volumeControl;
		boughtSound.volume = volumeControl;
		playSound.volume = volumeControl;
		clickSound.volume = volumeControl;
		GameObject.FindGameObjectWithTag ("MenuScene").GetComponents<AudioSource> () [0].volume = volumeControl;
		GameObject.FindGameObjectWithTag ("MenuScene").GetComponents<AudioSource> () [1].volume = volumeControl;
		GameObject.FindGameObjectWithTag ("MenuScene").GetComponents<AudioSource> () [2].volume = volumeControl;
		GameObject.FindGameObjectWithTag ("MenuScene").GetComponents<AudioSource> () [3].volume = volumeControl;

	}
}
