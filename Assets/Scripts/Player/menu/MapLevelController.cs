using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapLevelController : MonoBehaviour
{
    private List<MapLevelChange> descriptions = new List<MapLevelChange>();
    [SerializeField] TeleportScript tp;
    public void OpenDescription(int id){
        for(int i = 0; i <= descriptions.Count; i++){
            if(PlayerPrefs.GetInt("LevelEnded" + id) == 1){
                tp.gameObject.SetActive(true);
            } else{
                tp.gameObject.SetActive(false);
            }
            if(descriptions[i].id != id){
                descriptions[i].gameObject.SetActive(false);
            }
            else{
                descriptions[i].gameObject.SetActive(true);
                tp.currentId = descriptions[i].id;
            }
        }
    }
    private void Start() {
        for(int i = 0; i < transform.childCount; i++){
            descriptions.Add(transform.GetChild(i).GetComponent<MapLevelChange>());
        }
    }
}
