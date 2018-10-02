using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Missile : MonoBehaviour {

    //Photon var
    PhotonView missileView;

    //Rotation var
    Quaternion rot;

    //Missile vars
    int fromId;
    bool fromBlue;
    public bool isOwner;


    void Awake() {
        //Photon init
        missileView = GetComponent<PhotonView>();

        //Rotation init
        rot = Quaternion.LookRotation(new Vector3(0, 0, 1));      
    }



    void Start() {
        //Missile inits
        fromId = (int)missileView.instantiationData[0];
        fromBlue = (bool)missileView.instantiationData[1];


        GetComponent<Rigidbody2D>().velocity = (Vector2)missileView.instantiationData[2];
        GetComponent<AudioSource>().volume = PlayerPrefs.GetFloat("SfxVol");
        Destroy(gameObject, 8);
    }



    void Update() {
        Vector2 vel = GetComponent<Rigidbody2D>().velocity;
        float ang = Mathf.Atan2(vel.y, vel.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.AngleAxis(ang - 90f, new Vector3(0, 0, 1));
    }



    void OnTriggerEnter2D(Collider2D other) {
        if(other.gameObject.tag != "Projectile" && other.gameObject.tag != "Pickup") {
            float expX = transform.position.x;
            float expY = transform.position.y;
            Vector3 expPos = new Vector3(expX, expY, 0);
            if (isOwner) {
                object[] expArgs = {
                    fromId,
                    fromBlue
                };
                PhotonNetwork.Instantiate("Explosion", expPos, rot, 0, expArgs);
            }
            Destroy(gameObject);
        } 
    }
}
