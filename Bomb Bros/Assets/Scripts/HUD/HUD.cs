using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class HUD : MonoBehaviour {

    //Hud var
    float startTime;
    bool leaving;

    //Input vars
    Vector2 shootStart;
    Vector2 shootEnd;
    float shootSens;
    bool isHovered;

    //Camera var
    Camera mainCam;

    //Script var
    Bro myBro;

    //UI vars
    public Text timeObj;
    public Text killsObj;
    public Text deathsObj;
    public Image healthImg;
    public RectTransform joyBack;
    public RectTransform joy;
    public Button useButton;
    public RectTransform useTrans;


    void Awake() {
        //Hud init
        startTime = Time.time;
        leaving = false;

        //Input inits
        shootStart = new Vector2();
        shootEnd = new Vector2();
        if (Menu.isPc) {
            shootSens = PlayerPrefs.GetFloat("ShootSens") * 3.25f + 0.125f;
        }
        else {
            shootSens = PlayerPrefs.GetFloat("ShootSens") * 24f + 1;
        }
        isHovered = false;

        //Camera init
        mainCam = Camera.main;
    }



    void Start() {
        //Script init
        GameObject[] objs = GameObject.FindGameObjectsWithTag("Bro");
        foreach (GameObject obj in objs) {
            myBro = obj.GetComponent<Bro>();
            if (myBro != null) {
                if (myBro.isMe) {
                    break;
                }
            }
        }
        if (!myBro.isBlue) {
            joyBack.anchorMin = new Vector2(1, 0);
            joy.anchorMin = new Vector2(1, 0);
            joyBack.anchorMax = new Vector2(1, 0);
            joy.anchorMax = new Vector2(1, 0);
            joyBack.anchoredPosition = new Vector2(-90, 90);
            joy.anchoredPosition = new Vector2(-90, 90);

            useTrans.anchorMin = new Vector2(0, 0);
            useTrans.anchorMax = new Vector2(0, 0);
            useTrans.anchoredPosition = new Vector2(70, 70);
        }
    }



    void FixedUpdate() {
        if (!leaving) {
            if (Menu.isPc) {
                GetPcInput();
            }
            else {
                GetMobileInput();
            }

            healthImg.fillAmount = myBro.health / myBro.maxHealth;
            killsObj.text = "" + myBro.kills;
            deathsObj.text = "" + myBro.deaths;
            ChangeTime();
        }
    }



    public void UpdateHovered(bool hovered) {
        isHovered = hovered;
    }



    void ChangeTime() {
        string seconds;
        float secondsf = Mathf.Round(Time.time - startTime) % 60;
        if (secondsf >= 600) {
            BackPressed(); //Leave Game
        }

        if (secondsf < 10) {
            seconds = "0" + secondsf;
        }
        else {
            seconds = "" + secondsf;
        }

        string time = Mathf.Floor(Mathf.Round(Time.time - startTime) / 60) + ":" + seconds;
        timeObj.text = time;
    }



    public void BackPressed() {
        leaving = true;
        if (PhotonNetwork.offlineMode) {
            PhotonNetwork.LeaveRoom();
        }
        else {
            if (PhotonNetwork.room.PlayerCount == 1) {
                Debug.Log("DestroyedAll");
                PhotonNetwork.DestroyAll();
            }
            PhotonNetwork.Disconnect();
        }
    }



    void OnLeftRoom() {
        Debug.Log("LeftRoom");
        SceneManager.LoadSceneAsync("Menu");
    }



    void OnDisconnectedFromPhoton() {
        Debug.Log("Disconnected");
        SceneManager.LoadSceneAsync("Menu");
    }



    void GetPcInput() {
        if (Input.GetKey("w")) {
            myBro.Jump();
        }

        if (Input.GetKey("a")) {
            myBro.Move(-1);
        }

        if (Input.GetKey("d")) {
            myBro.Move(1);
        }

        if (!Input.GetKey("d") && !Input.GetKey("a")) {
            myBro.Move(0);
        }

        if (Input.GetMouseButton(0) && !isHovered) {
            PcCalcFire();
        }
    }



    void GetMobileInput() {
        bool moveTouch = false;
        bool shootTouch = false;

        foreach (Touch touch in Input.touches) {
            if (!moveTouch || !shootTouch) {
                Vector2 tPos = touch.position;
                float rad = joyBack.rect.width / 2;
                bool isMoveCond = false;
                if (myBro.isBlue) {
                    isMoveCond = tPos.x < Screen.width / 2;
                }
                else {
                    isMoveCond = tPos.x > Screen.width / 2;
                }

                if (isMoveCond && !moveTouch) {
                    Vector2 touchVec;
                    RectTransformUtility.ScreenPointToLocalPointInRectangle(joyBack,
                    tPos, mainCam, out touchVec);

                    Vector2 aBackPos = joyBack.anchoredPosition;
                    Vector2 aJoyPos = joy.anchoredPosition;

                    if (touchVec.magnitude > rad) {
                        touchVec = touchVec.normalized * rad;
                    }

                    joy.anchoredPosition = aBackPos + touchVec;

                    myBro.Move(touchVec.x / rad);
                    if (touchVec.y / rad > 0.5f) {
                        myBro.Jump();
                    }

                    moveTouch = true;
                }
                if (!isMoveCond && !shootTouch) {
                    if (touch.phase == TouchPhase.Began) {
                        shootStart = touch.position;
                    }
                    else if (touch.phase == TouchPhase.Ended) {
                        shootEnd = touch.position;
                        if (!isHovered) {
                            MobileCalcFire();
                        }
                    }
                    shootTouch = true;
                }
            }
            else {
                break;
            }
        }
        if (!moveTouch) {
            joy.anchoredPosition = joyBack.anchoredPosition;
            myBro.Move(0);
        }
    }



    public void PcCalcFire() {
        if (!myBro.isDead) {
            float camHeight = mainCam.orthographicSize * 2;
            float camWidth = camHeight * mainCam.aspect;

            float gameX = Input.mousePosition.x * camWidth / Screen.width - camWidth / 2;
            float gameY = Input.mousePosition.y * camHeight / Screen.height - camHeight / 2;

            Vector2 projDir = new Vector2(gameX, gameY) - myBro.rb.position;
            Vector2 projPos = myBro.rb.position + projDir.normalized * myBro.fireRad;
            myBro.Fire(projPos, projDir * shootSens);
        }
    }



    public void MobileCalcFire() {
        if (!myBro.isDead) {
            Vector2 shootNet = (shootEnd - shootStart) / Screen.dpi;

            Vector2 projPos = myBro.rb.position + shootNet.normalized * myBro.fireRad;
            myBro.Fire(projPos, shootNet * shootSens);
        }
    }



    public void ChangeUseButton(bool canUse) {
        if (canUse) {
            useButton.interactable = true;
        }
        else {
            useButton.interactable = false;
        }
    }



    public void UsePressed() {
        if (!myBro.isDead) {
            myBro.gameObject.GetComponent<PhotonView>().RPC("SwapGun", PhotonTargets.AllBuffered, myBro.pickupType);
        }
    }
}
