using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomBro : MonoBehaviour {

    //UI vars
    public GameObject roomMap;
    public GameObject roomWait;
    public RoomSelect roomSelect;


	public void SelectClicked() {
        if (RoomSelect.created) {
            gameObject.SetActive(false);
            roomMap.SetActive(true);
        }
        else {
            if(RoomSelect.roomJoinName == "") {
                PhotonNetwork.JoinRandomRoom();
            }
            else {
                PhotonNetwork.JoinRoom(RoomSelect.roomJoinName);
            } 
        }
    }



    void OnJoinedRoom() {
        int blueCount = 0;
        int redCount = 0;
        foreach (PhotonPlayer player in PhotonNetwork.playerList) {   
            if (!player.Equals(PhotonNetwork.player)) {
                if ((bool)player.CustomProperties["isBlue"]) {
                    blueCount++;
                }
                else {
                    redCount++;
                }
            }
        }
        ExitGames.Client.Photon.Hashtable props = new ExitGames.Client.Photon.Hashtable();
        props.Add("isBlue", redCount >= blueCount);

        PhotonNetwork.SetPlayerCustomProperties(props);
        if(PlayerPrefs.GetString("DisplayName") == "") {
            int ranNum = (int)Mathf.Floor(Random.value * 1000);
            PlayerPrefs.SetString("DisplayName", "Player" + ranNum);
            PlayerPrefs.Save();
        }
        PhotonNetwork.playerName = PlayerPrefs.GetString("DisplayName");
        gameObject.SetActive(false);
        roomWait.SetActive(true);
        roomWait.GetComponent<PhotonView>().RPC("ChangeNames", PhotonTargets.All);
    }



    void OnPhotonJoinRoomFailed(object[] code) {
        PhotonNetwork.Disconnect();
    }



    void OnDisconnectedFromPhoton() {
        gameObject.SetActive(false);
        roomSelect.OnDisconnectedFromPhoton();
    }
}
