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


	public Sprite hamburgerOpenSprite;
	public Sprite hamburgerCloseSprite;

	public hero theHero;
	public dialogController theDialogController;
	public controllerScript theGameController;

	public Animator statsAnimator;

	// Use this for initialization
	void Start () {
		
		hpPanel.GetComponent<CanvasGroup>().alpha = 0;

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

	public void resetGame(){

		menuClick();

		theDialogController.startOver ();
		theDialogController.clearHistory ();
		theHero.resetHero();
		hpPanel.GetComponent<CanvasGroup>().alpha = 0;

		refresh();

		theGameController.reloadRoom();

		
	}

}
