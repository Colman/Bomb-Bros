using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Intro : MonoBehaviour {

    public PhysicsMaterial2D secondMat;
    public ParticleSystem sparkler;
    public Light match;
    public Light fuseLight;
    public GameObject bomb;
    public GameObject sparklerObj;
    public AudioSource fuseSound;
    public Image faderImg;


    void Start() {
        StartCoroutine(IntroUpdate());
    }



    IEnumerator IntroUpdate() {
        yield return new WaitForSeconds(1.2f);
        match.enabled = true;
        for (int i = 0; i < 20; i++) {
            match.intensity += 1;
            yield return new WaitForSeconds(0.03f);
        }
        yield return new WaitForSeconds(0.4f);
        fuseLight.enabled = true;
        sparkler.gameObject.SetActive(true);
        match.enabled = false;
        yield return new WaitForSeconds(0.72f);
        bomb.GetComponent<Rigidbody2D>().velocity = new Vector2(7, 0);
        yield return new WaitForSeconds(1.5f);
        bomb.GetComponent<Rigidbody2D>().sharedMaterial = secondMat;
        yield return new WaitForSeconds(5.5f);
        for (int i = 0; i < 20; i++) {
            Color c = faderImg.color;
            c.a += 0.05f;
            faderImg.color = c;
            fuseSound.volume -= 0.05f;
            yield return new WaitForSeconds(0.05f);
        }
        SceneManager.LoadSceneAsync("Menu");
    }
}
