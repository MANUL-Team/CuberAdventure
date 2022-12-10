using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurbineChange : MonoBehaviour
{
    [SerializeField] private int id;
    [SerializeField] private GameObject button, buttonClose;
    [SerializeField] private PlayerStats stats;

    public void ChangeTurbine(){
        if(id != 0){
            if(PlayerPrefs.GetInt("Turbine" + id.ToString()) == 1){
                PlayerPrefs.SetInt("ChangedTurbine", id);
                stats.LoadStats();
            }
        }else{
            PlayerPrefs.SetInt("ChangedTurbine", id);
        }
    }
    private void Update() {
        if(id != 0){
            if(PlayerPrefs.GetInt("Turbine" + id.ToString()) != 1){
                buttonClose.SetActive(true);
            } else{
                buttonClose.SetActive(false);
            }
        }
        if(PlayerPrefs.GetInt("ChangedTurbine") == id){
            button.SetActive(false);
        }
        else{
            button.SetActive(true);
        }
    }
}
