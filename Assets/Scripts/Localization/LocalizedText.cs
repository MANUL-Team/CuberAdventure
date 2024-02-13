using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LocalizedText : MonoBehaviour
{
    [SerializeField]
    private string key;
    
    private Text text;

    void Awake()
    {
        if(text == null)
        {
            text = GetComponent<Text>();
        }
        LocalizationManager.OnLanguageChanged += UpdateText;
    }

    void Start()
    {
        UpdateText();
    }

    private void OnDestroy()
    {
        LocalizationManager.OnLanguageChanged -= UpdateText;
    }

    virtual protected void UpdateText()
    {
        if (gameObject == null) return;
        if (text == null)
        {
            text = GetComponent<Text>();
        }
        text.text = LocalizationManager.GetLocalizedValue(key);
    }
}