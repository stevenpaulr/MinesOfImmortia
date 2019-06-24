using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;



public class controllerScript : MonoBehaviour {

	//Prefabs for the different possibilities of boxes to start with
	public GameObject textBoxPrefab;
	public GameObject textBox2Prefab;
	public GameObject textBox3Prefab;
	public GameObject textBox4Prefab;
	public GameObject imageBoxPrefab;


	public AudioSource clickSound;

	//Prefab for dice
	public GameObject dice;

	//reference to the general canvas
	public Canvas myCanvas;
	public RectTransform textPanel;

	//Dialog controller actually does all the dialog interactions
	public dialogController theDialogController;

	//UI controller controls UI pieces that are not the main game text
	public uiControllerScript theUIController;
	public saveController theSaveController;

	//reference to the hero
	[SerializeField] private hero theHero;

	public bool gameActive = false;


	//Size of text (resizable later)
	public int fontSize = 52;

	//These distances are used when scrolling
	float distanceToMove = 0.0f;
	float distancePerClick = 0.0f;

	//These are speeds for animations
	float scrollSpeed = 4.0f; // time to scroll = 1/X
	float typingSpeed = 0.01f; //time between keystrokes
	float fadeSpeed = 0.25f; //time in seconds to fade in

	//Keeping track of the top of the screen so we know when boxes go past it
	float topOfScreen;
	//We need to know how much to scale items to fit all devices
	float scale;

	//References to the objects on the screen
	System.Collections.Generic.List<GameObject> textBoxes;
	GameObject lastTextBox;
	GameObject lastImageBox;
	Text animatingText;

	//Using this so animation only happens once per click
	bool needToAnimateLastBox = false;

	//use this so we only end the dieroll once
	bool needtoadddice = false;

	bool needToClickSound = true;

	//Tracking where the dice spawn
	GameObject spawnPoint = null;

	//string to add to the beginning of a text if a die was rolled.
	string dieRollText = "";


	static string mask = "<color=#00000000>";
	static string unmask = "</color>";


	// Use this for initialization
	void Start () {
		//Create an empty textboxes list
		textBoxes = new System.Collections.Generic.List<GameObject>();

		//Dialog controller loads all of the dialog from the JSON file
		theDialogController.Load ();

		//getting the scale of the canvas so that we can use it for later math
//		CanvasScaler canvasScaler = myCanvas.GetComponent<CanvasScaler>();

		scale = myCanvas.transform.GetComponent<RectTransform> ().localScale.y;

		//Get the top of the screen
		topOfScreen = (myCanvas.transform.GetComponent<RectTransform> ().rect.height);


		//Start the game in a co-routine
		//StartCoroutine(beginGame()); //the menu will do this now
	}

	//This will begin playing the game
	IEnumerator beginGame(){

		//First we wait a moment to make sure everything else is set up
		yield return new WaitForSeconds (0.5f);


		//Now create the first text box
		createTextBox ();


	}


