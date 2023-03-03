using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mission : MonoBehaviour
{
    public int id, steps;
    [TextArea]
    [SerializeField] private string[] descriptionsRu, descriptionsEng;
    [SerializeField] private MissionsController mc;
    [SerializeField] private GameObject checkMission;

    public void SelectMission(){
        PlayerPrefs.SetInt("CheckMission " + id, 0);
        for(int i = 0; i <= steps; i++){
            if(PlayerPrefs.GetInt("Mission " + id + " Step " + i) == 1){
                if(PlayerPrefs.GetInt("Language")  == 0){
                    mc.description.text = descriptionsEng[i];
                }
                else if(PlayerPrefs.GetInt("Language")  == 1){
                    mc.description.text = descriptionsRu[i];
                }
            }
        }
    }
    private void FixedUpdate() {
        if(PlayerPrefs.GetInt("CheckMission " + id) == 1){
            checkMission.SetActive(true);
        }
        else{
            checkMission.SetActive(false);
        }
    }
}
