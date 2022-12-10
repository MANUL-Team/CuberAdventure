using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SkillPoints : MonoBehaviour
{
    private Text text;
    private void Start() {
        text = GetComponent<Text>();
    }
    private void FixedUpdate() {
        if(PlayerPrefs.GetInt("Language") == 0){
            text.text = "Skill points: " + PlayerPrefs.GetInt("SkillPoints");
        }
        else if(PlayerPrefs.GetInt("Language") == 1){
            text.text = "Очки навыков: " + PlayerPrefs.GetInt("SkillPoints");
        }
    }
}
