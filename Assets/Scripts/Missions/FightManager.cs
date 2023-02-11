using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FightManager : MonoBehaviour
{
    [SerializeField] private DialogManager dialog, dialog2;
    [SerializeField] private GameObject boss, bossObj, trigger, dialogObj, dialogObj2;
    public bool spawned, dialog1started, dialog2started;
    private void FixedUpdate() {
        if(!spawned && !dialog1started){
            dialog.OpenDialog();
            dialog1started = true;
        }
        if(dialogObj.activeSelf == false && !spawned){
            trigger.SetActive(false);
            boss.SetActive(true);
            spawned = true;
        }
        if(spawned && bossObj.activeSelf == false && !dialog2started){
            dialog2.OpenDialog();
            dialog2started = true;
        }
    }
}
