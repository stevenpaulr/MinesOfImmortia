using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class DialogEvent {

//        "key":"",
//        "text":"",
//        "option1Name":"",
//        "option1Key":"",
//        "option2Name":"",
//        "option2Key":"",
//        "option3Name":"",
//        "option3Key":""

	public string text;
	public string key;

	public string option1Name = "";
	public string option1Key = "";

	public string option2Name = "";
	public string option2Key = "";

	public string option3Name = "";
	public string option3Key = "";

	public string option4Name = "";
	public string option4Key = "";

	public bool moveTriggered = false;

	public string moveStat = "";

	public string image = "";

	public string action = "";

	public bool persistant = false;

	public string attribute;

	public int amount;

	public DialogEvent(){
	}

	//Only 1 option, don't need anything but the key for the next dialog event
	public DialogEvent(string newText, string newOption1Key){
	
		text = newText;
		option1Key = newOption1Key;
	
	}

	//Only 1 option, don't need anything but the key for the next dialog event
	public DialogEvent(string newText, string newOption1Name, string newOption1Key){

		text = newText;
		option1Name = newOption1Name;
		option1Key = newOption1Key;

	}

	//2 options
	public DialogEvent(string newText, string newOption1Name, string newOption1Key, string newOption2Name, string newOption2Key){

		text = newText;
		option1Name = newOption1Name;
		option1Key = newOption1Key;
		option2Name = newOption2Name;
		option2Key = newOption2Key;

	}

	//3 options
	public DialogEvent(string newText, string newOption1Key, string newOption2Key, string newOption3Key){

		text = newText;
		option1Key = newOption1Key;
		option2Key = newOption2Key;
		option3Key = newOption3Key;
		moveTriggered = false;

	}
}
