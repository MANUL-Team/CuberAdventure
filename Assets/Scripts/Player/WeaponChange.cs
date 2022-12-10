using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponChange : MonoBehaviour
{
    [SerializeField] private int id;
    [SerializeField] private GameObject button, buttonClose;
    [SerializeField] private PlayerStats stats;

    public void ChangeWeapon(){
        if(id != 0){
            if(PlayerPrefs.GetInt("Weapon" + id.ToString()) == 1){
                PlayerPrefs.SetInt("ChangedWeapon", id);
                stats.LoadStats();
            }
        }else{
            PlayerPrefs.SetInt("ChangedWeapon", id);
        }
    }
    private void Update() {
        if(id != 0){
            if(PlayerPrefs.GetInt("Weapon" + id.ToString()) != 1){
                buttonClose.SetActive(true);
            } else{
                buttonClose.SetActive(false);
            }
        }
        if(PlayerPrefs.GetInt("ChangedWeapon") == id){
            button.SetActive(false);
        }
        else{
            button.SetActive(true);
        }
    }
    
}
