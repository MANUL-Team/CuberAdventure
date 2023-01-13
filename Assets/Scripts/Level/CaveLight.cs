using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CaveLight : MonoBehaviour
{
    [SerializeField] private GameObject lightSmall, lightBig;

    void FixedUpdate(){
        if(PlayerPrefs.GetInt("LightPoision") == 0){
            lightSmall.SetActive(true);
            lightBig.SetActive(false);
        }
        else{
            lightSmall.SetActive(false);
            lightBig.SetActive(true);
        }
    }
}
