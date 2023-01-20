using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UWSystemChange : MonoBehaviour
{
    [SerializeField] private int id;
    [SerializeField] private GameObject button, buttonClose;
    [SerializeField] private PlayerStats stats;

    public void ChangeUWS(){
        if(id != 0){
            if(PlayerPrefs.GetInt("UnderWaterSystem" + id.ToString()) == 1){
                PlayerPrefs.SetInt("ChangedUnderWaterSystem", id);
                stats.LoadStats();
            }
        }else{
            PlayerPrefs.SetInt("ChangedUnderWaterSystem", id);
        }
    }
    private void Update() {
        if(id != 0){
            if(PlayerPrefs.GetInt("UnderWaterSystem" + id.ToString()) != 1){
                buttonClose.SetActive(true);
            } else{
                buttonClose.SetActive(false);
            }
        }
        if(PlayerPrefs.GetInt("ChangedUnderWaterSystem") == id){
            button.SetActive(false);
        }
        else{
            button.SetActive(true);
        }
    }
}
