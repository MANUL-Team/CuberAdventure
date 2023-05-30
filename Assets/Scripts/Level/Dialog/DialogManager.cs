using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialogManager : MonoBehaviour
{
    [SerializeField] private GameObject dialog, button;
    [SerializeField] public Text name, text;
    [SerializeField] private PrintableText printText;
    private int id;
    [SerializeField] private string[] names;
    [SerializeField] public string[] dialogTexts;
    private Animator cam;
    public bool dialogEnded;
    private LocalizationManager localizationManager;
    void Start(){
        localizationManager = GameObject.FindGameObjectWithTag("LocalizationManager").GetComponent<LocalizationManager>();
        cam = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Animator>();
    }
    public void OpenDialog(){
        dialog.SetActive(true);
        button.SetActive(false);
        PlayerPrefs.SetInt("MaskCS", 1);
        PlayerPrefs.SetInt("MaskD", 1);
        id = 0;
        dialogEnded = false;
        cam.SetBool("Dialog", true);
        printText.PrintText(localizationManager.GetLocalizedValue(dialogTexts[id]));
        name.text = localizationManager.GetLocalizedValue(names[id]);
        Debug.Log("Print");
    }
    public void NextPage(){
        id += 1;
        if(id >= names.Length && dialog.activeSelf == true){
            dialog.SetActive(false);
            dialogEnded = true;
            cam.SetBool("Dialog", false);
            PlayerPrefs.SetInt("MaskCS", 0);
            PlayerPrefs.SetInt("MaskD", 0);
        }
        else{
            for(int i = 0; i < names.Length; i++){
                if(i == id){
                    name.text = localizationManager.GetLocalizedValue(names[id]);
                }
            }
            printText.PrintText(localizationManager.GetLocalizedValue(dialogTexts[id]));
        }
    }
    public void SkipText(){
        printText.speedOfPrint = 0.0005f;
    }
    public int ID{
        get{
            return id;
        }
    }
}
