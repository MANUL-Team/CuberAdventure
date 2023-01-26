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
        StartCoroutine(CheckMissions());
    }
    private IEnumerator CheckMissions(){
        while(true){
            for(int i = 0; i < missions.Count; i++){
                if(PlayerPrefs.GetInt("Mission " + missions[i].id) != 1){
                    missions[i].gameObject.SetActive(false);
                }
                else{
                    missions[i].gameObject.SetActive(true);
                }
            }
            yield return new WaitForSeconds(0.1f);
        }
    }
}
