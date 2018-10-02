using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Menu : MonoBehaviour {

    //Machine var
    public static bool isPc;
    //Version var
    string version;

    public GameObject shop;


    void Awake() {
        //Machine init
        isPc = true;
        PlayerPrefs.DeleteAll();

        //Version init
        version = "v1.0.0";
    }



    void Start() {
        PhotonNetwork.offlineMode = true;

        if (!PlayerPrefs.HasKey("MusicVol")) {
            PlayerPrefs.SetFloat("MusicVol", 0.5f);
        }

        if (!PlayerPrefs.HasKey("SfxVol")) {
            PlayerPrefs.SetFloat("SfxVol", 0.5f);
        }

        if (!PlayerPrefs.HasKey("ShootSens")) {
            PlayerPrefs.SetFloat("ShootSens", 0.5f);
        }

        if (!PlayerPrefs.HasKey("DisplayName")) {
            int ranNum = (int)Mathf.Floor(Random.value * 1000);
            PlayerPrefs.SetString("DisplayName", "Player" + ranNum);
            PlayerPrefs.Save();
        }

        GetComponent<AudioSource>().volume = PlayerPrefs.GetFloat("MusicVol");
        PhotonNetwork.sendRate = 40;
    }



    public void MultiplayerPressed() {
        PhotonNetwork.ConnectUsingSettings(version);
    }
}
