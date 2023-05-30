using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartThisMission : MonoBehaviour
{
    [SerializeField] private DialogManager mission;
    [SerializeField] private DialogChange main;
    [SerializeField] private int step;
    [SerializeField] private string missionName;

    void FixedUpdate(){
        if(mission.dialogEnded == true){
            main.StartNewMissionStep(missionName, step, false);
        }
    }
}
