using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;


public class uiControllerScript : MonoBehaviour {

	public Canvas myCanvas;
	public RectTransform statsPanel;

	public Text attributesText;
	public Text hpText;
	public Text strengthText;
	public Text constitutionText;
	public Text dexterityText;
	public Text intelligenceText;
	public Text wisdomText;
	public Text charismaText;


	public Button menuButton;


	public Sprite hamburgerOpenSprite;
	public Sprite hamburgerCloseSprite;

	public hero theHero;

	public Animator statsAnimator;

	// Use this for initialization
	void Start () {
		
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


	public void refresh(){

		strengthText.text = theHero.getStat ("Strength").ToString();
		constitutionText.text = theHero.getStat ("Constitution").ToString();
		dexterityText.text = theHero.getStat ("Dexterity").ToString();
		intelligenceText.text = theHero.getStat ("Intelligence").ToString();
		charismaText.text = theHero.getStat ("Charisma").ToString();
		wisdomText.text = theHero.getStat ("Wisdom").ToString();

		hpText.text = theHero.getHP().ToString() + "/" + theHero.getmaxHP().ToString();

		attributesText.text = theHero.getAttributes ();

	}

}
