using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Sfx : MonoBehaviour {

    AudioSource SfxSound;
    float lastVol;
    bool first;

    void Start() {
        SfxSound = GetComponent<AudioSource>();
        first = true;
        if (PlayerPrefs.HasKey("SfxVol")) {
            float vol = PlayerPrefs.GetFloat("SfxVol");
            GetComponent<Slider>().value = vol;
            lastVol = vol;
        }
        else {
            GetComponent<Slider>().value = 0.5f;
            lastVol = 0.5f;
        }
    }



    public void VolChanged() {
        if(!first && Mathf.Abs(GetComponent<Slider>().value - lastVol) > 0.05f) {
            float vol = GetComponent<Slider>().value;
            SfxSound.volume = vol;
            SfxSound.Play();
            PlayerPrefs.SetFloat("SfxVol", vol);
            PlayerPrefs.Save();
            lastVol = vol;
        }
        first = false;
    }
}
