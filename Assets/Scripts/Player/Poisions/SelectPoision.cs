using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectPoision : MonoBehaviour
{
    [SerializeField] private int poisionId;
    [SerializeField] private GameObject[] useButtons;
    [SerializeField] private SwitchPoisionButtons switchClass;

    private void FixedUpdate() {
        if(PlayerPrefs.GetInt("SelectedPoision") == poisionId){
            useButtons[0].SetActive(false);
            useButtons[1].SetActive(true);
        }
        else{
            useButtons[0].SetActive(true);
            useButtons[1].SetActive(false);
        }
    }
    public void Select(){
        PlayerPrefs.SetInt("SelectedPoision", poisionId);
        switchClass.PoisionSwitch();
    }
}
