using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class testControllerScript : MonoBehaviour
{

    private bool isHidden = true;
    
    public RectTransform testPanel;

	//Dialog controller actually does all the dialog interactions
	public dialogController theDialogController;

	//UI controller controls UI pieces that are not the main game text
	public uiControllerScript theUIController;

    public controllerScript theGameController;

    public hero theHero;


    public Text doomText;
    public Text hpText;

    public Dropdown roomDropdown;

    List<string> roomlist = new List<string> {
        "spiderroom", 
        "cavetrolls",
        "tunnel2cont",
        "cockatrice"
        };


    public Text roomText;

    // Start is called before the first frame update
    void Start()
    {
        roomDropdown.ClearOptions();
        roomDropdown.AddOptions(roomlist);


    }

    // Update is called once per frame
    void Update()
    {

        //This probably doesn't need to be done here, it's a waste of processor.
        if(!isHidden){ //dont' do this if it's not visible
            doomText.text = theHero.getDoom().ToString();
            hpText.text = theHero.getHP().ToString();
            roomText.text = theDialogController.currentKey();
        }

    }

    public void testClick(){

        if(isHidden){

			testPanel.position = new Vector3 (0, testPanel.position.y, testPanel.position.z);
            isHidden = false;
        }
        else
        {
			testPanel.position = new Vector3 (-10000, testPanel.position.y, testPanel.position.z);
            isHidden = true;
        }


    }


    public void increaseDoom(){
        theHero.increaseDoom();
        theUIController.refresh();
    }

    public void decreaseDoom(){
        theHero.increaseDoom(-1);
        theUIController.refresh();
    }

    public void increaseHP(){
        theHero.restoreHP(1);
        theUIController.refresh();
    }

    public void decreaseHP(){
        theHero.loseHP(1);
        theUIController.refresh();
   }


    public void setFighter(){
        theHero.setClass("Fighter");
        theUIController.refresh();
    }

    public void setWizard(){
        theHero.setClass("Wizard");
        theUIController.refresh();
    }


    public void goToRoom(){

        string roomKey = roomlist[roomDropdown.value];
        Debug.Log(roomKey);

        theDialogController.loadRoom(roomKey);
        theGameController.reloadRoom();

        testClick();

    }

}
