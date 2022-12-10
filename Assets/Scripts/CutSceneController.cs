using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CutSceneController : MonoBehaviour
{
    [SerializeField] public bool cutScene;
    [SerializeField] private CutScene[] scenes;

    private void Awake() {
        if(scenes.Length > 0){
            for(int i = 0; i < scenes.Length; i++){
                if(PlayerPrefs.GetInt("CutScene" + scenes[i].id.ToString()) == 1){
                    scenes[i].trigger.SetActive(false);
                }
            }
        }
    }
    public void StartScene(int id){
        for(int i = 0; i < scenes.Length; i++){
            if(scenes[i].id == id){
                scenes[i].scene.SetActive(true);
                scenes[i].trigger.SetActive(false);
                PlayerPrefs.SetInt("CutScene" + id.ToString(), 1);
            }
        }
    }
    private void Update() {
        if(cutScene){
            PlayerPrefs.SetInt("MaskCS", 1);
        }else{
            PlayerPrefs.SetInt("MaskCS", 0);
        }
    }
}
