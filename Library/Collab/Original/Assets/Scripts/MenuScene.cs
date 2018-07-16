using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MenuScene : MonoBehaviour {

	#region all the defined stuff
	public CanvasGroup fadeGroup;
	private float fadeInSpeed = 0.33f;

	public RectTransform menuContainer;
	public CanvasGroup menuCanvas;
	public CanvasGroup PlayButton;
	public CanvasGroup garageCanvas;
	public Transform garageTransform;
	public CanvasGroup settingsCanvas;
	public CanvasGroup helpCanvas;
	public CanvasGroup moneyDisplayCanvas;
	public GameObject AgilBuyButton;
	public GameObject ContainerBuyButton;
	public GameObject ComboBuyButton;
	public GameObject LivesBuyButton;
	public GameObject BoatBuyButton;
	public GameObject AGVBuyButton;
	public GameObject MagnetBuyButton;
	public GameObject PowerupBuyButton;

	public AudioSource[] Sounds;
	public AudioSource boughtSound;
	public AudioSource failSound;
	public AudioSource clickSound;
	public AudioSource playSound;
	public VolumeManager VolumeManager;

	private Vector3[] garagePage1position;

	public Sprite zeroBar;
	public Sprite oneBar;
	public Sprite twoBar;
	public Sprite threeBar;
	public Sprite fourBar;
	public Sprite fiveBar;
	public Sprite maxButton;
	public Sprite LeftSide;
	public Sprite RightSide;
	public GameObject lilDots;


	public GameObject AgilLevel;
	public GameObject ContainerLevel;
	public GameObject ComboLevel;
	public GameObject LiveLevel;
	public GameObject BoatLevel;
	public GameObject AGVLevel;
	public GameObject MagnetLevel;
	public GameObject PowerupLevel;

	//settings 
	public GameObject MusicSlider;
	public GameObject EffectSlider;



	public Text goldText;
	
	public Text colorBuySetsText;
	//time reference
	private float currentTime;
	private float previousTime;
	private float transitTime;
	public float wordTimer;


	private Vector3 desiredMenuPosition; 
	private int menuIndex;
	private int prevMenuIndex;
	private int garageIndex;
	private Vector3 newGarageMenuPosition;

	#endregion

	// Use this for initialization
	void Start () {
		//saveManager = gameManager.GetComponent<SaveManager> ();

		Debug.Log(Screen.width);
		wordTimer = 0;
		prevMenuIndex = 0;
		menuIndex = 0;
		previousTime = Time.time;
		settingsCanvas.alpha = 0;
		settingsCanvas.interactable = false;
		settingsCanvas.blocksRaycasts = false;

		Sounds = GetComponents<AudioSource> ();
		boughtSound = Sounds [0];
		failSound = Sounds [1];
		clickSound = Sounds [2];
		playSound = Sounds [3];
		//VolumeManager.InitVolume();

		garagePage1position = new Vector3[8];
		int i = 0;
		foreach(Transform t in garageTransform){
			if(i>3){
				Vector3 temp = t.position;
				t.position = new Vector3(Screen.width*1.5f,temp.y,temp.z);
			}
			i++;
		}



//		helpCanvas.interactable = false;
//		helpCanvas.blocksRaycasts = false;
		//temporurau
		//SaveManager.Instance.state.gold = 999;

		// tell our gold text how much he should display
		UpdateGoldText();

		//grab the only canvas group
		//fadeGroup = FindObjectOfType<CanvasGroup>();
		//start with white screen

		#region button and image initialisation
		fadeGroup.alpha = 1;
		InitButton (AgilBuyButton, SaveManager.Instance.getAgility());
		InitButton (ContainerBuyButton, SaveManager.Instance.getContainerPoints());
		InitButton (ComboBuyButton, SaveManager.Instance.getCombo());
		InitButton (LivesBuyButton, SaveManager.Instance.getLife());
		InitButton (BoatBuyButton, SaveManager.Instance.getBoat()); //update when SM updates function
		InitButton (AGVBuyButton, SaveManager.Instance.getAGV()); //update when SM updates function
		InitButton (MagnetBuyButton, SaveManager.Instance.getMagnet()); //update when SM updates function
		InitButton (PowerupBuyButton, SaveManager.Instance.getPowerups()); //update when SM updates function

		updateLevelImage (AgilLevel, SaveManager.Instance.getAgility ());
		updateLevelImage (ContainerLevel, SaveManager.Instance.getContainerPoints ());
		updateLevelImage (ComboLevel, SaveManager.Instance.getCombo ());
		updateLevelImage (LiveLevel, SaveManager.Instance.getLife ());
		updateLevelImage (BoatLevel, SaveManager.Instance.getBoat());
		updateLevelImage(AGVLevel, SaveManager.Instance.getAGV());
		updateLevelImage (MagnetLevel, SaveManager.Instance.getMagnet());
		updateLevelImage (PowerupLevel, SaveManager.Instance.getPowerups());

		#endregion

	}
	
	// Update is called once per frame
	void Update () {
		//GameObject.FindGameObjectWithTag ("Tester").GetComponent<AudioSource> ().volume = 0.2f;
		currentTime = Time.time;
		//Fade - in
		fadeGroup.alpha = 1 - Time.timeSinceLevelLoad * fadeInSpeed;

		garageMenu (currentTime - previousTime);
		blinkingWord (currentTime - previousTime);

		//shift garage menu left or right smoothly
		checkNewGarageMenu ();
		garageCanvas.transform.position = Vector3.Lerp (garageCanvas.transform.position, newGarageMenuPosition, 0.1f);

		//menu navigation smooth
		menuContainer.anchoredPosition3D = Vector3.Lerp(menuContainer.anchoredPosition3D,desiredMenuPosition,0.1f);
		previousTime = currentTime;


	}

	public void onSettingsClick(){
		Debug.Log ("Settings button clicked");
		clickSound.Play ();
		openSettings ();

	}

	public void openSettings(){
		//MusicSlider.GetComponentInChildren<Slider> ().value = SaveManager.Instance.getMusic ();
		//EffectSlider.GetComponentInChildren<Slider> ().value = SaveManager.Instance.getEffect ();
		settingsCanvas.alpha = 1;
		settingsCanvas.interactable = true;
		settingsCanvas.blocksRaycasts = true;
	}

	public void onSettingsBackClick(){
		Debug.Log ("transiting back");
		clickSound.Play ();
		settingsCanvas.interactable = false;
		settingsCanvas.blocksRaycasts = false;
		settingsCanvas.alpha = 0;
		SaveManager.Instance.saveMusic (MusicSlider.GetComponentInChildren<Slider> ().value);
		SaveManager.Instance.saveEffect (EffectSlider.GetComponentInChildren<Slider> ().value);
	}
		
//	public void onHelpClick(){
//		openHelp ();
//		Debug.Log ("Help button clicked");
//	}
//
//	public void openHelp(){
//		helpCanvas.interactable = true;
//		helpCanvas.blocksRaycasts = true;
//
//	}
//
//	public void closeHelp(){
//		helpCanvas.interactable = false;
//		helpCanvas.blocksRaycasts = false;
//
//	}

	private void InitButton(GameObject button, int level){ //to make sure the button displays the correct price based on level
		if (button == LivesBuyButton) {
			switch (level) {
			case 1:
				button.GetComponentInChildren<Text> ().text = "10000";
				break;
			case 2:
				button.GetComponentInChildren<Text> ().text = "50000";
				break;
			case 3:
				button.GetComponentInChildren<Text> ().text = "100000";
				break;
			case 4:
				button.GetComponentInChildren<Text> ().text = "";
				button.GetComponentInChildren<Image> ().sprite = maxButton;
				break;

			}
		} else {
			switch (level) {
			case 1:
				button.GetComponentInChildren<Text> ().text = "100";
				break;
			case 2:
				button.GetComponentInChildren<Text> ().text = "500";
				break;
			case 3:
				button.GetComponentInChildren<Text> ().text = "1500";
				break;
			case 4:
				button.GetComponentInChildren<Text> ().text = "5000";
				break;
			case 5:
				button.GetComponentInChildren<Text> ().text = "10000";
				break;
			case 6:
				button.GetComponentInChildren<Text> ().text = "";
				button.GetComponentInChildren<Image> ().sprite = maxButton;
				break;
			}
		}
	}




	//UI Image Update
	public void updateLevelImage(GameObject bar,int level){
		Debug.Log ("this is the current level " + level);
		if (level == 6) {
			bar.GetComponentInChildren<Image> ().sprite = fiveBar;
		} else if (level == 5) {
			bar.GetComponentInChildren<Image> ().sprite = fourBar;
		} else if (level == 4) {
			bar.GetComponentInChildren<Image> ().sprite = threeBar;
		} else if (level == 3) {
			bar.GetComponentInChildren<Image> ().sprite = twoBar;
		} else if (level == 2) {
			bar.GetComponentInChildren<Image> ().sprite = oneBar;
		} else {
			bar.GetComponentInChildren<Image> ().sprite = zeroBar;
		}
	}

	#region Listener for all the garage buying button the parsing is here
	public void BuyAgilityButton(){
		Debug.Log ("the level is " + SaveManager.Instance.getAgility());
		int cost = int.Parse (AgilBuyButton.GetComponentInChildren<Text> ().text);
		OnBuyAgility (cost);
		Debug.Log ("the cost is "+cost);
	}

	public void BuyContainerPointsButton(){
		Debug.Log ("the level is " + SaveManager.Instance.getContainerPoints());
		int cost = int.Parse (ContainerBuyButton.GetComponentInChildren<Text> ().text);
		OnBuyContainerPoints (cost);
		Debug.Log ("the cost is "+cost);
	}

	public void BuyComboButton(){
		int cost = int.Parse (ComboBuyButton.GetComponentInChildren<Text> ().text);
		OnBuyCombo (cost);
		Debug.Log ("the cost is "+cost);
	}

	public void BuyLifeButton(){
		int cost = int.Parse (LivesBuyButton.GetComponentInChildren<Text> ().text);
		OnBuyLife (cost);
		Debug.Log ("the cost is "+cost);
	}

	public void BuyBoatBonanzaButton(){
		int cost = int.Parse (BoatBuyButton.GetComponentInChildren<Text> ().text);
		OnBuyBoat (cost);
		Debug.Log ("the cost is "+cost);
	}

	public void BuyAGVButton(){
		int cost = int.Parse (AGVBuyButton.GetComponentInChildren<Text> ().text);
		OnBuyAGV (cost);
		Debug.Log ("the cost is "+cost);
	}

	public void BuyMagnetMadnessButton(){
		int cost = int.Parse (MagnetBuyButton.GetComponentInChildren<Text> ().text);
		OnBuyMagnet (cost);
		Debug.Log ("the cost is "+cost);
	}

	public void BuyPowerUpFrequencyButton(){
		int cost = int.Parse (PowerupBuyButton.GetComponentInChildren<Text> ().text);
		OnBuyPowerup (cost);
		Debug.Log ("the cost is "+cost);
	}
		
	#endregion

	#region functions for buying, embed into the click functions
	//for the following 4 functions, the parameter to input is the cost of what is being bought
	public void OnBuyAgility(int cost){
		Debug.Log ("buy agility");
		if (SaveManager.Instance.buyAgility (cost)) {
			UpdateGoldText ();
			InitButton (AgilBuyButton, SaveManager.Instance.getAgility ());
			updateLevelImage (AgilLevel, SaveManager.Instance.getAgility ());
			boughtSound.Play ();
		} else {
			Debug.Log ("not enough money");
			failSound.Play ();
		}
	}
	public void OnBuyContainerPoints(int cost){
		Debug.Log ("buy container points");
		if (SaveManager.Instance.buyContainerPoints(cost)) {
			UpdateGoldText ();
			InitButton (ContainerBuyButton, SaveManager.Instance.getContainerPoints ());
			updateLevelImage (ContainerLevel, SaveManager.Instance.getContainerPoints ());
			boughtSound.Play ();
		} else {
			Debug.Log ("not enough money");
			failSound.Play ();
		}
	}
	public void OnBuyCombo(int cost){
		Debug.Log ("buy combo");
		if (SaveManager.Instance.buyCombo (cost)) {
			UpdateGoldText ();
			InitButton (ComboBuyButton, SaveManager.Instance.getCombo ());
			updateLevelImage (ComboLevel, SaveManager.Instance.getCombo ());
			boughtSound.Play ();
		} else {
			Debug.Log ("not enough money");
			failSound.Play ();
		}
	}
	public void OnBuyLife(int cost){
		Debug.Log ("buy life");
		if (SaveManager.Instance.buyLife(cost)) {
			UpdateGoldText ();
			InitButton (LivesBuyButton, SaveManager.Instance.getLife ());
			updateLevelImage (LiveLevel, SaveManager.Instance.getLife ());
			boughtSound.Play ();
		} else {
			Debug.Log ("not enough money");
			failSound.Play ();
		}
	}

	public void OnBuyBoat(int cost){
		Debug.Log ("buy life");
		if (SaveManager.Instance.buyBoat(cost)) {
			UpdateGoldText ();
			InitButton (BoatBuyButton, SaveManager.Instance.getBoat());
			updateLevelImage (BoatLevel, SaveManager.Instance.getBoat ());
			boughtSound.Play ();
		} else {
			Debug.Log ("not enough money");
			failSound.Play ();
		}
	}

	public void OnBuyAGV(int cost){
		Debug.Log ("buy AGV");
		if (SaveManager.Instance.buyAGV(cost)) {
			UpdateGoldText ();
			InitButton (AGVBuyButton, SaveManager.Instance.getAGV());
			updateLevelImage (AGVLevel, SaveManager.Instance.getAGV ());
			boughtSound.Play ();
		} else {
			Debug.Log ("not enough money");
			failSound.Play ();
		}
	}

	public void OnBuyMagnet(int cost){
		Debug.Log ("buy MAgnet");
		if (SaveManager.Instance.buyMagnet(cost)) {
			UpdateGoldText ();
			InitButton (MagnetBuyButton, SaveManager.Instance.getMagnet());
			updateLevelImage (MagnetLevel, SaveManager.Instance.getMagnet ());
			boughtSound.Play ();
		} else {
			Debug.Log ("not enough money");
			failSound.Play ();
		}
	}

	public void OnBuyPowerup(int cost){
		Debug.Log ("buy Powerup");
		if (SaveManager.Instance.buyPowerUp(cost)) {
			UpdateGoldText ();
			InitButton (PowerupBuyButton, SaveManager.Instance.getPowerups());
			updateLevelImage (PowerupLevel, SaveManager.Instance.getPowerups ());
			boughtSound.Play ();
		} else {
			Debug.Log ("not enough money");
			failSound.Play ();
		}
	}
	#endregion

	#region Miscellaneous like blinking
	//to blink the button
	void blinkingWord(float deltatime){
		wordTimer += deltatime;
		if (wordTimer <= 0.8f) {
			PlayButton.alpha = 1-wordTimer;

		}
		if(wordTimer>=0.8f){
			PlayButton.alpha = wordTimer-0.8f;
		}
		if (wordTimer >= 2.8f) {
			wordTimer = 0;
		}
	}

	private void UpdateGoldText()
	{
		goldText.text = SaveManager.Instance.state.gold.ToString ();
	}
	#endregion
		
	#region all the fucking button
	//Buttons
	public void garageBackClick(){
		clickSound.Play ();
		Debug.Log ("transiting back");
		menuIndex = 0;
	}

	public void onPlayClick(){
		//menuIndex = 1;

		Debug.Log ("Play button clicked");
		playSound.PlayDelayed (0.25f);
		SceneManager.LoadScene ("MainGame");
	}

	public void onShopClick(){
		clickSound.Play ();
		menuIndex = 1;
		Debug.Log ("shop button clicked");
	}
	#endregion

	#region related to garage menu
	//to summon garage
	private void garageMenu(float deltaTime){
		if (menuIndex > prevMenuIndex) {
			transitTime += deltaTime;
			if (transitTime >= 0.5f) {
				menuCanvas.alpha = 0;
				transitTime = 0;
				prevMenuIndex = menuIndex;
				menuCanvas.interactable = false;
				menuCanvas.blocksRaycasts = false;
			} else {
				menuCanvas.alpha = 1 - transitTime * 2;
			}
		} else if (prevMenuIndex>menuIndex) {

			transitTime += deltaTime;
			if (transitTime >= 0.5f) {
				menuCanvas.alpha = 1;
				transitTime = 0;
				prevMenuIndex = menuIndex;
				menuCanvas.interactable = true;
				menuCanvas.blocksRaycasts = true;
			} else {
				Debug.Log ("appear");
				menuCanvas.alpha = transitTime * 2;
			}
		}
	}

	private void checkNewGarageMenu(){
		
		if (menuCanvas.alpha==0&&swipeManager.Instance.SwipeLeft && garageIndex==0 && (garageCanvas.transform.position - newGarageMenuPosition).magnitude<1) {
			Debug.Log ("Swiped Left");
			garageIndex = 1;
			newGarageMenuPosition = garageCanvas.transform.position + Vector3.left * (Screen.width);
			moneyDisplayCanvas.GetComponent<Animator>().SetTrigger ("Swipe");
			lilDots.GetComponentInChildren<Image> ().sprite = RightSide;
			Debug.Log(Screen.width);

		} else if (menuCanvas.alpha ==0&&swipeManager.Instance.SwipeRight && garageIndex==1 && (garageCanvas.transform.position - newGarageMenuPosition).magnitude<1) {
			Debug.Log ("swiped right");
			garageIndex = 0;
			newGarageMenuPosition = garageCanvas.transform.position + Vector3.right *( Screen.width);
			moneyDisplayCanvas.GetComponent<Animator>().SetTrigger ("Swipe");
			lilDots.GetComponentInChildren<Image> ().sprite = LeftSide;

		}
		moneyDisplayCanvas.GetComponent<Animator>().SetTrigger ("Normal");
	}
	#endregion

	#region Irrelevant stuff just ignore bruh
	//set the color of avatar

//	private void InitShop()
//	{
//		//just make sure we assigned the referencees
//		if (colorPanel == null || trailPanel == null) {
//			Debug.Log ("you did not assign the color/trail panel in the inspector");
//		}
//
//		//for every children transform under our color panel, find the button and add onclick.
//		int i = 0;
//		foreach (Transform t in colorPanel) {
//			int currentIndex = i;
//			Button b = t.GetComponent<Button> ();
//			b.onClick.AddListener (() => OnColorSelect (currentIndex));
//
//			//set color of image based on whether it is owned
//			Image img = t.GetComponent<Image>();
//			img.color = SaveManager.Instance.IsColorOwned (i) ? Color.white : new Color (0.7f, 0.7f, 0.7f);
//
//			i++;
//		}
//
//		//reset index
//		i = 0;
//		//do the same for trail panel
//		foreach (Transform t in trailPanel) {
//			int currentIndex = i;
//
//			Button b = t.GetComponent<Button> ();
//			b.onClick.AddListener (() => OnTrailSelect (currentIndex));
//
//			//set color of image based on whether it is owned
//			Image img = t.GetComponent<Image>();
//			img.color = SaveManager.Instance.IsTrailOwned (i) ? Color.white : new Color (0.7f, 0.7f, 0.7f);
//
//			i++;
//		}
//	}
//
//	private void InitLevel(){
//		if (levelPanel == null) {
//			Debug.Log ("you did not assign the level panel in the inspector");
//		}
//
//		//for every children transform under our level panel, find the button and add onclick.
//		int i = 0;
//		foreach (Transform t in levelPanel) {
//			int currentIndex = i;
//			Button b = t.GetComponent<Button> ();
//			b.onClick.AddListener (() => OnLevelSelect (currentIndex));
//			i++;
//		}
//
//	}
//
//	//navigation function. Navigation of vectors
//	private void NavigateTo(int MenuIndex){
//		switch (MenuIndex) {
//		//0 and default case is main menu
//		default:
//		case 0:
//			desiredMenuPosition = Vector3.zero;
//			break;
//		case 1:
//			desiredMenuPosition = Vector3.right * 800;
//			break;
//		case 2:
//			desiredMenuPosition = Vector3.left * 800;
//			break;
//		}
//	}
//	private void SetColor(int index){
//		//set active index
//		activeColorIndex = index;
//		SaveManager.Instance.state.activeColor = index;
//
//		//change color on player model
//
//		//change buy/set button text
//		//colorBuySetText.text = "current";
//
//		//remember
//		SaveManager.Instance.Save();
//	}
//
//	private void SetTrail(int index){
//		//set tje active index
//		activeTrailIndex = index;
//
//		SaveManager.Instance.state.activeTrail = index;
//
//		//change the trail on the player model
//
//		//change buy/set button text
//		trailBuySetText.text = "current";
//
//		//remember
//		SaveManager.Instance.Save();
//
//	}
//
//	public void onBackClick(){
//		NavigateTo (0);
//		Debug.Log ("back button has been clicked");
//	}
//
//	private void OnColorSelect(int currentIndex){
//		Debug.Log ("selecting color button:"+ currentIndex);
//
//		//if the button clicked already selected, exit
//		if (selectedColorIndex == currentIndex) {
//			return;
//		}
//
//		//make icon bigger
//		colorPanel.GetChild(currentIndex).GetComponent<RectTransform>().localScale = Vector3.one* 1.125f;
//
//		//make the prevuius one normal
//		colorPanel.GetChild(selectedColorIndex).GetComponent<RectTransform>().localScale = Vector3.one;
//
//
//		//set the selected color 
//		selectedColorIndex = currentIndex;
//
//		//change the content of buy/set button, depending on the state of the color
//		if (SaveManager.Instance.IsColorOwned (currentIndex)) {
//			//color is owned
//			//is it already our current color
//			if (activeColorIndex == currentIndex) {
//				//colorBuySetText.text = "current";
//			} else {
//				//colorBuySetText.text = "Select";
//			}
//		}
//		else
//		{
//			//color isnt owned
//			//colorBuySetText.text = "Buy: " + colorCost[currentIndex].ToString();
//		}
//	}
//
//	private void OnTrailSelect(int currentIndex){
//		Debug.Log ("selecting trail button:"+ currentIndex);
//
//		//if the button clicked already selected, exit
//		if (selectedTrailIndex == currentIndex) {
//			return;
//		}
//
//		//make icon bigger
//		trailPanel.GetChild(currentIndex).GetComponent<RectTransform>().localScale = Vector3.one* 1.125f;
//
//		//make the prevuius one normal
//		trailPanel.GetChild(selectedTrailIndex).GetComponent<RectTransform>().localScale = Vector3.one;
//
//		//set the selected trail
//		selectedTrailIndex = currentIndex;
//
//		//change the content of buy/set button, depending on the state of the trail
//		if (SaveManager.Instance.IsTrailOwned (currentIndex)) {
//			//trail is owned
//			//is it already our current trail
//			if (activeTrailIndex == currentIndex) {
//				trailBuySetText.text = "current";
//			} else {
//				trailBuySetText.text = "Select";
//			}
//		}
//		else
//		{
//			//trail isnt owned
//			trailBuySetText.text = "Buy: " + trailCost[currentIndex].ToString();
//		}
//	}
//
//	private void OnLevelSelect(int currentIndex){
//		
//		Debug.Log ("selecting level:" + currentIndex);
//		SceneManager.LoadScene ("MainGame");
//	}
//
//	public void OnColorBuySet(){
//		Debug.Log ("buy set color");
//
//		//is the selected color owned
//		if (SaveManager.Instance.IsColorOwned (selectedColorIndex)) {
//			//set the color!
//			SetColor (selectedColorIndex);
//		}else{
//			//attempt to buy the color
//			if(SaveManager.Instance.BuyColor(selectedColorIndex,colorCost[selectedColorIndex]))
//			{
//				//success
//				SetColor(selectedColorIndex);
//
//				//change color of the button
//				colorPanel.GetChild(selectedColorIndex).GetComponent<Image>().color = Color.white;
//
//				//update the gold text
//				UpdateGoldText();
//			}
//			else
//			{
//				//do not have enough gold
//				//play sound
//				Debug.Log("not enough gold");
//			}
//		}
//
//	}
//
//	public void OnTrailBuySet(){
//		Debug.Log ("buy set trail");
//
//		//is the selected trail owned
//		if (SaveManager.Instance.IsTrailOwned (selectedTrailIndex)) {
//			//set the trail!
//			SetTrail (selectedTrailIndex);
//		}else{
//			//attempt to buy the trail
//			if(SaveManager.Instance.BuyTrail(selectedTrailIndex,trailCost[selectedTrailIndex]))
//			{
//				//success
//				SetTrail(selectedTrailIndex);
//
//				//change color of the button
//				trailPanel.GetChild(selectedTrailIndex).GetComponent<Image>().color = Color.white;
//
//				//update the gold text
//				UpdateGoldText();
//			}
//			else
//			{
//				//do not have enough gold
//				//play sound
//				Debug.Log("not enough gold");
//			}
//		}
//
//	}
//	public Text trailBuySetText
//	private int[] colorCost = new int[]{0,5,5,5,10,10,10,10,10,2};
//	private int[] trailCost = new int[]{0,5,5,5,10,10,10,10,10,2};
//	private int selectedColorIndex;
//	private int selectedTrailIndex;
//	private int activeColorIndex;
//	private int activeTrailIndex;
//	public Transform colorPanel;
//	public Transform levelPanel;
//	public Transform trailPanel;
//	//make items bigger for selected
//	colorPanel.GetChild(SaveManager.Instance.state.activeColor).GetComponent<RectTransform>().localScale = Vector3.one * 1.125f;
//	trailPanel.GetChild(SaveManager.Instance.state.activeTrail).GetComponent<RectTransform>().localScale = Vector3.one * 1.125f;
	#endregion
}