	// Update is called once per frame
	void Update () {

		//If we need to move some textboxes.
		if (distanceToMove > 0) {

			//A list of boxes that needs to get removed from the general list
			System.Collections.Generic.List<GameObject> toRemove = new System.Collections.Generic.List<GameObject>();

			//Loop through every textbox
			foreach (GameObject thisTextBox in textBoxes){

				//Change the Y value and move it up the screen
				RectTransform rt = thisTextBox.GetComponent<RectTransform> ();

				if (distanceToMove < distancePerClick * Time.deltaTime * scrollSpeed) {
					rt.position = new Vector3 (rt.position.x, rt.position.y + (distanceToMove) * scale, rt.position.z);
				} else {
					rt.position = new Vector3 (rt.position.x, rt.position.y + (distancePerClick * Time.deltaTime * scrollSpeed) * scale, rt.position.z);
				}

				//if it's above the top of the screen, destroy it and add it to the list of boxes to be removed
				if (rt.position.y - rt.rect.height > (topOfScreen)*scale) {
					toRemove.Add (thisTextBox);
					Destroy (thisTextBox);
				}
					
			}

			//Remove all the boxes in the toRemove list from the main list. 
			foreach (GameObject thisTextBox in toRemove) {
				textBoxes.Remove (thisTextBox);
			}

			//Reduce the amount of distance we need to move by the amount that we did move.
			distanceToMove -= distancePerClick * Time.deltaTime * scrollSpeed;


		//If we are not moving
		} else {
			//If the last box needs to be animated, start it the animation and flip the bool so that it only happens once
			if (needToAnimateLastBox == true) {
				needToAnimateLastBox = false;

				StartCoroutine (animateLastBox ());
			}

			//In case the distanceToMove ends up negative, set it back to 0
			distanceToMove = 0;
		}

		//If there are dice on the table
		if (Dice.Count ("") > 0) {


			//If they're done rolling and we haven't started adding them up yet
			if (Dice.rolling != true && needtoadddice) {

				//Just keep going if there is a ?
				if (!Dice.AsString ("").Contains ("?")) {
					//Only add them up once
					needtoadddice = false;

					//start the coroutine to add the dice and display results
					StartCoroutine (dieRollEnd ());
				} else {
					
					Debug.Log("Keep Rolling " + Dice.AsString(""));
				}
			}

		}
	}

	void checkAction(){
		if (theDialogController.hasAction()) {
			Debug.Log (theDialogController.action ());
			performAction (theDialogController.action());
		}
	}

	//Takes the action that is passed from the dialog controller and acts on it.
	//This function will change a LOT based on each individual story.
	//Typically these are things that would be considered things that the GM would do in a standard RPG
	void performAction(string action){


		//store the current dialog key to see if it gets changed
		string oldDialog = theDialogController.currentKey();

		switch (action) {
		case "imafighter":
			theHero.setClass ("Fighter");
			theUIController.showHP();
			break;
		
		case "imawizard":
			theHero.setClass ("Wizard");
			theUIController.showHP();
			break;

		case "losehp":
			//Randomizing the loss in hitpoints
			//There can be a small, medium, or large hit
			//based on the amount parameter 1 = small, 2 = medium, 3 = large
			int rand = 0;
			if(theDialogController.getParameterInt() <= 1 ){ //Ideally 1, small hit
				rand = Random.Range(1,5);
			} else if (theDialogController.getParameterInt() == 2){ //2 medium hit
				rand = Random.Range(5,9);
			} else { //Ideally 3, large hit
				rand = Random.Range(9,13);
			}

			theDialogController.setRand(rand);

			theHero.loseHP(rand);
			break;

		case "longrest":
			theHero.restoreHP();
			theHero.removeAttribute("Exhausted");
			break;

		case "shortrest":
			theHero.restoreHP(8);
			theHero.removeAttribute("Exhausted");
			break;

		case "restoreHP":
			if(theDialogController.getParameterInt() > 0){
				theHero.restoreHP(theDialogController.getParameterInt());
			} else {
				theHero.restoreHP();

			}
			break;


		case "clear":
			for (int i = textBoxes.Count - 1; i >= 0; i--) {
				GameObject thisTextBox = textBoxes [i];
				textBoxes.Remove (thisTextBox);
				Destroy (thisTextBox);
			}
			break;


		//Get the number of options from the current room, and then randomly go to one of them.
		//This should have an equal distribution, but that is not guaranteed.
		//This case needs to remain in tact if we do a different game
		case "randomroom":
			Debug.Log("THERE ARE " + theDialogController.numOptions() + " Rooms!");

			int roomnum = Random.Range(0,theDialogController.numOptions());
			if(roomnum == 0){
				theDialogController.goToOption1();
			} else if (roomnum == 1) {
				theDialogController.goToOption2();
			} else if ( roomnum == 2 ) {
				theDialogController.goToOption3();
			} else if (roomnum == 3 ) {
				theDialogController.goToOption4();
			} else {
				Debug.Log("random room function isn't working right. Random Number is " + roomnum);
				theDialogController.goToError();
			}

			break;

		case "doom":
			if(theDialogController.getParameterInt() == 0)
				theHero.increaseDoom ();
			else
				theHero.increaseDoom(theDialogController.getParameterInt());
			break;

		case "checkDoom":
			if(theHero.getDoom() < theDialogController.getParameterInt()){
				theDialogController.goToOption1();
			} else {
				theDialogController.goToOption2();
			}
			break;

		case "displayDoom":
			//Set the random number holder in dialog controller to the doom level
			//this will automatically replace @X in the dialog
			theDialogController.setRand(theHero.getDoom());

			break;

		case "+1Forward":
			theHero.getPlus1Forward();
			break;

		case "-1Forward":
			theHero.getMinus1Forward();
			break;

		case "getPrepared":
			theHero.addAttribute ("Well-Prepared");
			break;

		case "checkPrepared":
			//If the attribute needed is there, do option 1
			if(theHero.hasAttribute("Well-Prepared")){
				theDialogController.goToOption1();
			} else {
				theDialogController.goToOption2();
			}
			break;

		case "checkClass":
			//If wizard, take option 1, if fighter option 2, if something else, option 3
			if(theHero.hasAttribute("Wizard")){
				theDialogController.goToOption1();
			} else if(theHero.hasAttribute("Fighter")){
				theDialogController.goToOption2();
			}else {
				theDialogController.goToError();
			}
			break;

		case "addAttribute":
			if(theHero.hasAttribute(theDialogController.getParameter())){
				//do nothing
			} else {
				theHero.addAttribute(theDialogController.getParameter());
			}
			break;


		case "checkAttribute":
			//If attribute exists, take option 1, else option 2
			if(theHero.hasAttribute(theDialogController.getParameter())){
				theDialogController.goToOption1();
			} else {
				theDialogController.goToOption2();
			}

			break;

		case "removeAttribute":
			if(theHero.hasAttribute(theDialogController.getParameter())){
				theHero.removeAttribute(theDialogController.getParameter());
			} else {
				//do nothing 
			}
			break;

		case "checkHistory":
			//If item is in the history, take option 1, else option 2
			if(theDialogController.checkHistory(theDialogController.getParameter())){
				theDialogController.goToOption1();
			} else {
				theDialogController.goToOption2();
			}

			break;


		default:
			Debug.Log ("Perform action not found " + action);
			break;
		}
			

		if(oldDialog != theDialogController.currentKey()){
			//We changed dialog, and the new one might have an action. 
			checkAction();
		}


		//After any actions have occured, update the UI to reflect any changes.
		theUIController.refresh();

	}



