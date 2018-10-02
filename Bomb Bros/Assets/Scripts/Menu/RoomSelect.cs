using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RoomSelect : Photon.MonoBehaviour {

    //Room var
    bool openRoom;

    //UI vars
    int statsUpdateTime;
    float dotSpeed;

    //Room vars
    RoomInfo[] rooms;
    public GameObject roomList;
    public GameObject roomPrefab;

    //UI vars
    public Text statsText;
    public Text connectingText;
    public GameObject noRooms;
    public GameObject roomScroll;
    public GameObject randButton;
    public GameObject stats;
    public GameObject orText;
    public GameObject roomTitle;
    public GameObject roomInput;
    public GameObject createButton;
    public GameObject main;
    public GameObject roomBro;
    
    //RoomSelect vars
    public static bool created;
    public static string roomJoinName;


    void Awake() {
        //Room init
        openRoom = false;

        //UI inits
        statsUpdateTime = 1;
        dotSpeed = 0.1f;
    }



    void OnEnable() {
        if (PhotonNetwork.connected) {
            OnJoinedLobby();
        }
        StartCoroutine(Connecting());
    }



    IEnumerator Connecting() {
        while (true) {
            connectingText.text = "Connecting";
            yield return new WaitForSeconds(dotSpeed);
  
            connectingText.text = "Connecting.";
            yield return new WaitForSeconds(dotSpeed);

            connectingText.text = "Connecting..";
            yield return new WaitForSeconds(dotSpeed);

            connectingText.text = "Connecting...";
            yield return new WaitForSeconds(dotSpeed);
        }
    }



    IEnumerator Stats() {
        while (true) {
            statsText.text = "Players Online: " + PhotonNetwork.countOfPlayers + "\n"
                            + "Rooms: " + PhotonNetwork.countOfRooms;
            yield return new WaitForSeconds(statsUpdateTime);
        }
    }



    public void JoinRandomPressed() {
        roomJoinName = "";
        created = false;
        gameObject.SetActive(false);
        roomBro.SetActive(true);
    }
    


    void OnJoinedLobby() {
        Debug.Log(PhotonNetwork.sendRate);
        connectingText.gameObject.SetActive(false);
        roomScroll.SetActive(true);
        randButton.SetActive(true);
        stats.SetActive(true);
        orText.SetActive(true);
        roomTitle.SetActive(true);
        roomInput.SetActive(true);
        createButton.SetActive(true);
        UpdateCreate();
        rooms = PhotonNetwork.GetRoomList();
        statsText.text = "Players Online: " + PhotonNetwork.countOfPlayers + "\n"
                            + "Rooms: " + PhotonNetwork.countOfRooms;
        StartCoroutine(Stats());
    }



    void OnReceivedRoomListUpdate() {
        rooms = PhotonNetwork.GetRoomList();
        ChangeRoomScroll();
        randButton.GetComponent<Button>().interactable = openRoom;  
    }



    public void UpdateCreate() {
        bool empty = roomInput.GetComponent<InputField>().text == "";
        createButton.GetComponent<Button>().interactable = !empty;
    }



    public void CreatePressed() {
        created = true;
        gameObject.SetActive(false);
        roomBro.SetActive(true);
    }



    public void RoomPressed(string roomName) {
        roomJoinName = roomName;
        created = false;
        gameObject.SetActive(false);
        roomBro.SetActive(true);
    }



    public void BackPressed() {
        PhotonNetwork.Disconnect();  
    }



    public void OnDisconnectedFromPhoton() {
        PhotonNetwork.offlineMode = true;
        connectingText.gameObject.SetActive(true);
        roomScroll.SetActive(false);
        randButton.SetActive(false);
        stats.SetActive(false);
        orText.SetActive(false);
        roomTitle.SetActive(false);
        roomInput.SetActive(false);
        createButton.SetActive(false);
        noRooms.SetActive(false);
        StopCoroutine(Connecting());
        StopCoroutine(Stats());
        gameObject.SetActive(false);
        main.SetActive(true);
    }



    void ChangeRoomScroll() {
        RectTransform rectTrans = roomList.GetComponent<RectTransform>();
        int contentHeight = 0;
        if (rooms.Length > 0) {
            contentHeight = 70 * rooms.Length - 10;
        }
        rectTrans.sizeDelta = new Vector2(rectTrans.sizeDelta.x, contentHeight);

        int start = roomList.transform.childCount - 1;
        for (int i = start; i >= 0; i--) {
            DestroyImmediate(roomList.transform.GetChild(i).gameObject);
        }
        openRoom = false;
        for (int i = 0; i < rooms.Length; i++) {
            GameObject roomObj = Instantiate(roomPrefab, roomList.transform);
            Text nameComp = roomObj.transform.Find("RoomName").GetComponent<Text>();
            nameComp.text = rooms[i].Name;

            Text playerComp = roomObj.transform.Find("Players").GetComponent<Text>();
            playerComp.text = rooms[i].PlayerCount + "/6";
            if(rooms[i].IsOpen) {
                openRoom = true;
                roomObj.GetComponent<Button>().interactable = true;
            }
            else {
                roomObj.GetComponent<Button>().interactable = false;
            }
        }

        int count = 0;
        foreach (RectTransform child in roomList.transform) {
            child.anchoredPosition = new Vector2(child.anchoredPosition.x, -70 * count);
            count++;
        }

        if (rooms.Length == 0) {
            noRooms.SetActive(true);
        }
        else {
            noRooms.SetActive(false);
        }
    }
}
