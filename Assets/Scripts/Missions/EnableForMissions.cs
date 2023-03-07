using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnableForMissions : MonoBehaviour
{
    [SerializeField] private int id, step;
    [SerializeField] private GameObject enable, disable;

    private void Start() {
        if(PlayerPrefs.GetInt("Mission " + id + " Step " + step) == 1){
            enable.SetActive(true);
            disable.SetActive(false);
        }
        else{
            enable.SetActive(false);
            disable.SetActive(true);
        }
    }
}
