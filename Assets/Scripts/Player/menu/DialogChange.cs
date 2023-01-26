using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogChange : MonoBehaviour
{
    [SerializeField] private GameObject mainDialog, missionDialog;
    [SerializeField] private DialogManager mission;
    [SerializeField] private int id, step;
    void FixedUpdate(){
        if(step != 0){
            if(PlayerPrefs.GetInt("Mission " + id + " Step " + step) == 0 && (PlayerPrefs.GetInt("Mission " + id + " Step " + (step-1)) == 1)){
                missionDialog.SetActive(true);
                mainDialog.SetActive(false);
            }
            else{
                missionDialog.SetActive(false);
                mainDialog.SetActive(true);
            }
        }
        else{
            if(PlayerPrefs.GetInt("Mission " + id + " Step " + step) == 0){
                missionDialog.SetActive(true);
                mainDialog.SetActive(false);
            }
            else{
                missionDialog.SetActive(false);
                mainDialog.SetActive(true);
            }
        }
        
        if(mission.dialogEnded == true){
            StartNewMissionStep();
        }
    }
    public void StartNewMissionStep(){
        if(step != 0){
            PlayerPrefs.SetInt("Mission " + id + " Step " + (step-1), 2);
            PlayerPrefs.SetInt("Mission " + id + " Step " + step, 1);
        }
        else{
            PlayerPrefs.SetInt("Mission " + id, 1);
            PlayerPrefs.SetInt("Mission " + id + " Step " + step, 1);
        }
    }
}
