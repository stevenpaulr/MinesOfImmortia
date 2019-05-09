using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class hero : MonoBehaviour {

	[SerializeField] private Dictionary<string, int> stats;
	[SerializeField] private List<string> attributes;
	[SerializeField] private int HP;
	[SerializeField] private int maxHP;
	[SerializeField] private int doomClock;

	[SerializeField] private bool plus1Forward = false;
	
	// Use this for initialization
	void Start () {

		//Create the list of attributes and add a couple
		attributes = new List<string> ();
		attributes.Add("Human");


		//Create the list of stats. Using a dictionary so that these can easily be replaced in other stories
		stats = new Dictionary<string, int> ();

		stats ["Strength"] = 0;
		stats ["Dexterity"] = 0;
		stats ["Constitution"] = 0;
		stats ["Intelligence"] = 0;
		stats ["Wisdom"] = 0;
		stats ["Charisma"] = 0;

		HP = 4;
		maxHP = HP;

		doomClock = 0;

	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void resetHero(){

		//Create the list of attributes and add a couple
		attributes = new List<string> ();
		attributes.Add("Human");


		//Create the list of stats. Using a dictionary so that these can easily be replaced in other stories
		stats = new Dictionary<string, int> ();

		stats ["Strength"] = 0;
		stats ["Dexterity"] = 0;
		stats ["Constitution"] = 0;
		stats ["Intelligence"] = 0;
		stats ["Wisdom"] = 0;
		stats ["Charisma"] = 0;

		HP = 4;
		maxHP = HP;

		doomClock = 0;
	}


	public void setClass(string className){
		if (className == "Fighter") {
			removeAttribute ("Wizard");
			addAttribute("Fighter");

			stats ["Strength"] = 2;
			stats ["Dexterity"] = 1;
			stats ["Constitution"] = 1;
			stats ["Intelligence"] = 0;
			stats ["Wisdom"] = -1;
			stats ["Charisma"] = 0;

			HP = 25;
		} else if (className == "Wizard") {
			removeAttribute ("Fighter");
			addAttribute("Wizard");

			stats ["Strength"] = -1;
			stats ["Dexterity"] = 1;
			stats ["Constitution"] = 0;
			stats ["Intelligence"] = 2;
			stats ["Wisdom"] = 1;
			stats ["Charisma"] = 0;

			HP = 15;

		}

		maxHP = HP;

	}

	public void loseHP (int amount){
		HP = HP - amount;

		if (HP < 0) {
			HP = 0;
		}

	}

	public void restoreHP(){
		HP = maxHP;
	}

	public void restoreHP(int amount){
		HP = HP + amount;
		if(HP > maxHP){
			HP = maxHP;
		}
	}

	public int getHP(){
		return HP;
	}

	public int getmaxHP(){
		return maxHP;
	}

	//return the value of a given stat
	public int getStat(string statName){

		return stats [statName];
	}


	//return if the hero has a certain attribute or not
	public bool hasAttribute(string attributeName){
		if (attributes.Contains (attributeName)) {
			return true;
		} else {
			return false;
		}

	}

	//give the hero a new attribute
	public bool addAttribute(string attributeName){

		//add the attribute and return if it was added or not (false means it was already there)
		if (attributes.Contains (attributeName)) {
			return false;
		} else {
			attributes.Add (attributeName);
			return true;
		}
	}


	//remove the attribute, or return false
	public bool removeAttribute(string attributeName){
		if (attributes.Contains (attributeName)) {
			attributes.Remove (attributeName);
			return true;
		} else {
			return false;
		}
	}

	public string getAttributes(){

		string attributeList = "";

		foreach (string thisattribute in attributes){
			attributeList = attributeList + thisattribute + " ";
		}

		return attributeList;

	}

	public bool hasPlus1Forward(){
		return plus1Forward;
	}

	public void getPlus1Forward(){
		plus1Forward = true;
	}

	public bool usePlus1Forward(){
		if (plus1Forward){
			plus1Forward = false;
			return true;
		} else {
			return false;
		}
	}


	public void increaseDoom(){
		doomClock = doomClock + 1;
	}

	public void increaseDoom(int amount){
		doomClock = doomClock + amount;
	}

	public int getDoom(){
		return doomClock;
	}
}
