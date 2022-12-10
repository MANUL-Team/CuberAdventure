using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LangText : MonoBehaviour
{
    private Text text;
    private int lang;

    [SerializeField] private string eng, ru;
    [SerializeField] private string[] engEd, ruEd;
    [SerializeField] private bool editable, dialog;

    private void Awake() {
        lang = PlayerPrefs.GetInt("Language");
        text = GetComponent<Text>();
        if(editable){
            if(lang == 0){
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
            else if(lang == 1){
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
        }else{
            if(lang == 0){
                text.text = eng;
            } else if(lang == 1){
                text.text = ru;
            }
        }
    }

    void Update()
    {
        lang = PlayerPrefs.GetInt("Language");
        if(dialog == false){
        if(editable){
            if(lang == 0){
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
            else if(lang == 1){
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
        }else{
            if(lang == 0){
                text.text = eng;
            } else if(lang == 1){
                text.text = ru;
            }
        }
        }
        
        
    }
}
