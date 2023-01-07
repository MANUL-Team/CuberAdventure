using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SpawnPlayers : MonoBehaviour
{

    private GameObject obj;
    private Transform playerTransform;
    [SerializeField] private Transform LevelTeleportL, LevelTeleportP, NewSpawn;
    [SerializeField] private GameObject player;

    private void Awake() {
        QualitySettings.SetQualityLevel(PlayerPrefs.GetInt("Graphics"), true);
        if(PlayerPrefs.GetInt("NewSpawnTP") == 1 && NewSpawn != null){
            obj = Instantiate(player, new Vector3(NewSpawn.position.x, NewSpawn.position.y, 0), Quaternion.identity);
        }
        else if(PlayerPrefs.GetInt("LastLevel") <= PlayerPrefs.GetInt("Level") && PlayerPrefs.GetInt("NewSpawnTP") == 0){
            obj = Instantiate(player, new Vector3(LevelTeleportL.position.x, LevelTeleportL.position.y, 0), Quaternion.identity);
        }
        else if(PlayerPrefs.GetInt("LastLevel") > PlayerPrefs.GetInt("Level") && PlayerPrefs.GetInt("NewSpawnTP") == 0){
            obj = Instantiate(player, new Vector3(LevelTeleportP.position.x, LevelTeleportP.position.y, 0), Quaternion.identity);
        }
    }
    private void Start() {
    }

}
