using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserController : MonoBehaviour
{
    [SerializeField] private Transform laserFolder;
    [SerializeField] private List<LaserChange> laser = new List<LaserChange>();

    private void OnEnable() {
        for(int i = 0; i < laserFolder.childCount; i++){
            laser.Add(laserFolder.GetChild(i).GetComponent<LaserChange>());
        }
        for(int i = 0; i < laser.Count; i++){
            if(laser[i].id != 0){
                if(PlayerPrefs.GetInt("Laser" + laser[i].id.ToString()) != 1){
                    laser[i].buttonClose.SetActive(true);
                } else{
                    laser[i].buttonClose.SetActive(false);
                }
            }
            if(PlayerPrefs.GetInt("ChangedLaser") == laser[i].id){
                laser[i].button.SetActive(false);
            }
            else{
                laser[i].button.SetActive(true);
            }
        }
    }
}
