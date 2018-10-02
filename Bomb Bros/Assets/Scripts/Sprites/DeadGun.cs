using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeadGun : MonoBehaviour {

    //Physics var
    Rigidbody2D rb;

    //Photon var
    PhotonView deadGunView;
    

    void Awake () {
        //Physics init
        rb = GetComponent<Rigidbody2D>();

        //Photon init
        deadGunView = GetComponent<PhotonView>();
    }


    void Start() {
        rb.AddForce((Vector2)deadGunView.instantiationData[0], ForceMode2D.Impulse);
        rb.angularVelocity = (float)deadGunView.instantiationData[1];
        Destroy(gameObject, (float)deadGunView.instantiationData[2]);
    }
}
