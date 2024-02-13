using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LangDropDown : MonoBehaviour
{

    [SerializeField] private string[] en, ru;
    private Dropdown dropdown;
    void Awake()
    {
        dropdown = GetComponent<Dropdown>();
        Localize();
    }

    void Localize()
    {
        int language = PlayerPrefs.GetInt("Language");
        if(language == 0){
            for(int i = 0; i < ru.Length; i++){
                dropdown.options[i].text = ru[i];
            }
        }
        else if(language == 1){
            for(int i = 0; i < en.Length; i++){
                dropdown.options[i].text = en[i];
            }
        }
    }
}
