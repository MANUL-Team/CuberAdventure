using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogChange : MonoBehaviour
{
    [SerializeField] private GameObject mainDialog;
    [SerializeField] private GameObject[] missionDialogs;
    [SerializeField] private DialogManager[] missions;
    [SerializeField] private string missionName;
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
                    StartNewMissionStep(missionName, steps[i], last);
                    missions[i].dialogEnded = false;
                }
            }
        }
    }
    private IEnumerator MissionsCheck(){
        while(true){
            main = true;
            for(int i = 0; i < missionDialogs.Length; i++){
                if(PlayerPrefs.GetInt("Mission" + missionName) == steps[i]-1){
                    missionDialogs[i].SetActive(true);
                    main = false;
                }
                else{
                    missionDialogs[i].SetActive(false);
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
    public void StartNewMissionStep(string missionName, int step, bool last){
        if(!(this.last && PlayerPrefs.GetInt("Mission" + missionName) == steps[steps.Length-1])){
            PlayerPrefs.SetInt("CheckTheMission", 1);
            PlayerPrefs.SetInt("CheckMission " + missionName, 1);
        }
        PlayerPrefs.SetInt("Mission" + missionName, step);
        Debug.Log("Mission " + missionName + " = " + PlayerPrefs.GetInt("Mission" + missionName));
    }
}
