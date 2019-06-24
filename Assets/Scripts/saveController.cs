using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
public class saveController : MonoBehaviour
{


    public dialogController theDialogController;
    public hero theHero;

    private string gameSavePath = "/gamesave.save";

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public bool saveExists(){
        return (File.Exists(Application.persistentDataPath + gameSavePath));
    }


    public void loadGame(){
        if (File.Exists(Application.persistentDataPath + gameSavePath)){
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(Application.persistentDataPath + gameSavePath, FileMode.Open);
            Save save = (Save)bf.Deserialize(file);
            file.Close();

            theDialogController.loadSave(save);
            theHero.loadSave(save);

        } else {
            Debug.Log("Tried loading a savefile that doesn't exist");
        }
    }

    public void saveGame(){
        //Create a new save data entity
		Save save = CreateSave();

        //Write the save data entity to a file
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Create(Application.persistentDataPath + "/gamesave.save");
        bf.Serialize(file, save);
        file.Close();

	}

	private Save CreateSave(){
        Save save = new Save();

        //iterate through dialog history and add to save
        List<string>dialogHistory = theDialogController.getDialogHistory();
        foreach (string dialogEvent in dialogHistory){
            save.dialogHistory.Add(dialogEvent);
        }

        //save the curent dialog event
        save.currentEventKey = theDialogController.currentKey();

        save.dialogRandNum = theDialogController.getRand();

        //save the hero
        //iterate through stats
        Dictionary <string,int> impStats = theHero.exportStats();
        foreach(KeyValuePair<string,int> entry in impStats){
            save.stats[entry.Key] = entry.Value;
        }

        //iterate through attributes
        List<string> impAtrbts = theHero.exportAttributes();
        foreach(string thisAttribute in impAtrbts){
            save.attributes.Add(thisAttribute);
        }

        //Set all of the other info
        save.HP = theHero.getHP();
        save.maxHP = theHero.getmaxHP();
        save.doomClock = theHero.getDoom();
        save.plus1Forward = theHero.hasPlus1Forward();
        save.minus1Forward = theHero.hasMinus1Forward();

        return save;
    }
}
