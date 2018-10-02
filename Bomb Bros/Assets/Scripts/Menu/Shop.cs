using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Shop : MonoBehaviour {

    //Purchaser var
    Purchaser purchaser;

    //Shop vars
    int broAmt;
    int shiftAmt;
    public static int selectedBro;
    public static bool[] unlocked;

    //UI vars
    string[] bros;
    public RectTransform[] trans;
    public Image[] imgs;
    public Text broName;
    public Image ownedImg;
    public Image lockImg;
    public Button unlockButton;


    void Awake() {
        //Purchaser init
        purchaser = GetComponent<Purchaser>();

        //Shop inits
        broAmt = 3;
        shiftAmt = 220;
        selectedBro = 0;
        unlocked = new bool[broAmt];

        //UI inits
        bros = new string[] {
            "Trump",
            "Jesus",
            "Dougie"
        };
    }



    void Start() {
        for (int i = 0; i < bros.Length; i++) {
            if (PlayerPrefs.HasKey(bros[i])) {
                if (PlayerPrefs.GetInt(bros[i]) == 0) {
                    unlocked[i] = false;
                }
                else {
                    unlocked[i] = true;
                }
            }
            else {
                PlayerPrefs.SetInt(bros[i], 0);
            }
        }

        lockImg.enabled = !unlocked[0];
        ownedImg.enabled = unlocked[0];
        unlockButton.interactable = !unlocked[0];
        gameObject.SetActive(false);
    }



    public void BroClicked(int broNum) {
        selectedBro = broNum;
        broName.text = bros[broNum];
        Shift();
        ChangeAlpha();
        lockImg.enabled = !unlocked[selectedBro];
        ownedImg.enabled = unlocked[selectedBro];
        unlockButton.interactable = !unlocked[selectedBro];
    }



    public void UnlockClicked() {
        string buyId = "com.bombbrosinc.bombbros." + bros[selectedBro]; //Fix
        purchaser.BuyProductID(buyId);
    }



    public void UpdateShop() {
        ownedImg.enabled = unlocked[selectedBro];
        lockImg.enabled = !unlocked[selectedBro];
        unlockButton.interactable = !unlocked[selectedBro];
    }



    public void RestoreClicked() {
        purchaser.RestorePurchases();
    }



    void Shift() {
        for (int i = 0; i < trans.Length; i++) {
            float broY = trans[i].anchoredPosition.y;
            trans[i].anchoredPosition = new Vector2((i - selectedBro) * shiftAmt, broY);
        }
    }



    void ChangeAlpha() {
        for (int i = 0; i < imgs.Length; i++) {
            float r = imgs[i].color.r;
            float g = imgs[i].color.g;
            float b = imgs[i].color.b;

            if (i - 2 == selectedBro || i + 2 == selectedBro) {
                imgs[i].color = new Color(r, g, b, 0.6f);
            }

            else if (i - 1 == selectedBro || i + 1 == selectedBro) {
                imgs[i].color = new Color(r, g, b, 0.8f);
            }
            else {
                imgs[i].color = new Color(r, g, b, 1);
            }
        }
    }
}
