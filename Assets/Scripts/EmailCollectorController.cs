using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public class EmailCollectorController : MonoBehaviour
{

    public InputField emailInput;
    public uiControllerScript uiController;


    [SerializeField] private List<string> emailAddresses;


    // Start is called before the first frame update
    void Start()
    {
        emailAddresses = new List<string>();
        
        if (File.Exists(Application.persistentDataPath + "/gamesave.save")){
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(Application.persistentDataPath + "/gamesave.save", FileMode.Open);
            Save save = (Save)bf.Deserialize(file);
            file.Close();

            foreach (string emailaddress in save.emails){
                emailAddresses.Add(emailaddress);
            }
        }


    }

    // Update is called once per frame
    void Update()
    {
        
    }


    public void doneButtonPress(){

        if(emailInput.text != ""){
            emailAddresses.Add(emailInput.text);
            emailInput.text = "";
        }

        uiController.menuClick();

        Save save = CreateSave();
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Create(Application.persistentDataPath + "/gamesave.save");
        bf.Serialize(file, save);
        file.Close();
    }


    private Save CreateSave(){
        Save save = new Save();
        foreach (string emailaddress in emailAddresses){
            save.emails.Add(emailaddress);
        }

        return save;
    }


}
