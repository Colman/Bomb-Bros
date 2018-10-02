using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tutorial : MonoBehaviour {

    //Tutorial vars
    int swipeDis;
    bool released;
    int page;

    //StartPos var
    Vector2 startPos;

    //UI vars
    public GameObject mainMenu;
    public GameObject tutorialOne;
    public GameObject tutorialTwo;
    public GameObject tap1;
    public GameObject tap2;
    bool isPressed;


    void Awake() {
        //Tutorial inits
        swipeDis = 20;
        released = false;
        page = 1;
    }



    void Start() {
        startPos = tap1.GetComponent<RectTransform>().anchoredPosition;
    }



    void Update() {
        GetInput();
        if(page == 0) {
            PageOne();
        }
        if (page == 1 && isPressed && released) {
            StartCoroutine(PageTwo());
            released = false;
        }
        if (page == 2 && isPressed && released) {
            ToMain();
            released = false;
        }
    }


    
    void GetInput() {
        if (Menu.isPc) {
            isPressed = Input.GetMouseButton(0);
        }
        else {
            foreach (Touch touch in Input.touches) {
                if (touch.phase != TouchPhase.Ended) {
                    isPressed = true;
                    break;
                }
                else {
                    isPressed = false;
                }
            }
        }
        if (!isPressed && !released) {
            released = true;
        }
    }


    void PageOne() {
        page = 1;
        mainMenu.SetActive(false);
        tutorialOne.SetActive(true);
    }


    IEnumerator PageTwo() {
        page = 2;
        tutorialOne.SetActive(false);
        tutorialTwo.SetActive(true);
        tap1.GetComponent<RectTransform>().anchoredPosition = startPos;
        tap2.GetComponent<RectTransform>().anchoredPosition = startPos;
        tap1.SetActive(true);
        tap2.SetActive(false);
        while (true) {
            yield return new WaitForSeconds(0.5f);
            tap1.SetActive(false);
            tap2.SetActive(true);
            for (int j = 0; j < swipeDis; j++) {
                Vector2 currPos = tap2.GetComponent<RectTransform>().anchoredPosition;
                tap2.GetComponent<RectTransform>().anchoredPosition = currPos + new Vector2(4, 4);
                yield return new WaitForSeconds(0.05f);
            }
            tap1.GetComponent<RectTransform>().anchoredPosition = tap2.GetComponent<RectTransform>().anchoredPosition;
            tap2.SetActive(false);
            tap1.SetActive(true);
            yield return new WaitForSeconds(0.5f);
            tap1.GetComponent<RectTransform>().anchoredPosition = startPos;
            tap2.GetComponent<RectTransform>().anchoredPosition = startPos;
        }
    }



    void ToMain() {
        page = 0;
        tutorialTwo.SetActive(false);
        StopCoroutine(PageTwo());
        gameObject.SetActive(false);
        mainMenu.SetActive(true);
    }
}
