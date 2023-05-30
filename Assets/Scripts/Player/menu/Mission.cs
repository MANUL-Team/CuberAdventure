using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mission : MonoBehaviour
{
    public int steps;
    public string missionName;
    [TextArea]
    [SerializeField] private string[] descriptionsRu, descriptionsEng;
    [SerializeField] private MissionsController mc;
    [SerializeField] private GameObject checkMission;

    public void SelectMission(){
        PlayerPrefs.SetInt("CheckMission " + missionName, 0);
        for(int i = 0; i <= steps; i++){
            if(PlayerPrefs.GetInt("Mission" + missionName) - 1 == i){
                if(PlayerPrefs.GetString("Language")  == "en_US"){
                    mc.description.text = descriptionsEng[i];
                }
                else if(PlayerPrefs.GetString("Language")  == "ru_RU"){
                    mc.description.text = descriptionsRu[i];
                }
            }
        }
    }
    private void FixedUpdate() {
        if(PlayerPrefs.GetInt("CheckMission " + missionName) == 1){
            checkMission.SetActive(true);
        }
        else{
            checkMission.SetActive(false);
        }
    }
}
