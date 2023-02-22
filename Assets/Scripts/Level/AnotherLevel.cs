using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class AnotherLevel : MonoBehaviour
{
    [SerializeField] private int level;
    [SerializeField] private TeleportScript tp;
    [SerializeField] private bool newSpawn;

    private void Start() {
        tp = GameObject.FindGameObjectWithTag("Teleport").GetComponent<TeleportScript>();
    }
    public void OnTriggerEnter2D(Collider2D collision){
        if(collision.CompareTag("Player")){
            tp.currentId = level;
            tp.Teleport();
            if(newSpawn){
                PlayerPrefs.SetInt("NewSpawnTP", 1);
            }
            else{
                PlayerPrefs.SetInt("NewSpawnTP", 0);
            }
        }
    }
}
