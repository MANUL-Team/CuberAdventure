using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class DeathScript : MonoBehaviour
{

    private GameObject obj;
    private Transform playerTransform;
    [SerializeField] private Transform playerPos, playerPosP;

    [SerializeField] private Transform Movement;

    [SerializeField] private GameObject[] players;

    [SerializeField] private GameObject Respawn;
    [SerializeField] private GameObject RespawnMenu;
    [SerializeField] private GameObject Player;
    [SerializeField] private GameObject Loading;

    [SerializeField] private GameObject DeathParticle, DeathParticleObj;

    [SerializeField] private bool alive;


    void OnTriggerEnter2D(Collider2D collision){
        if(collision.CompareTag("Player")){
            Death();
        }
    }

    public void Death(){
        DeathParticleObj = Instantiate(DeathParticle, Player.GetComponent<Transform>().position, Quaternion.identity);
        Player.SetActive(false);
        PlayerPrefs.SetInt("Dead", 1);
        RespawnMenu.SetActive(true);
        Destroy(DeathParticleObj, 2f);
    }


    public void Threesec(){
        Respawn.GetComponent<Text>().text = "Respawn in 3...";
    }
    public void Twosec(){
        Respawn.GetComponent<Text>().text = "Respawn in 2...";
    }
    public void Onesec(){
        Respawn.GetComponent<Text>().text = "Respawn in 1...";
    }
    public void Onesec1(){
        Invoke("Onesec", 1f);
    }
    public void RespawnVoid(){
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            PlayerPrefs.SetInt("Dead", 0);
    }

    private void Awake() {
        Respawn = GameObject.FindGameObjectWithTag("RespawnText");
        RespawnMenu = GameObject.FindGameObjectWithTag("DeathMenu");
    }
    private void Start() {
        playerPos = GameObject.FindGameObjectWithTag("PlayerPos").GetComponent<Transform>();
        playerPosP = GameObject.FindGameObjectWithTag("PlayerPosP").GetComponent<Transform>();
        RespawnMenu.SetActive(false);
        Player = GameObject.FindGameObjectWithTag("Player");
    }
    private void Update() {
        
    }
}
