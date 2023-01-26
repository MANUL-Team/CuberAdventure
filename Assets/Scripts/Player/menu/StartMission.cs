using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartMission : MonoBehaviour
{
    [SerializeField] private int id, step;

    public void ChangeMission(){
        if(PlayerPrefs.GetInt("Mission " + id) != 2){
            if(step != 0){
                if(PlayerPrefs.GetInt("Mission " + id + " Step " + step) != 2 && PlayerPrefs.GetInt("Mission " + id + " Step " + (step-1)) == 2){
                    PlayerPrefs.SetInt("Mission" + id + " Step " + step, 1);
                }
            }
            else{
                PlayerPrefs.SetInt("Mission " + id, 1);
                PlayerPrefs.SetInt("Mission " + id + " Step " + step, 1);
            }
        }
    }
    public void EndMission(int misId, int misStep){
        PlayerPrefs.SetInt("Mission " + misId, 2);
        PlayerPrefs.SetInt("Mission " + misId + " Step " + misStep, 2);
    }
}
