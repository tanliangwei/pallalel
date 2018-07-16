using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveManager : MonoBehaviour {

	public static SaveManager Instance {
		set;
		get;
	}

	public SaveState state;

	private void Awake()
	{
		//resetSave ();
		DontDestroyOnLoad(gameObject);
		Instance = this;
		Load ();
	}

	//save the whole state of this savestate scrript to the player pref

	public void Save(){
		PlayerPrefs.SetString("save",Helper.Serialize<SaveState>(state));
		//Debug.Log ("saving");
	}

	//Load the previous saved state from the player pref

	public void Load()
	{
		//DO WE ALREADY HAVE A SAVE
		if (PlayerPrefs.HasKey ("save")) {
			state = Helper.Deserialize<SaveState> (PlayerPrefs.GetString ("save"));
			//Debug.Log ("loading");
		} else {
			state = new SaveState ();
			Save ();
			//Debug.Log ("No save file found, creating a new one");
		}
	}

	#region obtain music settings

	public float getMusic(){
		return state.soundMusic;
	}

	public float getEffect(){
		return state.soundEffect;
	}

	public void saveMusic(float music){
		state.soundMusic = music;
		Save ();
	}

	public void saveEffect(float effect){
		state.soundEffect = effect;
		Save ();
	}

	#endregion

	public bool FirstTime(){
		return state.firstTime;
	}

	public void tutorialOver(){
		state.firstTime = false;
		Save();
	}

	public void tutorialReset(){
		state.firstTime = true;
		Save();
	}

	public int getHighScore(){
		return state.HighScore;
	}

	public void updateHighScore(int score){
		if (score > state.HighScore) {
			state.HighScore = score;
			Save ();
		}
	}

	public void addGold (int dollar){
		state.gold += dollar;
		Save ();
	}
	#region obtaining saved value
	public int getAgility(){
		if (translation (0, 5)) {
			return 6;
		} else if (translation (0, 4)) {
			return 5;
		} else if (translation (0, 3)) {
			return 4;
		} else if (translation (0, 2)) {
			return 3;
		} else if (translation (0, 1)) {
			return 2;
		}else{
			return 1;
		}
	}
	public int getContainerPoints(){
		if (translation (1, 5)) {
			return 6;
		} else if (translation (1, 4)) {
			return 5;
		} else if (translation (1, 3)) {
			return 4;
		} else if (translation (1, 2)) {
			return 3;
		} else if (translation (1, 1)) {
			return 2;
		} else {
			return 1;
		}
	}
	public int getCombo(){
		if (translation (2, 5)) {
			return 6;
		} else if (translation (2, 4)) {
			return 5;
		} else if (translation (2, 3)) {
			return 4;
		} else if (translation (2, 2)) {
			return 3;
		} else if (translation (2, 1)) {
			return 2;
		} else {
			return 1;
		}
	}

	public int getLife(){
		if (translation (3, 5)) {
			return 6;
		} else if (translation (3, 4)) {
			return 5;
		} else if (translation (3, 3)) {
			return 4;
		} else if (translation (3, 2)) {
			return 3;
		} else if (translation (3, 1)) {
			return 2;
		} else {
			return 1;
		}
	}

	public int getBoat(){
		if (translation2 (0, 5)) {
			return 6;
		} else if (translation2 (0, 4)) {
			return 5;
		} else if (translation2 (0, 3)) {
			return 4;
		} else if (translation2 (0, 2)) {
			return 3;
		} else if (translation2 (0, 1)) {
			return 2;
		} else {
			return 1;
		}
	}

	public int getAGV(){
		if (translation2 (1, 5)) {
			return 6;
		} else if (translation2 (1, 4)) {
			return 5;
		} else if (translation2 (1, 3)) {
			return 4;
		} else if (translation2 (1, 2)) {
			return 3;
		} else if (translation2 (1, 1)) {
			return 2;
		} else {
			return 1;
		}
	}

	public int getMagnet(){
		if (translation2 (2, 5)) {
			return 6;
		} else if (translation2 (2, 4)) {
			return 5;
		} else if (translation2 (2, 3)) {
			return 4;
		} else if (translation2 (2, 2)) {
			return 3;
		} else if (translation2 (2, 1)) {
			return 2;
		} else {
			return 1;
		}
	}

	public int getPowerups(){
		if (translation2 (3, 5)) {
			return 6;
		} else if (translation2 (3, 4)) {
			return 5;
		} else if (translation2 (3, 3)) {
			return 4;
		} else if (translation2 (3, 2)) {
			return 3;
		} else if (translation2 (3, 1)) {
			return 2;
		} else {
			return 1;
		}
	}

	public bool translation(int x, int index){
		int temp = index + (x * 6);
		return (state.powerups & (1 << temp)) != 0;
	}
	public bool translation2(int x, int index){
		int temp = index + (x * 6);
		return (state.powerups2 & (1 << temp)) != 0;
	}

	#endregion

	#region buying and saving stuff
	//buying stuff
	public bool buyAgility(int cost){
		int temp = getAgility ();
		if (temp != 6) {
			if (state.gold >= cost) {
				//enouugh money and remove it
				state.gold -= cost;
				unlockStuff (0, temp);

				//save progress
				Save ();
				return true;
			} else {
				//not allow return false
				return false;
			}
		} else {
			return false;
		}
	}

	public bool buyContainerPoints(int cost){
		int temp = getContainerPoints ();
		if (temp != 6) {
			if (state.gold >= cost) {
				//enouugh money and remove it
				state.gold -= cost;
				unlockStuff (1, temp);

				//save progress
				Save ();
				return true;
			} else {
				//not allow return false
				return false;
			}
		} else {
			return false;
		}
	}

	public bool buyCombo(int cost){
		int temp = getCombo ();
		if (temp != 6) {
			if (state.gold >= cost) {
				//enouugh money and remove it
				state.gold -= cost;
				unlockStuff (2, temp);

				//save progress
				Save ();
				return true;
			} else {
				//not allow return false
				return false;
			}
		} else {
			return false;
		}
	}

	public bool buyLife(int cost){
		int temp = getLife ();
		if (temp != 6) {
			if (state.gold >= cost) {
				//enouugh money and remove it
				state.gold -= cost;
				unlockStuff (3, temp);

				//save progress
				Save ();
				return true;
			} else {
				//not allow return false
				return false;
			}
		}else{
			return false;
		}
	}

	public bool buyBoat(int cost){
		int temp = getBoat();
		if (temp != 6) {
			if (state.gold >= cost) {
				//enouugh money and remove it
				state.gold -= cost;
				unlockStuff2 (0, temp);

				//save progress
				Save ();
				return true;
			} else {
				//not allow return false
				return false;
			}
		} else {
			return false;
		}
	}

	public bool buyAGV(int cost){
		int temp = getAGV ();
		if (temp != 6) {
			if (state.gold >= cost) {
				//enouugh money and remove it
				state.gold -= cost;
				unlockStuff2 (1, temp);

				//save progress
				Save ();
				return true;
			} else {
				//not allow return false
				return false;
			}
		} else {
			return false;
		}
	}

	public bool buyMagnet(int cost){
		int temp = getMagnet ();
		if (temp != 6) {
			if (state.gold >= cost) {
				//enouugh money and remove it
				state.gold -= cost;
				unlockStuff2 (2, temp);

				//save progress
				Save ();
				return true;
			} else {
				//not allow return false
				return false;
			}
		} else {
			return false;
		}
	}

	public bool buyPowerUp(int cost){
		int temp = getPowerups();
		if (temp != 6) {
			if (state.gold >= cost) {
				//enouugh money and remove it
				state.gold -= cost;
				unlockStuff2 (3, temp);

				//save progress
				Save ();
				return true;
			} else {
				//not allow return false
				return false;
			}
		}else{
			return false;
		}
	}


	public void unlockStuff(int index, int position){
		int temp = index * 6 + position;
		state.powerups |= 1 << temp;
	}
	public void unlockStuff2(int index, int position){
		int temp = index * 6 + position;
		state.powerups2 |= 1 << temp;
	}

	#endregion


	#region unused
//	//check if the color is owned 
//	public bool IsColorOwned(int index)
//	{
//		//check if bit is set, if so, color is owned 
//		return (state.colorOwned & (1 << index)) != 0;
//	}
//
//	//check if the trail is owned 
//	public bool IsTrailOwned(int index)
//	{
//		//check if bit is set, if so, color is owned 
//		return (state.trailOwned & (1 << index)) != 0;
//	}
//	//attempt buying a color, return true/false
//	public bool BuyColor(int index, int cost)
//	{
//		if (state.gold >= cost) {
//			//enouugh money and remove it
//			state.gold -= cost;
//			UnlockColor (index);
//
//			//save progress
//			Save ();
//			return true;
//		} else {
//			//not allow return false
//			return false;
//		}
//	}
//
//	//attempt buying a trail, return true/false
//	public bool BuyTrail(int index, int cost)
//	{
//		if (state.gold >= cost) {
//			//enouugh money and remove it
//			state.gold -= cost;
//			UnlockTrail (index);
//
//			//save progress
//			Save ();
//			return true;
//		} else {
//			//not allow return false
//			return false;
//		}
//	}
//
//	// unlock a color in the color owned int
//	public void UnlockColor(int index)
//	{
//		//toggle on the bit at index
//		state.colorOwned |= 1 << index;
//	}
//
//	// unlock a trail in the color owned int
//	public void UnlockTrail(int index)
//	{
//		//toggle on the bit at index
//		state.trailOwned |= 1 << index;
//	}
	#endregion

	//reset the whole save file
	public void resetSave(){
		PlayerPrefs.DeleteKey ("save");
	}

}
