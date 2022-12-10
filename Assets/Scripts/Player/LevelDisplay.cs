using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelDisplay : MonoBehaviour
{
    private Text text;

    private void Start() {
        text = GetComponent<Text>();
    }
    private void FixedUpdate() {
        if(PlayerPrefs.GetInt("Language") == 0){
            text.text = "Level: " + PlayerPrefs.GetInt("PlayerLevel");
        }
        else if(PlayerPrefs.GetInt("Language") == 1){
            text.text = "Уровень: " + PlayerPrefs.GetInt("PlayerLevel");
        }
    }
}
