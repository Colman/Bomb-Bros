using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Music : MonoBehaviour {

    void Start() {
        if (PlayerPrefs.HasKey("MusicVol")) {
            float vol = PlayerPrefs.GetFloat("MusicVol");
            GetComponent<Slider>().value = vol;
            AudioSource src = GameObject.Find("Main Camera").GetComponent<AudioSource>();
            src.volume = vol;
        }
        else {
            GetComponent<Slider>().value = 0.5f;
            AudioSource src = GameObject.Find("Main Camera").GetComponent<AudioSource>();
            src.volume = 0.5f;
        }
    }

	public void VolChanged() {
        float vol = GetComponent<Slider>().value;
        AudioSource src = GameObject.Find("Main Camera").GetComponent<AudioSource>(); 
        src.volume = vol;
        PlayerPrefs.SetFloat("MusicVol", vol);
        PlayerPrefs.Save(); 
    }
}
