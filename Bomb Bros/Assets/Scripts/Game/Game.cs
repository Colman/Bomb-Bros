using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Game : MonoBehaviour {

    //Spawning vars
    Vector3 blueSpawn;
    Vector3 redSpawn;

    //Player var (For Practice)
    public GameObject player;

    //Standard roatation for spawning
    Quaternion rot;

    //Gun and proj vars
    public string[] gunNames;
    public GameObject[] gunPrefabs;
    public string[] projNames;
    public GameObject[] projPrefabs;

    //Bro vars
    public string[] broNames;
    int enemyBroType;

    //Audio vars
    AudioSource music;
    public AudioClip practiceClip;
    public AudioClip multiplayerClip;
    

    void Awake() {
        //Spawning inits
        blueSpawn = new Vector3(-4, 0, 0);
        redSpawn = new Vector3(4, 0, 0);

        //Rot init
        rot = Quaternion.LookRotation(new Vector3(0, 0, 1));

        //Gun and proj inits
        gunNames = new string[] {
            "RocketLauncher",
            "MolotovLauncher"
        };
        projNames = new string[] {
            "Missile",
            "Molotov"
        };

        //Bro init
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

        //Audio init
        music = GetComponent<AudioSource>();


        if (PhotonNetwork.offlineMode) {
            music.volume = PlayerPrefs.GetFloat("MusicVol") * 0.5f;
            music.clip = practiceClip;
        }
        else {
            music.volume = PlayerPrefs.GetFloat("MusicVol") * 0.5f;
            music.clip = multiplayerClip;
        }
        music.Play();

        if (PhotonNetwork.offlineMode) {
            PhotonNetwork.JoinOrCreateRoom("PracticeRoom", new RoomOptions(), new TypedLobby());
        }
        else {
            CreateSelf();
        }
        PhotonNetwork.isMessageQueueRunning = true;
    }

    void OnJoinedRoom() {
        CreateSelf();
        StartPractice();
    }



    void StartPractice() {
        System.Random rand = new System.Random();

        if (BroSelect.selectedBro == 0) {
            enemyBroType = rand.Next(1, broNames.Length);
        }
        else if (BroSelect.selectedBro == broNames.Length - 1) {
            enemyBroType = rand.Next(0, broNames.Length - 1);
        }
        else {
            int randNum = rand.Next(2);
            if (randNum == 0) {
                enemyBroType = rand.Next(0, BroSelect.selectedBro);
            }
            else {
                enemyBroType = rand.Next(BroSelect.selectedBro + 1, broNames.Length);
            }
        }

        CreateEnemy();
    }



    void CreateSelf() {
        //Get spawn point
        Vector3 selfSpawn;
        if (PhotonNetwork.offlineMode) {
            selfSpawn = blueSpawn;
        }
        else {
            if (RoomWait.isBlue) {
                selfSpawn = blueSpawn;
            }
            else {
                selfSpawn = redSpawn;
            }
        }

        //Create self
        GameObject self;
        self = PhotonNetwork.Instantiate(broNames[BroSelect.selectedBro], selfSpawn, rot, 0);
        self.GetComponent<BoxCollider2D>().isTrigger = false;
        self.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Dynamic;

        //Setup bro component
        Bro bro = self.GetComponent<Bro>();
        bro.isMe = true;
        bro.respawnLoc = selfSpawn;

        if (PhotonNetwork.offlineMode) {
            bro.GetComponent<PhotonView>().RPC("SetTeam", PhotonTargets.AllBuffered, true);
        }
        else {
            bro.GetComponent<PhotonView>().RPC("SetTeam", PhotonTargets.AllBuffered, RoomWait.isBlue);
        }

        //Get player (for practice mode)
        if (PhotonNetwork.offlineMode) {
            player = self;
        }
    }



    void CreateEnemy() {
        //Create enemy
        GameObject enemy = PhotonNetwork.Instantiate(broNames[enemyBroType], redSpawn, rot, 0);
        enemy.AddComponent<Enemy>();
        enemy.GetComponent<BoxCollider2D>().isTrigger = false;
        enemy.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Dynamic;

        //Setup bro component
        Bro bro = enemy.GetComponent<Bro>();
        bro.isMe = false;
        bro.respawnLoc = redSpawn;
        bro.GetComponent<PhotonView>().RPC("SetTeam", PhotonTargets.AllBuffered, false); 
    }
}