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
        int language = PlayerPrefs.GetInt("Language");
        int missionIndex = PlayerPrefs.GetInt("Mission" + missionName) - 1;
        for(int i = 0; i <= steps; i++){
            if(missionIndex == i){
                switch (language)
                {
                    case 0:
                        mc.description.text = descriptionsRu[i];
                        break;
                    case 1:
                        mc.description.text = descriptionsEng[i];
                        break;
                    default:
                        mc.description.text = descriptionsEng[i];
                        break;
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
