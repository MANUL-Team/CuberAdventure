using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UWSystemChange : MonoBehaviour
{
    public int id;
    public GameObject button, buttonClose;
    [SerializeField] private PlayerStats stats;
    [SerializeField] private UWSSettings uws;

    public void ChangeUWS(){
        if(id != 0){
            if(PlayerPrefs.GetInt("UnderWaterSystem" + id.ToString()) == 1){
                PlayerPrefs.SetInt("ChangedUnderWaterSystem", id);
                stats.LoadStats();
            }
        }else{
            PlayerPrefs.SetInt("ChangedUnderWaterSystem", id);
        }
        uws.CheckModule();
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
