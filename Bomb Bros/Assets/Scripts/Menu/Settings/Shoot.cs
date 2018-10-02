using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Shoot : MonoBehaviour {
    AudioSource SfxSound;
    float lastVol;
    bool first;

    void Start() {
        if (PlayerPrefs.HasKey("ShootSens")) {
            float sens = PlayerPrefs.GetFloat("ShootSens");
            GetComponent<Slider>().value = sens;
        }
        else {
            GetComponent<Slider>().value = 0.5f;
        }
    }



    public void SensChanged() {
        float sens = GetComponent<Slider>().value;
        PlayerPrefs.SetFloat("ShootSens", sens);
        PlayerPrefs.Save();
    }
}
