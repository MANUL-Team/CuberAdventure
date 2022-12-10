using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{


    [SerializeField] private Transform[] LevelTeleports;
    [SerializeField] private Transform Player;

    [SerializeField] private Transform Camera;

    [SerializeField] private GameObject Sun;

    [SerializeField] private int level;



    private void Start() {
        Player = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();
        Camera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Transform>();
    }

    private void Update(){
        if(PlayerPrefs.GetInt("SwitchLevel") == 1){
            PlayerPrefs.SetInt("Level", PlayerPrefs.GetInt("Level") + 1);
            Player.position = new Vector2(LevelTeleports[PlayerPrefs.GetInt("Level")].position.x, LevelTeleports[PlayerPrefs.GetInt("Level")].position.y);
            Camera.position = new Vector3(LevelTeleports[PlayerPrefs.GetInt("Level")].position.x, LevelTeleports[PlayerPrefs.GetInt("Level")].position.y, LevelTeleports[PlayerPrefs.GetInt("Level")].position.z);
        }

        if(PlayerPrefs.GetInt("Level") == 1){
            Sun.SetActive(false);
        }


        level = PlayerPrefs.GetInt("Level");
    }


}
