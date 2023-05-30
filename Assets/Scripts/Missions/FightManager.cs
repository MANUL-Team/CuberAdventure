using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FightManager : MonoBehaviour
{
    [SerializeField] private DialogManager dialog, dialog2;
    [SerializeField] private GameObject boss, bossObj, trigger, dialogObj, dialogObj2;
    bool spawned, dialog1started, dialog2started;
    [SerializeField] private DialogChange dialogChange;
    [SerializeField] private int id, step;
    // private void FixedUpdate() {
    //     if(PlayerPrefs.GetInt("Mission " + id + " Step " + (step-1).ToString()) == 1){
    //         if(!spawned && !dialog1started){
    //             dialog.OpenDialog();
    //             dialog1started = true;
    //         }
    //         if(dialogObj.activeSelf == false && !spawned){
    //             trigger.SetActive(false);
    //             boss.SetActive(true);
    //             spawned = true;
    //         }
    //         if(spawned && bossObj.activeSelf == false && !dialog2started){
    //             dialog2.OpenDialog();
    //             dialog2started = true;
    //         }
    //         if(dialog2started && dialogObj2.activeSelf == false){
    //             dialogChange.StartNewMissionStep(id, step, false);
    //         }
    //     }
    // }
}
