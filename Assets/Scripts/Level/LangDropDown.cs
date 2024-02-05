using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LangDropDown : MonoBehaviour
{

    [SerializeField] private string[] en, ru;
    private Dropdown dropdown;
    void Start()
    {
        dropdown = GetComponent<Dropdown>();
    }

    void FixedUpdate()
    {
        if(PlayerPrefs.GetString("Language") == "en_US"){
            for(int i = 0; i < en.Length; i++){
                dropdown.options[i].text = en[i];
            }
        }
        if(PlayerPrefs.GetString("Language") == "ru_RU"){
            for(int i = 0; i < ru.Length; i++){
                dropdown.options[i].text = ru[i];
            }
        }
    }
}
