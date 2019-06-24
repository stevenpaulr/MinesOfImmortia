using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;


public class uiControllerScript : MonoBehaviour {

	public Canvas myCanvas;
	public RectTransform statsPanel;

	public RectTransform hpPanel;

	public Text hpText;
	

	public Button menuButton;

	public Button continueButton;


	public Sprite hamburgerOpenSprite;
	public Sprite hamburgerCloseSprite;

	public hero theHero;
	public dialogController theDialogController;
	public controllerScript theGameController;
	public saveController theSaveController;

	public Animator statsAnimator;

	// Use this for initialization
	void Start () {
		
		hpPanel.GetComponent<CanvasGroup>().alpha = 0;

		menuButton.enabled = false;

		if(theSaveController.saveExists() == false){
			continueButton.enabled = false;
			continueButton.GetComponent<CanvasGroup> ().alpha = 0.25f;

		} else {
			continueButton.enabled = true;
			continueButton.GetComponent<CanvasGroup> ().alpha = 1.0f;

		}

	}
	
	// Update is called once per frame
	void Update () {
		
	}


	public void menuClick(){

		bool isHidden = statsAnimator.GetBool ("isHidden");

		if(isHidden){
			menuButton.image.sprite = hamburgerCloseSprite;
		} else {
			menuButton.image.sprite = hamburgerOpenSprite;
		}

		statsAnimator.SetBool("isHidden", !isHidden);

	}


	public void showHP(){
		hpPanel.GetComponent<CanvasGroup>().alpha = 1;

	}

	public void hideHP(){
		hpPanel.GetComponent<CanvasGroup>().alpha = 0;

	}


	public void refresh(){

		hpText.text = theHero.getHP().ToString() + "/" + theHero.getmaxHP().ToString();

	}

//this will start a new game no matter where we are
	public void resetGame(){

		menuButton.enabled = true;

		theGameController.gameActive = true;
		menuClick();

		theDialogController.startOver ();
		theDialogController.clearHistory ();
		theHero.resetHero();
		hpPanel.GetComponent<CanvasGroup>().alpha = 0;

		refresh();

		theGameController.reloadRoom();

		continueButton.enabled = true;

	}

	public void resumeGame(){

		menuButton.enabled = true;

		menuClick();

		//If the game hasn't started yet, we need to load the save data, then continue
		if(theGameController.gameActive == false){
			//Have the dialog controller load the savegame data
			Debug.Log("Load the save game");

			theSaveController.loadGame();


			refresh();
			//have the game controller reloadRoom
			theGameController.reloadRoom();

			if(theHero.countAttributes() > 1){ //we have a class and we should show the health
				showHP();
			}
		} else {
			Debug.Log("game is active, just resuming");
		} 

		theGameController.gameActive = true;
	}

}
