using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogChange : MonoBehaviour
{
    [SerializeField] private GameObject mainDialog;
    [SerializeField] private GameObject[] missionDialogs;
    [SerializeField] private DialogManager[] missions;
    [SerializeField] private int id;
    [SerializeField] private int[] steps;
    [SerializeField] private bool main, last;
    [SerializeField] private bool missionsBool;
    private bool checkStarted;
    void FixedUpdate(){
        if(!missionsBool){
            if(!checkStarted){
                StartCoroutine(MissionsCheck());
                checkStarted = true;
            }
            for(int i = 0; i < missions.Length; i++){
                if(missions[i].dialogEnded == true){
                    StartNewMissionStep(id, steps[i], false);
                    missions[i].dialogEnded = false;
                }
            }
            if(last){
                if(PlayerPrefs.GetInt("Mission " + id + " Step " + steps[steps.Length - 1]) == 1){
                    PlayerPrefs.SetInt("Mission " + id, 2);
                }
            }
        }
    }
    private IEnumerator MissionsCheck(){
        while(true){
            main = true;
            for(int i = 0; i < missionDialogs.Length; i++){
                if(steps[i] != 0){
                    if(PlayerPrefs.GetInt("Mission " + id + " Step " + steps[i]) == 0 && (PlayerPrefs.GetInt("Mission " + id + " Step " + (steps[i]-1)) == 1)){
                        missionDialogs[i].SetActive(true);
                        main = false;
                    }
                    else{
                        missionDialogs[i].SetActive(false);
                    }
                }
                else{
                    if(PlayerPrefs.GetInt("Mission " + id + " Step " + steps[i]) == 0){
                        missionDialogs[i].SetActive(true);
                        main = false;
                    }
                    else{
                        missionDialogs[i].SetActive(false);
                    }
                }
            }
            if(main){
                mainDialog.SetActive(true);
            }
            else{
                mainDialog.SetActive(false);
            }
            yield return new WaitForSeconds(0.2f);
        }
    }
    public void StartNewMissionStep(int id, int step, bool last){
        PlayerPrefs.SetInt("CheckTheMission", 1);
        PlayerPrefs.SetInt("CheckMission " + id, 1);
        if(last){
            PlayerPrefs.SetInt("Mission " + id, 2);
            PlayerPrefs.SetInt("Mission " + id + " Step " + step, 2);
        }
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
