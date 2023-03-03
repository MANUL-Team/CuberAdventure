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
            if(PlayerPrefs.GetInt("Mission " + missions[i].id) == 1){
                missions[i].gameObject.SetActive(true);
            }
            else{
                missions[i].gameObject.SetActive(false);
            }
        }
    }
    void OnEnable(){
        if(PlayerPrefs.GetInt("Language") == 0){
            description.text = "Select a mission.";
        }
        else if(PlayerPrefs.GetInt("Language") == 1){
            description.text = "Выберите миссию.";
        }
    }
}
