using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurbineController : MonoBehaviour
{
    [SerializeField] private Transform turbineFolder;
    [SerializeField] private List<TurbineChange> turbine = new List<TurbineChange>();

    private void OnEnable() {
        for(int i = 0; i < turbineFolder.childCount; i++){
            turbine.Add(turbineFolder.GetChild(i).GetComponent<TurbineChange>());
        }
        for(int i = 0; i < turbine.Count; i++){
            if(turbine[i].id != 0){
                if(PlayerPrefs.GetInt("Laser" + turbine[i].id.ToString()) != 1){
                    turbine[i].buttonClose.SetActive(true);
                } else{
                    turbine[i].buttonClose.SetActive(false);
                }
            }
            if(PlayerPrefs.GetInt("ChangedLaser") == turbine[i].id){
                turbine[i].button.SetActive(false);
            }
            else{
                turbine[i].button.SetActive(true);
            }
        }
    }
}
