using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponChange : MonoBehaviour
{
    public int id;
    public GameObject button, buttonClose;
    [SerializeField] private PlayerStats stats;
    [SerializeField] private CoreSettings core;

    public void ChangeWeapon(){
        if(id != 0){
            if(PlayerPrefs.GetInt("Weapon" + id.ToString()) == 1){
                PlayerPrefs.SetInt("Core", id);
                stats.LoadStats();
            }
        }else{
            PlayerPrefs.SetInt("Core", id);
        }
        core.CheckModule();
    }
    private void Update() {
        if(id != 0){
            if(PlayerPrefs.GetInt("Weapon" + id.ToString()) != 1){
                buttonClose.SetActive(true);
            } else{
                buttonClose.SetActive(false);
            }
        }
        if(PlayerPrefs.GetInt("Core") == id){
            button.SetActive(false);
        }
        else{
            button.SetActive(true);
        }
    }
    
}
