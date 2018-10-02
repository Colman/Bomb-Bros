using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PracticeDifficulty : MonoBehaviour {

    //PracticeDifficulty var
    public static int difficulty;


	public void DifficultyPressed(int diff) {
        difficulty = diff;
        SceneManager.LoadSceneAsync(PracticeMapSelect.level);
    }
}
