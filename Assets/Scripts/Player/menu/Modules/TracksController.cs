using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TracksController : MonoBehaviour
{
    [SerializeField] private Transform tracksFolder;
    [SerializeField] private List<GuseniciChange> tracks = new List<GuseniciChange>();

    private void OnEnable() {
        for(int i = 0; i < tracksFolder.childCount; i++){
            tracks.Add(tracksFolder.GetChild(i).GetComponent<GuseniciChange>());
        }
        for(int i = 0; i < tracks.Count; i++){
            if(tracks[i].id != 0){
                if(PlayerPrefs.GetInt("Gusenici" + tracks[i].id.ToString()) != 1){
                    tracks[i].buttonClose.SetActive(true);
                } else{
                    tracks[i].buttonClose.SetActive(false);
                }
            }
            if(PlayerPrefs.GetInt("ChangedGusenici") == tracks[i].id){
                tracks[i].button.SetActive(false);
            }
            else{
                tracks[i].button.SetActive(true);
            }
        }
    }
}
