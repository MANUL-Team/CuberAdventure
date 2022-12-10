using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelUpText : MonoBehaviour
{
    private Text text;
    private int oldLevel, newLevel;

    private void Start() {
        text = GetComponent<Text>();
        oldLevel = PlayerPrefs.GetInt("PlayerLevel") - 1;
        newLevel = PlayerPrefs.GetInt("PlayerLevel");
        if(PlayerPrefs.GetInt("Language") == 0){
            text.text = "Level: " + oldLevel;
        }
        else if(PlayerPrefs.GetInt("Language") == 1){
            text.text = "Уровень: " + oldLevel;
        }
    }
    public void LevelUpForText(){
        if(PlayerPrefs.GetInt("Language") == 0){
            text.text = "Level: " + newLevel;
        }
        else if(PlayerPrefs.GetInt("Language") == 1){
            text.text = "Уровень: " + newLevel;
        }
    }
    public void CloseText(){
        gameObject.SetActive(false);
    }

}
