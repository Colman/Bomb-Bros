using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RoomButton : MonoBehaviour {

    public void RoomPressed() {
        RoomSelect select = GameObject.Find("RoomSelect").GetComponent<RoomSelect>();
        select.RoomPressed(transform.Find("RoomName").GetComponent<Text>().text);
    }
}
