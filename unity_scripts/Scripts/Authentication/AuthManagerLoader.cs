using System.Collections;
using UnityEngine;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;

/// <summary>
/// This class is currently irrelevant, as Authentication is performed by
/// the web application that serves it. 
/// In the future, it may make sense to implement it
/// and make the Unity client more platform oblivious.
/// </summary>
public class AuthManagerLoader : MonoBehaviour
{
    public string uid;
    public GameObject buttonSubmit;
    public GameObject displayText;
    public InputField usernameField;
    public InputField passwordField;

    private string inputUid;
    private int attempts;
    private string sceneName;


    private void Start()
    {
        buttonSubmit.SetActive(true);
    }

    private void goToStart()
    {
        buttonSubmit.SetActive(true);
    }


    void onLoginButtonClick()
    {
        buttonSubmit.SetActive(false);

        //bool successfulAuthentication = AttemptToLogin();
    }

    private void AttemptToLogin()
    {
        
    }

    public void LoadMyScene()
    {
        attempts += 1;
        Load();
        if (uid != inputUid)
        {
            displayText.GetComponent<Text>().text = "" + uid + " vs " + inputUid + " " + "Your Unique Identifier Does Not Match";
            //uidField.text = "";
            this.GetComponent<MeshRenderer>().material.color = Color.red;
        }
        else
        {
            displayText.GetComponent<Text>().text = "Your Unique Identifier Is Updated";
            this.GetComponent<MeshRenderer>().material.color = Color.green;
            //buttonReset.SetActive(false);
            buttonSubmit.SetActive(false);
            //Load next scene
            sceneName = "CardboardRCT/Scenes/MainSceneVR";

            StartCoroutine(LoadVR());

        }
    }
    IEnumerator LoadVR()
    {

        SceneManager.LoadScene(sceneName, LoadSceneMode.Single);
        yield return new WaitForSeconds(10f);
        goToStart();
        displayText.GetComponent<Text>().text = "Error. Try again.";
    }

    public void ResetUid()
    {
        uid = inputUid;
        Save();
        LoadMyScene();
    }

    public void UpdateUid()
    {
        //inputUid = uidField.text;
    }

    public void Save()
    {
        // Create a binary formatter
        BinaryFormatter bf = new BinaryFormatter();
        // Create a file to store the data
        FileStream file = File.Open(Application.persistentDataPath + "/userData.dat", FileMode.OpenOrCreate);
        // Create a container for the data
        //UserData data = new UserData();

        // Assign the current data values
        //data.uid = uid;

        //// Write the data to the file
        //bf.Serialize(file, data);
        // Close file
        file.Close();
    }

    public void Load()
    {
        // Check if file exists
        if (File.Exists(Application.persistentDataPath + "/userData.dat"))
        {
            // Create a binary formatter
            BinaryFormatter bf = new BinaryFormatter();
            // Create a file to store the data
            FileStream file = File.Open(Application.persistentDataPath + "/userData.dat", FileMode.Open);
            // Read data from file
            //UserData data = (UserData)bf.Deserialize(file);
            // Close file
            file.Close();

            // Assign the saved data values to current values
            //uid = data.uid;

        }
        else
        {
            uid = inputUid;
            Save();
        }
    }
}
