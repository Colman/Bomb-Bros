using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bro : MonoBehaviour {

    //Bro Vars
    public bool isMe = false;
    public bool isBlue;
    public Vector3 respawnLoc;
    public float moveSpeed; //Public only for AI
    public float jumpSpeed; //Public only for AI

    //Health and dying vars
    public float maxHealth;
    public float health;
    public bool isDead;
    float dieLength;
    float diedAt;
    public int kills;
    public int deaths;

    //Anim vars
    Animator anim;
    public bool onGround; //Public only for AI
    bool isFlipped;
    bool isMoving;
    int airHash;
    int moveHash;

    //Audio var
    AudioSource dieSound;

    //Physics vars
    public Rigidbody2D rb;
    Collider2D col;
    ContactFilter2D conFil;
    Collider2D[] contact;
    float aliveMass;
    float deadMass;

    //Gun vars
    int gunType;
    public int pickupType;
    float shootTime;
    float shootDuration;
    float flashTime;
    float flashDuration;
    Transform gunArm;
    public float fireRad;
    public GameObject deadGun;

    //Photon var
    public PhotonView broView;

    //Rotation var
    Quaternion rot;

    //Script vars
    Game game;

    //Health bar vars
    public GameObject backBar;
    public SpriteRenderer frontBar;


    void Awake() {
        //Bro inits
        moveSpeed = 3.5f;
        jumpSpeed = 12;

        //Health and dying inits
        maxHealth = 100;
        health = maxHealth;
        isDead = false;
        dieLength = 5;
        diedAt = 0;

        //Anim inits
        anim = GetComponent<Animator>();
        onGround = false;
        isFlipped = false;
        isMoving = false;
        airHash = Animator.StringToHash("AirBorne");
        moveHash = Animator.StringToHash("Moving");

        //Audio init
        dieSound = GetComponent<AudioSource>();

        //Physics inits
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<Collider2D>();
        conFil = new ContactFilter2D().NoFilter();
        contact = new Collider2D[1];
        aliveMass = 0.8f;
        deadMass = 0.6f;

        //Gun inits
        gunType = 0;
        pickupType = -1;
        shootTime = 0;
        shootDuration = 0.2f;
        flashTime = 0;
        flashDuration = 0.05f;
        gunArm = transform.Find("GunArm");
        fireRad = 1.5f;

        //Photon init
        broView = GetComponent<PhotonView>();

        //Rotation init
        rot = Quaternion.LookRotation(new Vector3(0, 0, 1));
    }



    void Start() {
        //Game init
        game = GameObject.Find("Main Camera").GetComponent<Game>();


        dieSound.volume = PlayerPrefs.GetFloat("SfxVol");
        rb.mass = aliveMass;
        StartCoroutine(CheckFlash());
    }



    [PunRPC]
    public void SetTeam(bool team) {
        isBlue = team;
        if (PhotonNetwork.offlineMode) {
            if (isBlue) {
                backBar.SetActive(false);
            }
            else {
                frontBar.color = new Color32(0xF9, 0x35, 0x35, 0xFF);
            }
        }
        else if (broView.isMine) {
            backBar.SetActive(false);
        }
        else {
            if (isBlue) {
                frontBar.color = new Color32(0, 0x30, 0xFF, 0xFF);
            }
            else {
                frontBar.color = new Color32(0xF9, 0x35, 0x35, 0xFF);
            }
        }
    }



    void Update() {
        if (broView.isMine) { //Have to do enemy bros too in practice mode
            if (col.OverlapCollider(conFil, contact) == 0) {
                onGround = false;
                anim.SetBool(airHash, !onGround);
            }

            if (Mathf.Abs(rb.velocity.x) > 0.1 && onGround && !isDead) {
                if (!isMoving) {
                    anim.SetBool(moveHash, true);
                    isMoving = true;
                }
            }
            else if (isMoving) {
                anim.SetBool(moveHash, false);
                isMoving = false;
            }

            if (isDead) {
                if (Time.time - diedAt >= dieLength) {
                    Respawn();
                }
            }
        }
    }



    public void Move(float mult) {
        if (!isDead) {
            if (mult < 0) {
                if (!isFlipped) {
                    broView.RPC("FlipRPC", PhotonTargets.All, true);
                }
            }
            if (mult > 0) {
                if (isFlipped) {
                    broView.RPC("FlipRPC", PhotonTargets.All, false);
                }
            }
            rb.velocity = new Vector2(mult * moveSpeed, rb.velocity.y);
        }
    }



    public void Jump() {
        if (onGround && !isDead) {
            rb.velocity = new Vector2(rb.velocity.x, jumpSpeed);
        }
    }



    void OnCollisionStay2D(Collision2D collision) {
        foreach (ContactPoint2D point in collision.contacts) {
            Vector2 norm = point.normal;
            float ang = Mathf.Atan2(norm.y, norm.x) * Mathf.Rad2Deg;
            if (ang >= 45 && ang <= 135) {
                onGround = true;
                anim.SetBool(airHash, !onGround);
                return;
            }
        }
        onGround = false;
        anim.SetBool(airHash, !onGround);
    }



    public void ChangeHealth(float amount, int fromId, int dieType, Vector2 forceVec) {
        health -= amount;

        broView.RPC("ChangeHealthBar", PhotonTargets.All, health, maxHealth);
        if (health <= 0 && !isDead) {
            broView.RPC("AddDeath", PhotonTargets.All);
            if (fromId != broView.viewID) {
                GameObject[] bros = GameObject.FindGameObjectsWithTag("Bro");
                foreach (GameObject b in bros) {
                    PhotonView bView = b.GetComponent<PhotonView>();
                    if (bView.viewID == fromId) {
                        bView.RPC("AddKill", PhotonTargets.All);
                        break;
                    }
                }
            }
            Die(dieType, forceVec);
        }
    }



    [PunRPC]
    public void AddKill() {
        kills++;
    }



    [PunRPC]
    public void AddDeath() {
        deaths++;
    }



    [PunRPC]
    public void ChangeHealthBar(float a_health, float a_maxHealth) {
        float tempHealth = a_health / a_maxHealth;
        if (tempHealth < 0) {
            tempHealth = 0;
        }
        if (tempHealth > 1) {
            tempHealth = 1;
        }
        Vector3 prevScale = frontBar.transform.localScale;
        frontBar.transform.localScale = new Vector3(tempHealth, prevScale.y, prevScale.z);
    }



    [PunRPC]
    public void ChangeDeadGun(bool show) {
        if (show) {
            foreach (Transform child in gunArm) {
                if (child.name == "Type" + gunType) {
                    child.gameObject.SetActive(true);
                    break;
                }
            }
        }
        else {
            foreach (Transform child in gunArm) {
                child.gameObject.SetActive(false);
            }
        }
    }



    void Die(int dieType, Vector2 forceVec) {
        isDead = true;
        rb.mass = deadMass;
        rb.freezeRotation = false;
        broView.RPC("ChangeDeadGun", PhotonTargets.All, false);
        dieSound.Play();

        object[] deadGunArgs = new object[3];
        if (dieType == 0) { //From explosion
            deadGunArgs[0] = forceVec;
            deadGunArgs[1] = Random.value * 180f - 90f;
            deadGunArgs[2] = dieLength;
        }
        if (dieType == 1) { //From fire
            deadGunArgs[0] = Vector2.zero;
            deadGunArgs[1] = 0;
            deadGunArgs[2] = dieLength;
        }
        PhotonNetwork.Instantiate(game.gunNames[gunType], transform.position, rot, 0, deadGunArgs);

        diedAt = Time.time;
    }



    void Respawn() {
        isDead = false;
        rb.mass = aliveMass;
        rb.freezeRotation = true;

        broView.RPC("ChangeDeadGun", PhotonTargets.All, true);
        transform.position = respawnLoc;
        transform.rotation = rot;
        rb.velocity = new Vector2(0, 0);
        onGround = false;

        health = maxHealth;
        broView.RPC("ChangeHealthBar", PhotonTargets.All, health, maxHealth);
        broView.RPC("FlipRPC", PhotonTargets.All, false);
    }



    [PunRPC]
    public void SwapGun(int type) {
        gunType = type;

        foreach (Transform child in gunArm) {
            if (child.name == "Type" + type) {
                child.gameObject.SetActive(true);
                child.GetChild(0).gameObject.SetActive(false);
            }
            else {
                child.gameObject.SetActive(false);
            }
        }
    }



    IEnumerator CheckFlash() {
        while (true) {
            if (flashTime != 0) {
                foreach (Transform child in gunArm) {
                    child.GetChild(0).gameObject.SetActive(false);
                }
                flashTime = 0;
            }
            yield return new WaitForSeconds(flashDuration);
        }
    }



    public bool Fire(Vector2 projPos, Vector2 projVel) {
        if (Time.time - shootTime < shootDuration) {
            return false;
        }

        Vector2 projDir = projVel.normalized;
        Vector2 projNorm = new Vector2();

        if (isFlipped) {
            projNorm.x = projDir.y;
            projNorm.y = -1 * projDir.x;
        }
        else {
            projNorm.x = -1 * projDir.y;
            projNorm.y = projDir.x;
        }

        float projAng = Mathf.Atan2(projDir.y, projDir.x);
        if (projAng < 0 && projAng > -Mathf.PI) {
            float dis = Mathf.Abs(projAng + Mathf.PI / 2) * (-0.35f / Mathf.PI / 2) + 0.5f;
            projNorm = projNorm.normalized * dis;
        }
        else {
            projNorm = projNorm.normalized * 0.15f;
        }

        Vector2 newProjPos = projPos + projNorm;

        if (projVel.x < 0 && !isFlipped) {
            GetComponent<PhotonView>().RPC("FlipRPC", PhotonTargets.All, true);
        }
        else if (projVel.x >= 0 && isFlipped) {
            GetComponent<PhotonView>().RPC("FlipRPC", PhotonTargets.All, false);
        }


        if (isFlipped) {
            projAng += Mathf.PI;
        }

        Quaternion armRot = Quaternion.AngleAxis(projAng * Mathf.Rad2Deg, new Vector3(0, 0, 1));
        GetComponent<PhotonView>().RPC("ArmRPC", PhotonTargets.All, armRot);

        foreach (Transform child in gunArm) {
            if (child.gameObject.activeInHierarchy == true) {
                child.GetChild(0).gameObject.SetActive(true);
                break;
            }
        }

        if (isFlipped) {
            projAng -= Mathf.PI;
        }
        Quaternion projAngQuat = Quaternion.AngleAxis(projAng * Mathf.Rad2Deg - 90f, new Vector3(0, 0, 1));

        object[] projArgs = {
            broView.viewID,
            isBlue,
            projVel
        };
        GameObject proj = PhotonNetwork.Instantiate(game.projNames[gunType], newProjPos, projAngQuat, 0, projArgs);
        if (gunType == 0) {
            proj.GetComponent<Missile>().isOwner = true;
        }
        if (gunType == 1) {
            proj.GetComponent<Molotov>().isOwner = true;
        }

        flashTime = Time.time;
        shootTime = Time.time;
        return true;
    }



    [PunRPC]
    public void ArmRPC(Quaternion armRot) {
        gunArm.localRotation = armRot;
    }



    [PunRPC]
    public void FlipRPC(bool left) {
        Flip(transform, left, true);
    }



    public void Flip(Transform trans, bool left, bool first) {
        if (!first || left != isFlipped) {
            isFlipped = left;
            if (first) {
                trans.GetComponent<SpriteRenderer>().flipX = left;
            }
            foreach (Transform child in trans) {
                if (child.gameObject.name != "HealthBack") {
                    child.GetComponent<SpriteRenderer>().flipX = left;

                    Vector3 spritePos = child.localPosition;
                    child.localPosition = new Vector3(-1 * spritePos.x, spritePos.y, spritePos.z);

                    Flip(child, left, false);
                }
            }
        }
    }
}
