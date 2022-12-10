using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightController : MonoBehaviour
{
    [SerializeField] private GameObject[] lights;
    [SerializeField] private int lightIndex;

    private void OnTriggerEnter2D(Collider2D other) {
        if(other.CompareTag("Player")){
            lights[lightIndex].SetActive(true);
        }
    }
    private void Start() {
        if(PlayerPrefs.GetInt("LastLevel") <= PlayerPrefs.GetInt("Level")){
            for(int i = 1; i < lights.Length; i++){
                lights[i].SetActive(false);
            }
        }
        else{
            for(int i = 1; i < lights.Length; i++){
                lights[i].SetActive(true);
            }
        }
    }
}
