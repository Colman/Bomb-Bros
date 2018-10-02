using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class RoomWait : MonoBehaviour {

    //Photon var
    PhotonView roomWaitView;

    //UI vars
    public Text roomName;
    public Text mapName;
    public Button startButton;
    public Button swapButton;
    public Button backButton;
    public Text[] blue;
    public Text[] red;

    //RoomWait vars
    public static bool isBlue;
    int blueCount;
    int redCount;

    //RoomSelect var
    public RoomSelect roomSelect;


    void Awake() {
        //Photon init
        roomWaitView = GetComponent<PhotonView>();
    }



    void Start() {
        Debug.Log("Synced");
        PhotonNetwork.automaticallySyncScene = true;
    }



    [PunRPC]
    public void ChangeMapName(string name) {
        mapName.text = name;
    }



    [PunRPC]
    public void ChangeNames() {
        foreach (Text t in blue) {
            t.text = "";
        }
        foreach (Text t in red) {
            t.text = "";
        }

        blueCount = 0;
        redCount = 0;
        ExitGames.Client.Photon.Hashtable props = new ExitGames.Client.Photon.Hashtable();
        foreach (PhotonPlayer player in PhotonNetwork.playerList) {
            props = player.CustomProperties;
            if ((bool)props["isBlue"]) {
                blue[blueCount].text = player.NickName;
                blueCount++;
            }
            else {
                red[redCount].text = player.NickName;
                redCount++;
            }

            if (player.Equals(PhotonNetwork.player)) {
                isBlue = (bool)props["isBlue"];
            }
        }

        if (blueCount >= 3 && !isBlue) {
            swapButton.interactable = false;
        }
        else if (redCount >= 3 && isBlue) {
            swapButton.interactable = false;
        }
        else {
            swapButton.interactable = true;
        }
    }



    public void SwapPressed() {
        ExitGames.Client.Photon.Hashtable props = new ExitGames.Client.Photon.Hashtable();
        if (isBlue) {
            props.Add("isBlue", false);
        }
        else {
            props.Add("isBlue", true);
        }
        PhotonNetwork.SetPlayerCustomProperties(props);
        roomWaitView.RPC("ChangeNames", PhotonTargets.All);
    }



    void OnEnable() {
        roomName.text = PhotonNetwork.room.Name;
        if (PhotonNetwork.isMasterClient) {
            startButton.interactable = true;
        }
        else {
            startButton.interactable = false;
        }
    }



    [PunRPC]
    public void MasterStarted(string mapString) {
        startButton.interactable = false;
        swapButton.interactable = false;
        backButton.interactable = false;
        PhotonNetwork.isMessageQueueRunning = false;
        SceneManager.LoadSceneAsync(mapString);
    }



    public void StartPressed() {
        PhotonNetwork.room.IsOpen = false;
        roomWaitView.RPC("MasterStarted", PhotonTargets.All, mapName.text);
    }



    public void BackPressed() {
        PhotonNetwork.Disconnect();
    }



    void OnDisconnectedFromPhoton() {
        gameObject.SetActive(false);
        roomSelect.OnDisconnectedFromPhoton();
    }



    void OnPhotonPlayerDisconnected(PhotonPlayer player) {
        if (PhotonNetwork.isMasterClient) {
            startButton.interactable = true;
            if (PhotonNetwork.room.PlayerCount >= PhotonNetwork.room.MaxPlayers) {
                PhotonNetwork.room.IsOpen = false;
            }
            else {
                PhotonNetwork.room.IsOpen = true;
            }
        }
        else {
            startButton.interactable = false;
        }
        ChangeNames();
    }



    void OnPhotonPlayerConnected(PhotonPlayer player) {
        if (PhotonNetwork.isMasterClient) {
            if (PhotonNetwork.room.PlayerCount >= PhotonNetwork.room.MaxPlayers) {
                PhotonNetwork.room.IsOpen = false;
            }
            else {
                PhotonNetwork.room.IsOpen = true;
            }
        }
    }
}
