using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MissionsController : MonoBehaviour
{
    [SerializeField] private List<Mission> missions = new List<Mission>();
    public Text description;
    [SerializeField] private Transform missionsFolder;

    void Start(){
        for(int i = 0; i < missionsFolder.childCount; i++){
            missions.Add(missionsFolder.GetChild(i).GetComponent<Mission>());
        }
    }
    void FixedUpdate(){
        for(int i = 0; i < missions.Count; i++){
            if(PlayerPrefs.GetInt("Mission" + missions[i].missionName) > 0){
                missions[i].gameObject.SetActive(true);
            }
            else{
                missions[i].gameObject.SetActive(false);
            }
        }
    }
    void OnEnable(){
        if(PlayerPrefs.GetString("Language") == "en_US"){
            description.text = "Select a mission.";
        }
        else if(PlayerPrefs.GetString("Language") == "ru_RU"){
            description.text = "Выберите миссию.";
        }
    }
}
