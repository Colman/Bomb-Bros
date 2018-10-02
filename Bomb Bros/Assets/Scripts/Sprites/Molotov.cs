using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Molotov : MonoBehaviour {

    //Photon var
    PhotonView molotovView;

    //Rotation var
    Quaternion rot;

    //Molotov vars
    int fromId;
    bool fromBlue;
    public bool isOwner;


    void Awake() {
        //Photon init
        molotovView = GetComponent<PhotonView>();

        //Rotation init
        rot = Quaternion.LookRotation(new Vector3(0, 0, 1));
    }



    void Start() {
        //Molotov inits
        fromId = (int)molotovView.instantiationData[0];
        fromBlue = (bool)molotovView.instantiationData[1];


        GetComponent<Rigidbody2D>().velocity = (Vector2)molotovView.instantiationData[2];
        GetComponent<AudioSource>().volume = PlayerPrefs.GetFloat("SfxVol");
        Destroy(gameObject, 8);
    }


    void Update() {
        Vector2 vel = GetComponent<Rigidbody2D>().velocity;
        float ang = Mathf.Atan2(vel.y, vel.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.AngleAxis(ang - 90f, new Vector3(0, 0, 1));
    }



    void OnTriggerEnter2D(Collider2D other) {
        if (other.gameObject.tag != "Projectile" && other.gameObject.tag != "Pickup") {
            float fireX = transform.position.x;
            float fireY = transform.position.y;
            Vector3 firePos = new Vector3(fireX, fireY, 0);
            if (isOwner) {
                object[] fireArgs = {
                    fromId,
                    fromBlue
                };
                PhotonNetwork.Instantiate("MolotovFire", firePos, rot, 0, fireArgs);
            }
            Destroy(gameObject);
        }
    }
}

