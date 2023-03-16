using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UWSController : MonoBehaviour
{
    [SerializeField] private Transform uwsFolder;
    [SerializeField] private List<UWSystemChange> uws = new List<UWSystemChange>();

    private void OnEnable() {
        for(int i = 0; i < uwsFolder.childCount; i++){
            uws.Add(uwsFolder.GetChild(i).GetComponent<UWSystemChange>());
        }
        for(int i = 0; i < uws.Count; i++){
            if(uws[i].id != 0){
                if(PlayerPrefs.GetInt("UnderWaterSystem" + uws[i].id.ToString()) != 1){
                    uws[i].buttonClose.SetActive(true);
                } else{
                    uws[i].buttonClose.SetActive(false);
                }
            }
            if(PlayerPrefs.GetInt("ChangedUnderWaterSystem") == uws[i].id){
                uws[i].button.SetActive(false);
            }
            else{
                uws[i].button.SetActive(true);
            }
        }
    }
}
