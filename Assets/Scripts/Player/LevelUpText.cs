using System.Collections;
using System.Collections.Generic;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.UI;

public class LevelUpText : MonoBehaviour
{
    private Text text;
    private int oldLevel, newLevel, lang;

    private void Start() {
        text = GetComponent<Text>();
        lang = PlayerPrefs.GetInt("Language");
    }
    public void OldLevel(){
        oldLevel = PlayerPrefs.GetInt("PlayerLevel") - 1;
        newLevel = PlayerPrefs.GetInt("PlayerLevel");
        switch (lang)
        {
            case 0:
                text.text = $"Уровень: {oldLevel}";
                break;
            default:
                text.text = $"Level: {oldLevel}";
                break;
        }
    }
    public void LevelUpForText(){
        switch (lang)
        {
            case 0:
                text.text = $"Уровень: {newLevel}";
                break;
            default:
                text.text = $"Level: {newLevel}";
                break;
        }
    }
    public void CloseText(){
        gameObject.SetActive(false);
    }

}
