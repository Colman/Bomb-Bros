using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RoomMapSelect : MonoBehaviour {

    //Options var
    RoomOptions options;

    //RoomMapSelect var
    public static string level;

    //UI vars
    public GameObject roomWait;
    public Text roomText;

    
    void Awake() {
        //Options init
        options = new RoomOptions();
        options.MaxPlayers = 6;
    }



    public void JunglePressed() {
        level = "Jungle";
        PhotonNetwork.JoinOrCreateRoom(roomText.text, options, null);
    }



    public void FactoryPressed() {
        level = "Factory";
        PhotonNetwork.JoinOrCreateRoom(roomText.text, options, null);
    }



    public void TemplePressed() {
        level = "Temple";
        PhotonNetwork.JoinOrCreateRoom(roomText.text, options, null);
    }



    public void ForestPressed() {
        level = "Forest";
        PhotonNetwork.JoinOrCreateRoom(roomText.text, options, null);
    }



    public void VolcanoPressed() {
        level = "Volcano";
        PhotonNetwork.JoinOrCreateRoom(roomText.text, options, null);
    }



    void OnJoinedRoom() {
        ExitGames.Client.Photon.Hashtable props = new ExitGames.Client.Photon.Hashtable();
        props.Add("isBlue", true);
        PhotonNetwork.SetPlayerCustomProperties(props);
        
        if (PlayerPrefs.GetString("DisplayName") == "") {
            int ranNum = (int) Mathf.Floor(Random.value * 1000);
            PlayerPrefs.SetString("DisplayName", "Player" + ranNum);
            PlayerPrefs.Save();
        }
        PhotonNetwork.playerName = PlayerPrefs.GetString("DisplayName");

        gameObject.SetActive(false);
        roomWait.SetActive(true);
        roomWait.GetComponent<PhotonView>().RPC("ChangeNames", PhotonTargets.All);
        roomWait.GetComponent<PhotonView>().RPC("ChangeMapName", PhotonTargets.AllBuffered, level);
    }
}

