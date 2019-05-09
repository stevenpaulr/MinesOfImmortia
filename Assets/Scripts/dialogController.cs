using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;


[System.Serializable]
public class dialogController : MonoBehaviour {


	private Dictionary<string, DialogEvent> dialogs;

	[SerializeField] private DialogEvent currentEvent;

	[SerializeField] private List<string> dialogHistory;

	private string fileName = "dialog";

	[SerializeField] private hero theHero;


	private int rand = 0;


	// Use this for initialization
	void Start () {



	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void Load(){
		dialogs = new Dictionary<string, DialogEvent> ();

		dialogHistory = new List<string> ();

		TextAsset file = Resources.Load (fileName) as TextAsset;

		string jsn = file.ToString();

		DialogEvent[] events = JsonHelper.FromJson<DialogEvent>(jsn);

		foreach (DialogEvent thisEvent in events){


			dialogs.Add (thisEvent.key, thisEvent);

		}

		currentEvent = dialogs ["starthere"];

	}


	public string currentKey(){
		return currentEvent.key;
	}


	//Replace the @X symbol with the rand variable
	public string currentText(){

		string thisText = currentEvent.text;

		if(thisText.Contains("@X")){
			thisText = thisText.Replace("@X", rand.ToString());
		}

		return thisText;

	}

	public string option1Text(){

		return currentEvent.option1Name;
	}

	public string option2Text(){

		return currentEvent.option2Name;
	}

	public string option3Text(){

		return currentEvent.option3Name;
	}

	public string option4Text(){
		return currentEvent.option4Name;
	}

	public int numOptions(){

		if (currentEvent.moveTriggered) {
			return 1;
		} else if (currentEvent.option4Key != "") {
			return 4;
		} else if (currentEvent.option3Key != "") {
			return 3;
		} else if (currentEvent.option2Key != "") {
			return 2;
		} else {
			return 1;
		}
	}

	public void goToOption1(){

		currentEvent = dialogs [currentEvent.option1Key];
		if(currentEvent.persistant == false)
			dialogHistory.Add (currentEvent.key);
		cleanCurrentEventOptions ();
	}

	public void goToOption2(){

		currentEvent = dialogs [currentEvent.option2Key];
		if(currentEvent.persistant == false)
			dialogHistory.Add (currentEvent.key);
		cleanCurrentEventOptions ();
	}


	public void goToOption3(){

		currentEvent = dialogs [currentEvent.option3Key];
		if(currentEvent.persistant == false)
			dialogHistory.Add (currentEvent.key);
		cleanCurrentEventOptions ();
	}

	public void goToOption4(){

		currentEvent = dialogs [currentEvent.option4Key];
		if(currentEvent.persistant == false)
			dialogHistory.Add (currentEvent.key);
		cleanCurrentEventOptions ();
	}


	private void cleanCurrentEventOptions(){

		//Removes options that have already been used if they are not persistant.
		//Moves other keys down to fill the gap.
		if (dialogHistory.Contains(currentEvent.option4Key)){
			currentEvent.option4Key = "";
			currentEvent.option4Name = "";
		}

		if (dialogHistory.Contains(currentEvent.option3Key)){
			currentEvent.option3Key = currentEvent.option4Key;
			currentEvent.option3Name = currentEvent.option4Name;
			currentEvent.option4Key = "";
			currentEvent.option4Name = "";
		}

		if (dialogHistory.Contains(currentEvent.option2Key)){
			currentEvent.option2Key = currentEvent.option3Key;
			currentEvent.option2Name = currentEvent.option3Name;
			currentEvent.option3Key = currentEvent.option4Key;
			currentEvent.option3Name = currentEvent.option4Name;
			currentEvent.option4Key = "";
			currentEvent.option4Name = "";
		}
					
		if (dialogHistory.Contains(currentEvent.option1Key)){
			currentEvent.option1Key = currentEvent.option2Key;
			currentEvent.option1Name = currentEvent.option2Name;
			currentEvent.option2Key = currentEvent.option3Key;
			currentEvent.option2Name = currentEvent.option3Name;
			currentEvent.option3Key = currentEvent.option4Key;
			currentEvent.option3Name = currentEvent.option4Name;
			currentEvent.option4Key = "";
			currentEvent.option4Name = "";
		}

		//Check and see if this is a point that hasn't been written yet.
		if(currentEvent.text == "" || currentEvent.text == "Double-click this passage to edit it."){
			currentEvent.text = "You've gotten to a point that hasn't been written.\n\nKey: " + currentEvent.key;
		}

		//If there are no options to go forward, add restart option
		if(currentEvent.option1Key == ""){
			currentEvent.option1Key = "end";
			currentEvent.option1Name = "Restart";
		}



	}

	public void clearHistory(){
		dialogHistory.Clear ();

		dialogs.Clear();

		TextAsset file = Resources.Load (fileName) as TextAsset;

		string jsn = file.ToString();

		DialogEvent[] events = JsonHelper.FromJson<DialogEvent>(jsn);

		foreach (DialogEvent thisEvent in events){


			dialogs.Add (thisEvent.key, thisEvent);

		}
	}

	public bool checkHistory(string eventName){
		if(dialogHistory.Contains(eventName)){
			return true;
		}

		return false;
	}


	public bool lastOne(){

		if (currentEvent.option1Key == "end")
			return true;
		else
			return false;

	}

	public bool hasImage(){
		if (currentEvent.image != "") {
			return true;
		} else {
			return false;
		}
	}

	public string imageName(){
		return currentEvent.image;
	}

	public void startOver(){

		currentEvent = dialogs ["starthere"];


	}

	public bool dieRoll(){
		if (currentEvent.moveTriggered == true) {
			return true;
		} else {
			return false;
		}
	}

	public string moveStat(){
		return currentEvent.moveStat;
	}


	public bool hasAction(){
		if (currentEvent.action != "") {
			return true;
		} else {
			return false;
		}
	}

	public string action(){
		return currentEvent.action;
	}

	public string getParameter(){
		return currentEvent.attribute;
	}

	public int getParameterInt(){
		return currentEvent.amount;
	}

	public void setRand(int newRand){
		rand = newRand;
	}


	public void weAreDead(){
		//Someone has informed me that the player is now dead.
		//We will switch the dialog to the appropriate one and move on.
		currentEvent = dialogs ["dead"];
	}

	public void goToError(){
		//For some reason, we broke the system and need to show an error screen and restart.
		currentEvent = dialogs ["error"];
	}

	public void loadRoom(string roomKey){
		//jump to a room directly
		if(dialogs.ContainsKey(roomKey)){
			currentEvent = dialogs [roomKey];
		}
		else{
			Debug.Log("That key doesn't exist: " + roomKey);
			goToError();
		}
	}

}
