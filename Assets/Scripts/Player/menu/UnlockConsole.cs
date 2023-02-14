using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnlockConsole : MonoBehaviour
{
    [SerializeField] private GameObject console;
    private void Start() {
        if(PlayerPrefs.GetInt("ConsoleUnlocked") == 1){
            console.SetActive(true);
        }
        else{
            console.SetActive(false);
        }
    }
}
