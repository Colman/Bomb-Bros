using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Login : MonoBehaviour {

    public InputField nameField;


    void OnEnable() {
        nameField.text = PlayerPrefs.GetString("DisplayName");
    }



    public void OnNameChange() {
        PlayerPrefs.SetString("DisplayName", nameField.text);
        PlayerPrefs.Save();
    }
}