	//This will create a new textbox, set it up the way it needs to be set and pull the data from the Dialog Controller
	void createTextBox(){

		//get reference variables for a new textbox and imagebox
		GameObject textbox;
		GameObject imagebox;

		//preferred height of the box. This allows the box to get bigger or smaller depending on amount of text or image size
		float prefHeight = 0;

		//Need to keep track of the size of the image box if one is needed
		float imageBoxSpace = 0;

		//If we need an image
		if (theDialogController.hasImage()) {

			//Create an image box, and set it's parent
			imagebox = GameObject.Instantiate (imageBoxPrefab) as GameObject;
			imagebox.transform.SetParent (textPanel.transform, false);

			//get a reference to the actual image of the panel, and set it to the needed image
			Image img = imagebox.GetComponent<Image> (); 
			img.sprite = Resources.Load<Sprite> (theDialogController.imageName ());

			//Getting the height of the image so that we can resize other things accordingly
			float ratio = img.sprite.textureRect.size.y / img.sprite.textureRect.size.x;


			//set the image's Rect Transform to the right height based on the prefheight, and put it below the bottom of the screen
			RectTransform irt = imagebox.GetComponent<RectTransform> ();

			prefHeight = ratio * irt.rect.width;

			irt.SetSizeWithCurrentAnchors (RectTransform.Axis.Vertical, prefHeight);

			//place this box below the bottom of the screen
			irt.position = new Vector3 (irt.position.x, -1 * (topOfScreen/2) * scale, irt.position.z);

//			rt.position = new Vector3 (rt.position.x, -1 * (topOfScreen/2 + imageBoxSpace) * scale, rt.position.z);

			//This will be used to place the dialog box beneath the image box.
			imageBoxSpace = prefHeight + 20f;

			//add the imagebox to the list of boxes so that it can be animated
			textBoxes.Add (imagebox);

			//Keep track of image box for animation
			lastImageBox = imagebox;
			lastImageBox.GetComponent<CanvasGroup>().alpha = 0;


		}


		//Determine which of the prefabs we need to use and set up the box using the correct one
		//If it has a die roll, it'll have 3 options, but we act like there's only one.
		if (theDialogController.numOptions () == 1 || theDialogController.dieRoll ()) {
			textbox = GameObject.Instantiate (textBoxPrefab) as GameObject;
		} else if (theDialogController.numOptions () == 2) {
			textbox = GameObject.Instantiate (textBox2Prefab) as GameObject;
		} else if (theDialogController.numOptions () == 3) {
			textbox = GameObject.Instantiate (textBox3Prefab) as GameObject;
		} else  {
			textbox = GameObject.Instantiate (textBox4Prefab) as GameObject;
		}			
		//set the parent to the canvas
		textbox.transform.SetParent (textPanel.transform, false);

		//get an array of all the buttons
		Button[] Xs = textbox.GetComponentsInChildren <Button> ();

		//Loop through the buttons and set the actions for clicking.
		foreach (Button X in Xs) {


			if (theDialogController.dieRoll ()) {
				X.onClick.AddListener (() => this.dieRoll ());
			} else if(X.tag == "option1")
				X.onClick.AddListener (() => this.buttonClick (1));
			else if (X.tag == "option2")
				X.onClick.AddListener (() => this.buttonClick (2));
			else if (X.tag == "option3")
				X.onClick.AddListener (() => this.buttonClick (3));
			else if (X.tag == "option4")
				X.onClick.AddListener (() => this.buttonClick (4));

			//while we are looking at this button, we're going to disable it and make it invisible
			X.enabled = false;
			X.GetComponent<CanvasGroup> ().alpha = 0;
		}

		//get an array of all textboxes and loop through them
		Text[] Zs = textbox.GetComponentsInChildren <Text> ();
		foreach (Text Z in Zs) {

			Z.fontSize = fontSize;
			Z.resizeTextMaxSize = fontSize;

			//If it's the main text, we need to set it's text and get the height from it, and keep track of it for animating
			if (Z.tag == "mainText") {
				Z.text = dieRollText + theDialogController.currentText ();

				prefHeight = Z.preferredHeight;

				animatingText = Z;


			//For all other texts, we just need to set their text 
			}else if (Z.tag == "option1") {
				Z.text = theDialogController.option1Text ();
			}else if (Z.tag == "option2") {
				Z.text = theDialogController.option2Text ();
			}else if (Z.tag == "option3") {
				Z.text = theDialogController.option3Text ();
			}else if (Z.tag == "option4") {
				Z.text = theDialogController.option4Text ();
			}
		}

		//prefheight of text scaled down + space above and between + 50 for each button
		float boxHeight = prefHeight * 0.9f + 15.0f + (100.0f * theDialogController.numOptions ());

		//Get the rect transform for the box, and set it's size based on the prefheight value and number of options that it has.
		RectTransform rt = textbox.GetComponent<RectTransform> ();
		rt.SetSizeWithCurrentAnchors (RectTransform.Axis.Vertical, boxHeight);

		//place this box below the bottom of the screen (beneath the image box if it exists)
		//textbox.transform.position = new Vector3 (textbox.transform.position.x, -1 *(textbox.GetComponent<RectTransform> ().rect.height + imageBoxSpace) - topOfScreen/2, textbox.transform.position.z);
		rt.position = new Vector3 (rt.position.x, -1 * (topOfScreen/2 + imageBoxSpace) * scale, rt.position.z);


		//add this box to the list of textboxes
		textBoxes.Add (textbox);

		//set the distance that we are going to have to scroll to bring this box into view
		distancePerClick = (boxHeight + (20.0f));

		//keep a reference to this textbox for later
		lastTextBox = textbox;

		//Make it invisible so that we can fade it in later
		lastTextBox.GetComponent<CanvasGroup>().alpha = 0;

		//add to the total distance we need to move
		distanceToMove = distancePerClick + imageBoxSpace;

		//set the bool so that we animate. This should happen last!
		needToAnimateLastBox = true;



	}

