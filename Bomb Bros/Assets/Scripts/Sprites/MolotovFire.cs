using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MolotovFire : MonoBehaviour {

    //Explosion vars
    float duration;
    float tickLength;
    float radius;
    float damage;

    //Anim var
    Animator anim;

    //Sound var
    AudioSource fireSound;

    //Photon var
    PhotonView fireView;

    //Explosion vars
    int fromId;
    bool fromBlue;


    void Awake() {
        //Explosion inits
        duration = 5;
        tickLength = 0.25f;
        radius = 0.5f;
        damage = 10f;

        //Anim init
        anim = GetComponent<Animator>();

        //Sound init
        fireSound = GetComponent<AudioSource>();

        //Photon init
        fireView = GetComponent<PhotonView>();
    }



    void Start() {
        //Explosion inits
        fromId = (int)fireView.instantiationData[0];
        fromBlue = (bool)fireView.instantiationData[1];
        

        float startTime = Random.value;
        anim.Play("Default", -1, startTime);
        fireSound.volume = PlayerPrefs.GetFloat("SfxVol");

        StartCoroutine(ApplyDamage());
        Destroy(gameObject, duration);
    }



    IEnumerator ApplyDamage() {
        while (true) {
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
                if (rb.gameObject.tag == "Bro") {
                    Bro bro = rb.gameObject.GetComponent<Bro>();
                    if (bro.isMe || bro.isBlue != fromBlue) {
                        bro.ChangeHealth(damage, fromId, 0, Vector2.zero);
                    }
                }
            }
            yield return new WaitForSeconds(tickLength);
        }
    }
}
