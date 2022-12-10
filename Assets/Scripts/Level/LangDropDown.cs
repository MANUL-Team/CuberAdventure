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

    void Update()
    {
        if(PlayerPrefs.GetInt("Language") == 0){
            for(int i = 0; i < en.Length; i++){
                dropdown.options[i].text = en[i];
            }
        }
        if(PlayerPrefs.GetInt("Language") == 1){
            for(int i = 0; i < ru.Length; i++){
                dropdown.options[i].text = ru[i];
            }
        }
    }
}
