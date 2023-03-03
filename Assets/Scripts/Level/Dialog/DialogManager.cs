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
    [SerializeField] private string[] namesEng;
    [TextArea]
    [SerializeField] public string[] dialogTextsEng;
    [SerializeField] private string[] namesRu;
    [TextArea]
    [SerializeField] public string[] dialogTextsRu;
    private Animator cam;
    public bool dialogEnded;
    void Start(){
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
        if(PlayerPrefs.GetInt("Language") == 0){
            printText.PrintText(dialogTextsEng[id]);
        }
        else if(PlayerPrefs.GetInt("Language") == 1){
            printText.PrintText(dialogTextsRu[id]);
        }
        Debug.Log("Print");
    }
    public void NextPage(){
        id += 1;
        if(PlayerPrefs.GetInt("Language") == 0){
            printText.PrintText(dialogTextsEng[id]);
        }
        else if(PlayerPrefs.GetInt("Language") == 1){
            printText.PrintText(dialogTextsRu[id]);
        }
    }
    public void SkipText(){
        printText.speedOfPrint = 0.005f;
    }
    void FixedUpdate(){
        for(int i = 0; i < namesRu.Length; i++){
            if(i == id){
                if(PlayerPrefs.GetInt("Language") == 0){
                    name.text = namesEng[i];
                }
                else if(PlayerPrefs.GetInt("Language") == 1){
                    name.text = namesRu[i];
                }
            }
        }
        if(id >= namesRu.Length && dialog.activeSelf == true){
            dialog.SetActive(false);
            dialogEnded = true;
            cam.SetBool("Dialog", false);
            PlayerPrefs.SetInt("MaskCS", 0);
            PlayerPrefs.SetInt("MaskD", 0);
        }
    }
}
