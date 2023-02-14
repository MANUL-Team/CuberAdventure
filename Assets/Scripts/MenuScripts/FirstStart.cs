using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FirstStart : MonoBehaviour
{
    private void Start() {
        if(PlayerPrefs.GetInt("FirstStart") == 0){
            PlayerPrefs.SetFloat("Volume", 1);
            PlayerPrefs.SetInt("Language", 1);
            PlayerPrefs.SetInt("Graphics", 1);
            PlayerPrefs.SetInt("FirstStart", 1);
        }
    }
}
