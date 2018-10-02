using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pickup : MonoBehaviour {

    //Physics var
    Rigidbody2D rb;

    //Movement function vars
    float a;
    float b;
    float startY;
    float y;

    //Script vars
    HUD hud;

    //Pickup var
    public int pickupType;


    void Awake() {
        //Physics init
        rb = GetComponent<Rigidbody2D>();

        //Movement function inits
        a = 0.05f;
        b = 1.7f;
        startY = rb.position.y;
        y = startY;  
    }
	


    void Start() {
        //Script inits
        hud = GameObject.Find("HUD").GetComponent<HUD>();
    }



	void Update () {
        y = (a * Mathf.Sin(Time.time * b));
        rb.position = new Vector2(rb.position.x, startY + y);
    }



    void OnTriggerEnter2D(Collider2D other) {
        Bro bro = other.gameObject.GetComponent<Bro>();
        if (bro != null) {
            if (bro.isMe) {
                hud.ChangeUseButton(true);
                bro.pickupType = pickupType;
            }
        }
    }



    void OnTriggerExit2D(Collider2D other) {
        Bro bro = other.gameObject.GetComponent<Bro>();
        if (bro != null) {
            if (bro.isMe) {
                hud.ChangeUseButton(false);
                bro.pickupType = -1;
            }
        }
    }
}
