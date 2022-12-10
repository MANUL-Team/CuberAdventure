using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CutSceneMask : MonoBehaviour
{
    [SerializeField] private GameObject mask;

    private void Awake() {
        PlayerPrefs.SetInt("MaskCS", 0);
        PlayerPrefs.SetInt("MaskD", 0);
    }
    private void Update() {
        if(PlayerPrefs.GetInt("MaskCS") == 1 || PlayerPrefs.GetInt("MaskD") == 1){
            mask.SetActive(false);
        } else{
            mask.SetActive(true);
        }
    }
}
