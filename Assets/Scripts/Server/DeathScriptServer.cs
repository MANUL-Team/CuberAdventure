using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class DeathScriptServer : MonoBehaviour
{

    [SerializeField] private Transform playerPos;
    [SerializeField] private GameObject player;

    void OnTriggerEnter2D(Collider2D collision){
        if(collision.CompareTag("Killer")){
            player.GetComponent<Transform>().position = (new Vector2(playerPos.position.x, playerPos.position.y));
        }
    }
    private void Start() {
        playerPos = GameObject.FindGameObjectWithTag("PlayerPos").transform;
        }
    }