	IEnumerator animateLastBox(){

		//clear the text for animation to happen
		animatingText.text = "";

		//Fade in
		float t = 0;
		while (t < fadeSpeed) {
			t += Time.deltaTime;
			lastTextBox.GetComponent<CanvasGroup> ().alpha = t/fadeSpeed;
			if (theDialogController.hasImage ()) {
				lastImageBox.GetComponent<CanvasGroup> ().alpha = t/fadeSpeed;
			}

			yield return null;
		}


		//remove any dice that may be showing
		Dice.Clear ();


		//type text
		string currentText = dieRollText + theDialogController.currentText();


		animatingText.text = mask + currentText + unmask;

		for (int i = 0; i < currentText.Length + 1; i++) {

			if(Input.touchCount > 0 || Input.GetMouseButtonDown(0)){
				break;
			}

			animatingText.text = currentText.Substring (0, i) + mask + currentText.Substring(i) + unmask;
			yield return new WaitForSecondsRealtime (typingSpeed);
		}

		//Put in all the text incase some was skipped
		animatingText.text = currentText;


		//fade in the buttons
		t = 0;
		while (t < fadeSpeed) {
			t += Time.deltaTime;

			Button[] Xs = lastTextBox.GetComponentsInChildren <Button> ();
			foreach (Button X in Xs) {
				X.GetComponent<CanvasGroup> ().alpha = t/fadeSpeed;

				if (t >= fadeSpeed) {
					X.enabled = true;
				}
			}

			yield return null;
		}
			
		//done animating, can clear the string
		dieRollText = "";

	}


