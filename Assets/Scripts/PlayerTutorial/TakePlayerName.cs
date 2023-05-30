using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TakePlayerName : MonoBehaviour
{
    [SerializeField] private InputField nameField;
    [SerializeField] private GameObject namePlane;
    [SerializeField] private DialogManager dialog;
    public void ConfirmName(){
        if(nameField.text != ""){
            PlayerPrefs.SetString("PlayerName", nameField.text);
            dialog.NextPage();
            namePlane.SetActive(false);
        }
    }
    private void Update() {
        if(dialog.ID == 13){
            if(namePlane.activeSelf == false){
                namePlane.SetActive(true);
            }
        }
    }
}
