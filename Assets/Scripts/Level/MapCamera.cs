using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapCamera : MonoBehaviour
{
    [SerializeField]private GameObject cameraUI, mCam;
    [SerializeField]private SkySpawn sky;
    private Camera cam;
    private void Start() {
        cam = GetComponent<Camera>();
    }
    public void OpenCloseMap(){
        cameraUI.SetActive(!cameraUI.activeSelf);
        cam.enabled = cameraUI.activeSelf;
        mCam.SetActive(!cameraUI.activeSelf);
        if(sky != null){
            for(int i = 0; i < sky.SpawnedChunks.Count; i++){
                sky.SpawnedChunks[i].SetActive(!sky.SpawnedChunks[i].activeSelf);
            }
        }
        if(Time.timeScale == 1){
            Time.timeScale = 0;
        }
        else if(Time.timeScale == 0){
            Time.timeScale = 1;
        }
    }
    private void Update() {
    }

}
