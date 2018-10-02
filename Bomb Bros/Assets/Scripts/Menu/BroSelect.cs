using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BroSelect : MonoBehaviour {

    //BroSelect var
    public static int selectedBro;

    //Bro vars
    string[] broNames;
    int shiftAmt;
    
    //UI vars
    public RectTransform[] trans;
    public Image[] imgs;
    public Image lockImg;
    public Text broName;
    public Button selectButton;
    

    void Awake () {
        //BroSelect init
        selectedBro = 0;

        //Bro inits
        broNames = new string[] {
            "007",
            "Sam",
            "Dracula",
            "Evan",
            "Haru",
            "Harry",
            "Santa",
            "Genie",
            "Trump",
            "Jesus",
            "Dougie"
        };
        shiftAmt = 220;
    }



    void OnEnable() {
        BroClicked(selectedBro);
    }



    public void BroClicked(int broNum) {
        selectedBro = broNum;
        broName.text = broNames[broNum];
        Shift();
        ChangeAlpha();
        if(broNum >= 8) {
            if (Shop.unlocked[broNum - 8]) {
                selectButton.interactable = true;
                lockImg.enabled = false;
            }
            else {
                selectButton.interactable = false;
                lockImg.enabled = true;
            }
        }
        else {
            selectButton.interactable = true;
            lockImg.enabled = false;
        } 
    }



    void Shift() {
        for(int i = 0; i < trans.Length; i++) {
            float broY = trans[i].anchoredPosition.y;
            trans[i].anchoredPosition = new Vector2((i - selectedBro) * shiftAmt, broY);
        }  
    }



    void ChangeAlpha() {
        for (int i = 0; i < trans.Length; i++) {
            float r = imgs[i].color.r;
            float g = imgs[i].color.g;
            float b = imgs[i].color.b;

            if(i - 2 == selectedBro || i + 2 == selectedBro) {
                imgs[i].color = new Color(r, g, b, 0.6f);
            }

            else if(i - 1 == selectedBro || i + 1 == selectedBro) {   
                imgs[i].color = new Color(r, g, b, 0.8f);
            }
            else {
                imgs[i].color = new Color(r, g, b, 1);
            }
        }
    }
}
