using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonInvManager : MonoBehaviour
{
    [SerializeField] private GameObject[] clickableButtons;
    public void ButtonSwitch(int id){
        for(int i = 0; i < clickableButtons.Length; i++){
            if(id != i){
                clickableButtons[i].SetActive(true);
            }
            else{
                clickableButtons[i].SetActive(false);
            }
        }
    }
}
