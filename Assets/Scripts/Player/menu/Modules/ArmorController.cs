using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArmorController : MonoBehaviour
{
    [SerializeField] private Transform armorsFolder;
    [SerializeField] private List<ArmorChange> armors = new List<ArmorChange>();

    private void OnEnable() {
        for(int i = 0; i < armorsFolder.childCount; i++){
            armors.Add(armorsFolder.GetChild(i).GetComponent<ArmorChange>());
        }
        for(int i = 0; i < armors.Count; i++){
            if(armors[i].id != 0){
                if(PlayerPrefs.GetInt("Armor" + armors[i].id.ToString()) != 1){
                    armors[i].buttonClose.SetActive(true);
                } else{
                    armors[i].buttonClose.SetActive(false);
                }
            }
            if(PlayerPrefs.GetInt("ChangedArmor") == armors[i].id){
                armors[i].button.SetActive(false);
            }
            else{
                armors[i].button.SetActive(true);
            }
        }
    }
}
