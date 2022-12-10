using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArmorChange : MonoBehaviour
{
    [SerializeField] private int id;
    [SerializeField] private GameObject button, buttonClose;
    [SerializeField] private PlayerStats stats;

    public void ChangeArmor(){
        if(id != 0){
            if(PlayerPrefs.GetInt("Armor" + id.ToString()) == 1){
                PlayerPrefs.SetInt("ChangedArmor", id);
                stats.LoadStats();
            }
        }else{
            PlayerPrefs.SetInt("ChangedArmor", id);
        }
    }
    private void Update() {
        if(id != 0){
            if(PlayerPrefs.GetInt("Armor" + id.ToString()) != 1){
                buttonClose.SetActive(true);
            } else{
                buttonClose.SetActive(false);
            }
        }
        if(PlayerPrefs.GetInt("ChangedArmor") == id){
            button.SetActive(false);
        }
        else{
            button.SetActive(true);
        }
    }
}
