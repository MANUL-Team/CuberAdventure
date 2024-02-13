using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LangText : MonoBehaviour
{
    private Text text;
    private int lang;
    [TextArea]
    [SerializeField] private string eng, ru;
    [SerializeField] private string[] engEd, ruEd;
    [SerializeField] private bool editable;
    [SerializeField] private Dropdown dropdown;

    private void Awake()
    {
        text = GetComponent<Text>();
        Localize();
    }
    private void Localize()
    {
        int language = PlayerPrefs.GetInt("Language");
        if(!editable) 
            switch (language)
            {
                case 0:
                    text.text = ru;
                    break;
                case 1:
                    text.text = eng;
                    break;
                default:
                    text.text = eng;
                    break;
            }
        else
            switch (language)
            {
                case 0:
                    text.text = ruEd[dropdown.value];
                    break;
                case 1:
                    text.text = engEd[dropdown.value];
                    break;
            }
    }
}
