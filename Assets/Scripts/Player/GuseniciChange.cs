using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GuseniciChange : MonoBehaviour
{
    [SerializeField] private int id;
    [SerializeField] private GameObject button, buttonClose;
    [SerializeField] private PlayerStats stats;

    public void ChangeGusenici(){
        if(id != 0){
            if(PlayerPrefs.GetInt("Gusenici" + id.ToString()) == 1){
                PlayerPrefs.SetInt("ChangedGusenici", id);
                stats.LoadStats();
            }
        }else{
            PlayerPrefs.SetInt("ChangedGusenici", id);
        }
    }
    private void Update() {
        if(id != 0){
            if(PlayerPrefs.GetInt("Gusenici" + id.ToString()) != 1){
                buttonClose.SetActive(true);
            } else{
                buttonClose.SetActive(false);
            }
        }
        if(PlayerPrefs.GetInt("ChangedGusenici") == id){
            button.SetActive(false);
        }
        else{
            button.SetActive(true);
        }
    }
}
