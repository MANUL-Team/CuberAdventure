using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserChange : MonoBehaviour
{
    [SerializeField] private int id;
    [SerializeField] private GameObject button, buttonClose;
    [SerializeField] private PlayerStats stats;

    public void ChangeLaser(){
        if(id != 0){
            if(PlayerPrefs.GetInt("Laser" + id.ToString()) == 1){
                PlayerPrefs.SetInt("ChangedLaser", id);
                stats.LoadStats();
            }
        }else{
            PlayerPrefs.SetInt("ChangedLaser", id);
        }
    }
    private void Update() {
        if(id != 0){
            if(PlayerPrefs.GetInt("Laser" + id.ToString()) != 1){
                buttonClose.SetActive(true);
            } else{
                buttonClose.SetActive(false);
            }
        }
        if(PlayerPrefs.GetInt("ChangedLaser") == id){
            button.SetActive(false);
        }
        else{
            button.SetActive(true);
        }
    }
}
