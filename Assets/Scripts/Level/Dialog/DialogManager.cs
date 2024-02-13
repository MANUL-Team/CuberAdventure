using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialogManager : MonoBehaviour
{
    [SerializeField] private GameObject dialog, button;
    [SerializeField] public Text name;
    [SerializeField] private PrintableText printText;
    private int id;
    [SerializeField] private string[] namesRu, namesEng;
    [SerializeField] public string[] dialogRu, dialogEng;
    private Animator cam;
    public bool dialogEnded;
    private LocalizationManager localizationManager;
    private int language;
    void Start(){
        cam = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Animator>();
        language = PlayerPrefs.GetInt("Language");
    }
    public void OpenDialog(){
        dialog.SetActive(true);
        button.SetActive(false);
        PlayerPrefs.SetInt("MaskCS", 1);
        PlayerPrefs.SetInt("MaskD", 1);
        id = 0;
        dialogEnded = false;
        cam.enabled = true;
        cam.SetBool("Dialog", true);
        DoText();
    }
    public void NextPage(){
        id += 1;
        if(id >= namesRu.Length && dialog.activeSelf){
            dialog.SetActive(false);
            dialogEnded = true;
            cam.SetBool("Dialog", false);
            PlayerPrefs.SetInt("MaskCS", 0);
            PlayerPrefs.SetInt("MaskD", 0);
        }
        else DoText();
    }

    private void DoText()
    {
        string playerName = PlayerPrefs.GetString("PlayerName");
        string dName = "";
        string dText = "";
        
        switch (language)
        {
            case 0:
                dText = dialogRu[id].Replace("{playerName}", playerName);
                dName = namesRu[id] != "{playerName}" ? namesRu[id] : playerName;
                break;
            case 1:
                dText = dialogEng[id].Replace("{playerName}", playerName);
                dName = namesEng[id] != "{playerName}" ? namesEng[id] : playerName;
                break;
            default:
                dText = dialogEng[id].Replace("{playerName}", playerName);
                dName = namesEng[id] != "{playerName}" ? namesEng[id] : playerName;
                break;
        }
        printText.PrintText(dText);
        name.text = dName;
    }
    public void SkipText(){
        printText.speedOfPrint = 0.0005f;
    }
    public int ID => id;
}
