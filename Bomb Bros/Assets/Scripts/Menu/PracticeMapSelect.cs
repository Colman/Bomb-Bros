using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PracticeMapSelect : MonoBehaviour {

    //PracticeMapSelect var
    public static string level;


    public void JunglePressed() {
        level = "Jungle";
    }



    public void FactoryPressed() {
        level = "Factory";
    }



    public void TemplePressed() {
        level = "Temple";
    }



    public void ForestPressed() {
        level = "Forest";
    }



    public void VolcanoPressed() {
        level = "Volcano";
    }
}
