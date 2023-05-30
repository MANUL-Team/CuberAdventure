using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LangText : MonoBehaviour
{
    private Text text;
    private string lang;
    [TextArea]
    [SerializeField] private string eng, ru;
    [SerializeField] private string[] engEd, ruEd;
    [SerializeField] private bool editable, dialog;

    private void Awake() {
        lang = PlayerPrefs.GetString("Language");
        text = GetComponent<Text>();
        if(editable){
            if(lang == "en_US"){
                if(text.text == ruEd[0]){
                    text.text = engEd[0];
                }
                else if(text.text == ruEd[1]){
                    text.text = engEd[1];
                }
                else if(text.text == ruEd[2]){
                    text.text = engEd[2];
                }
            }
            else if(lang == "ru_RU"){
                if(text.text == engEd[0]){
                    text.text = ruEd[0];
                }
                else if(text.text == engEd[1]){
                    text.text = ruEd[1];
                }
                else if(text.text == engEd[2]){
                    text.text = ruEd[2];
                }
            }
        }
        else{
            if(lang == "en_US"){
                text.text = eng;
            } else if(lang == "ru_RU"){
                text.text = ru;
            }
        }
    }

    void Update()
    {
        lang = PlayerPrefs.GetString("Language");
        text = GetComponent<Text>();
        if(editable){
            if(lang == "en_US"){
                if(text.text == ruEd[0]){
                    text.text = engEd[0];
                }
                else if(text.text == ruEd[1]){
                    text.text = engEd[1];
                }
                else if(text.text == ruEd[2]){
                    text.text = engEd[2];
                }
            }
            else if(lang == "ru_RU"){
                if(text.text == engEd[0]){
                    text.text = ruEd[0];
                }
                else if(text.text == engEd[1]){
                    text.text = ruEd[1];
                }
                else if(text.text == engEd[2]){
                    text.text = ruEd[2];
                }
            }
        }
        else{
            if(lang == "en_US"){
                text.text = eng;
            } else if(lang == "ru_RU"){
                text.text = ru;
            }
        }
    }
}
