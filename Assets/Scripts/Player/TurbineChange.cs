using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurbineChange : MonoBehaviour
{
    public int id;
    public GameObject button, buttonClose;
    [SerializeField] private PlayerStats stats;
    [SerializeField] private TurbineSettings turbine;

    public void ChangeTurbine(){
        if(id != 0){
            if(PlayerPrefs.GetInt("Turbine" + id.ToString()) == 1){
                PlayerPrefs.SetInt("ChangedTurbine", id);
                stats.LoadStats();
            }
        }else{
            PlayerPrefs.SetInt("ChangedTurbine", id);
        }
        turbine.CheckModule();
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