	//Use this to inject dice rolling items
	public void dieRoll(){
		//Before anything else happens, go ahead and play the click sound
		playClick();

		needToClickSound = false;
		
		Button[] Xs = lastTextBox.GetComponentsInChildren <Button> ();

		//Loop through the buttons and mark clicked and disable.
		foreach (Button X in Xs) {

			X.GetComponent<CanvasGroup> ().alpha = 0.75f;
			X.enabled = false;
		}


		//Get the spawnpoint for the dice
		spawnPoint = GameObject.Find ("spawnPoint");


		//Roll 2 dice
		Dice.Roll ("2d6", "d6-red", spawnPoint.transform.position, Force());

		//We'll need to add them up
		needtoadddice = true;

	}

	private Vector3 Force()
	{
		Vector3 rollTarget = new Vector3(0,0,0) + new Vector3(200 + 700 * Random.value, 50F + 400 * Random.value, -200 - 300 * Random.value);
		Vector3 force = Vector3.Lerp (spawnPoint.transform.position, rollTarget, 1).normalized * (-35 - Random.value * 2000);
		force = force + new Vector3 (0, 6000, 0);

		Debug.Log("Force : " + force.x + " " + force.y + " " + force.z);

		return force;
	}


	IEnumerator dieRollEnd() {

		//First we wait a moment to see the finished dice
		yield return new WaitForSeconds (0.50f);


		Debug.Log (Dice.AsString (""));
		//Roll 2D6
		int rand = Dice.Value("");

		//get the bonus
		int bonus = theHero.getStat(theDialogController.moveStat());

		//add them up
		int total = bonus + rand;

		Debug.Log ("The stat is: " + theDialogController.moveStat () + ": " + bonus);


		//Get the correct grammer started
		if (rand == 8 || rand == 11) {
			dieRollText = "You rolled an ";
		} else {
			dieRollText = "You rolled a ";
		}

		//add the results and the stat info to the string
		if(theHero.usePlus1Forward()){
			total = total + 1;
			dieRollText += rand + " + " + bonus + "(" + theDialogController.moveStat() + ") plus 1 forward from before for a total of "  + total;
		} else if(theHero.useMinus1Forward()){
			total = total - 1;
			dieRollText += rand + " + " + bonus + "(" + theDialogController.moveStat() + ") minus 1 forward from before for a total of "  + total;
		} else {
			dieRollText += rand + " + " + bonus + "(" + theDialogController.moveStat() + ") for a total of "  + total;
		}

		//If statements based on the results
		if (total < 7) {
			dieRollText += ". (0-6)\n\n";
			buttonClick (1);
		} else if (total < 10) {
			dieRollText += ". (7-9)\n\n";
			buttonClick (2);
		} else {
			dieRollText += ". (10+)\n\n";
			buttonClick (3);
		}
			
	}


