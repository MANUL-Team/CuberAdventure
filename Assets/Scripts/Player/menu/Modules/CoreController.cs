using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoreController : MonoBehaviour
{
    [SerializeField] private Transform coresFolder;
    [SerializeField] private List<WeaponChange> cores = new List<WeaponChange>();

    private void OnEnable() {
        for(int i = 0; i < coresFolder.childCount; i++){
            cores.Add(coresFolder.GetChild(i).GetComponent<WeaponChange>());
        }
        for(int i = 0; i < cores.Count; i++){
            if(cores[i].id != 0){
                if(PlayerPrefs.GetInt("Weapon" + cores[i].id.ToString()) != 1){
                    cores[i].buttonClose.SetActive(true);
                } else{
                    cores[i].buttonClose.SetActive(false);
                }
            }
            if(PlayerPrefs.GetInt("Core") == cores[i].id){
                cores[i].button.SetActive(false);
            }
            else{
                cores[i].button.SetActive(true);
            }
        }
    }
}
