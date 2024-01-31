using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Data_Script : MonoBehaviour
{
    private static Data_Script _instance;

    public static Data_Script Instance { get { return _instance; } }

    private void Awake() {
        if (_instance != null && _instance != this) {
            Destroy(this.gameObject);
        } else {
            _instance = this;
        }
    }

    public void Save_Single_Data(string name, string stringValue, string type) {

        if (name != null && stringValue != null && type != null && !string.Equals(name, "") && !string.Equals(type, "") && !string.Equals(stringValue, "")) { // a bit to much maybe

            switch (type) {

                case "float":
                    float floatValue = float.Parse(stringValue);
                    PlayerPrefs.SetFloat(name, floatValue);
                    break;

                case "int":
                    int intValue = int.Parse(stringValue);
                    PlayerPrefs.SetInt(name, intValue);
                    break;

                case "string":
                    PlayerPrefs.SetString(name, stringValue);
                    break;
            }
        }
    }

    public void Save_Data_List(List<string> list, string name, string type) {

        if (name != null && type != null && !string.Equals(name, "") && !string.Equals(type, ""))

            if (string.Equals(type, "int") && list != null && list.Count >= 0) {
                PlayerPrefs.SetInt(name + "_Count", list.Count);
                for (int i = 0; i < list.Count; i++) {
                    int intValue = int.Parse(list[i]);
                    PlayerPrefs.SetInt(name + "_" + i, intValue);
                }

            } else if (string.Equals(type, "float") && list != null && list.Count >= 0) {
                PlayerPrefs.SetInt(name + "_Count", list.Count);
                for (int i = 0; i < list.Count; i++) {
                    float floatValue = float.Parse(list[i]);
                    PlayerPrefs.SetFloat(name + "_" + i, floatValue);
                }

            } else if (string.Equals(type, "string") && list != null && list.Count >= 0) {
                PlayerPrefs.SetInt(name + "_Count", list.Count);
                for (int i = 0; i < list.Count; i++) {
                    PlayerPrefs.SetString(name + "_" + i, list[i]);
                }
            }
    }

    //-----------------------------LOADING---------------------------------------------//


    public string Load_Single_Data_String(string name) {
        return PlayerPrefs.GetString(name);
    }

    public int Load_Single_Data_Int(string name) {
        return PlayerPrefs.GetInt(name);
    }

    public float Load_Single_Data_Float(string name) {
        return PlayerPrefs.GetFloat(name);
    }


    public List<string> Load_Data_List_String(string name) {
        List<string> tempList = new List<string>();
        int amt = PlayerPrefs.GetInt(name + "_Count");

        for (int i = 0; i < amt; i++) {
            string stringData = PlayerPrefs.GetString(name + "_" + i);
            tempList.Add(stringData);
        }
        return tempList;
    }

    public List<int> Load_Data_List_Int(string name) {
        List<int> tempList = new List<int>();
        int amt = PlayerPrefs.GetInt(name + "_Count");

        for (int i = 0; i < amt; i++) {
            string stringData = PlayerPrefs.GetString(name + "_" + i);
            int intValue = int.Parse(stringData);
            tempList.Add(intValue);
        }
        return tempList;
    }

    public List<float> Load_Data_List_Float(string name) {
        List<float> tempList = new List<float>();
        int amt = PlayerPrefs.GetInt(name + "_Count");

        for (int i = 0; i < amt; i++) {
            string stringData = PlayerPrefs.GetString(name + "_" + i);
            float floatValue = float.Parse(stringData);
            tempList.Add(floatValue);
        }
        return tempList;
    }


    //---------------------------------EXTRAS-------------------------------//


    public bool Has_Data_Key(string key) {
        return PlayerPrefs.HasKey(key);
    }

    public void Save_All_Data() {
        PlayerPrefs.Save();
    }

    public void Delete_All_Data() {
        PlayerPrefs.DeleteAll();
    }

    public void Delete_Data_With_Name(string name) {
        PlayerPrefs.DeleteKey(name);
    }




    // USE ingame

    private void Start() {
        New_Game_Settings();
    }

    private void New_Game_Settings() {
        string playerStats = Load_Single_Data_String("Player_Stats");

        if (playerStats == null || string.Equals(playerStats, "")) {

            Save_Single_Data("Player_Stats", "true", "string");
            Save_Single_Data("Highscore", "0", "int");
            Save_Single_Data("StackAmt", "0", "int");
            Save_All_Data();

            Debug.Log("Setting up new game settings");
        } else {
         
            //	Debug.Log("Loading game settings");
        }
    }

    public void SetNewHighScore(int score) {
        Save_Single_Data("Highscore", score.ToString(), "int");
    }

    public int GetPlayerHighScore() {
        return Load_Single_Data_Int("Highscore");
    }

    public void SetNewStackScore(int score) {
        Save_Single_Data("StackAmt", score.ToString(), "int");
    }

    public int GetPlayerStackScore() {
        return Load_Single_Data_Int("StackAmt");
    }
}