	public void buttonClick(int option){

		//Before anything else happens, go ahead and play the click sound
		if(needToClickSound)
			playClick();

		//set it true for next time around
		needToClickSound = true;


		Button[] Xs = lastTextBox.GetComponentsInChildren <Button> ();

		//Loop through the buttons and make them not-clickable.
		foreach (Button X in Xs) {

			if(theDialogController.dieRoll()){ //If it's a die roll, mark clicked
				X.GetComponent<CanvasGroup> ().alpha = 0.75f;
			} else if (X.tag == "option1") {
				if (option != 1) {
					X.GetComponent<CanvasGroup> ().alpha = 0.25f;
				} else {
					X.GetComponent<CanvasGroup> ().alpha = 0.75f;
				}
			} else if (X.tag == "option2") {
				if (option != 2) {
					X.GetComponent<CanvasGroup> ().alpha = 0.25f;
				} else {
					X.GetComponent<CanvasGroup> ().alpha = 0.75f;
				}
			} else if (X.tag == "option3") {
				if (option != 3) {
					X.GetComponent<CanvasGroup> ().alpha = 0.25f;
				} else {
					X.GetComponent<CanvasGroup> ().alpha = 0.75f;
				}
			} else if (X.tag == "option4") {
				if (option != 4) {
					X.GetComponent<CanvasGroup> ().alpha = 0.25f;
				} else {
					X.GetComponent<CanvasGroup> ().alpha = 0.75f;
				}
			}

			//while we are looking at this button, we're going to disable it
			X.enabled = false;
		}



		//if it's the last item from the dialog controller, start the game over
		if (theDialogController.lastOne () == true) {
			theDialogController.startOver ();
			theDialogController.clearHistory ();
			theHero.resetHero();
			theUIController.hideHP();
			theUIController.refresh();

			for (int i = textBoxes.Count - 1; i >= 0; i--) {
				GameObject thisTextBox = textBoxes [i];
				textBoxes.Remove (thisTextBox);
				Destroy (thisTextBox);
			}
		

		//We should probably check and see if we died.
		} else if(theHero.getHP() <= 0) {
			//Yup, we are dead.
			//Better tell that to the dialog controller so it can switch to that dialog
			theDialogController.weAreDead();

		//If we are not starting the game over, go to the correct option
		} else if (option == 1) {
			theDialogController.goToOption1 ();
		} else if (option == 2) {
			theDialogController.goToOption2 ();
		} else if (option == 3) {
			theDialogController.goToOption3 ();
		} else if (option == 4) {
			theDialogController.goToOption4 ();
		}

		

		//If we need to do some sort of extra action, go do it
		checkAction();


		//save the game here
		theSaveController.saveGame();

		//create the next text box
		createTextBox ();

	}

	public void reloadRoom(){



		//Clear dialogs
		for (int i = textBoxes.Count - 1; i >= 0; i--) {
			GameObject thisTextBox = textBoxes [i];
			textBoxes.Remove (thisTextBox);
			Destroy (thisTextBox);
		}

		//If we need to do some sort of extra action, go do it
		checkAction();

	
		//create the next text box
		createTextBox ();

	}



	public void playClick(){

		clickSound.Play();
	}

}
