using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Explosion : MonoBehaviour {

    //Explosion vars
    float duration;
    float explosionForce;
    float radius;
    float damage;

    //Sound var
    AudioSource expSound;

    //Photon var
    PhotonView expView;

    //Explosion vars
    int fromId;
    bool fromBlue;
    
    
    void Awake() {
        //Explosion inits
        duration = 2;
        explosionForce = 15;
        radius = 3;
        damage = 40;

        //Sound init
        expSound = GetComponent<AudioSource>();

        //Photon init
        expView = GetComponent<PhotonView>();
    }


    void Start() {
        //Explosion inits
        fromId = (int)expView.instantiationData[0];
        fromBlue = (bool)expView.instantiationData[1];


        expSound.volume = PlayerPrefs.GetFloat("SfxVol");
        Destroy(gameObject, duration);
        ApplyForce();
    }


    void ApplyForce() {
        Vector2 pos2D = new Vector2(transform.position.x, transform.position.y);

        ContactFilter2D conFil = new ContactFilter2D();
        conFil.SetLayerMask(LayerMask.GetMask("Bros"));
        Collider2D[] results = new Collider2D[10];
        int numCol = Physics2D.OverlapCircle(pos2D, radius, conFil, results);
        List<Rigidbody2D> rigidbodies = new List<Rigidbody2D>();
        
        for (int i = 0; i < numCol; i++) {
            if (results[i].attachedRigidbody != null && !rigidbodies.Contains(results[i].attachedRigidbody)) {
                rigidbodies.Add(results[i].attachedRigidbody);
            }
        }

        foreach (Rigidbody2D rb in rigidbodies) {

            Vector2 posVec = new Vector2(rb.position.x - transform.position.x,
                rb.position.y - transform.position.y);

            Vector2 forceVec;
            if (posVec.magnitude < 0.6f) {
                forceVec = posVec.normalized / 0.6f * explosionForce;
            }
            else {
                forceVec = posVec.normalized / posVec.magnitude * explosionForce;
            }


            if (rb.gameObject.tag == "Bro") {
                Bro bro = rb.gameObject.GetComponent<Bro>();
                if (bro.isMe || bro.isBlue != fromBlue) {
                    bro.ChangeHealth(1f / posVec.magnitude * damage, fromId, 0, forceVec);
                }
            }
            

            rb.AddForce(forceVec, ForceMode2D.Impulse);
            rb.angularVelocity = Random.value * 180f - 90f;
        }
    }
}
