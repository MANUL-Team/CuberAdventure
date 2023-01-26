using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mission : MonoBehaviour
{
    public int id, steps;
    [TextArea]
    [SerializeField] private string[] descriptionsRu, descriptionsEng;
    [SerializeField] private MissionsController mc;

    public void SelectMission(){
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
}
