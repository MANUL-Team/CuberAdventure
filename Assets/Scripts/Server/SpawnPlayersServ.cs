using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class SpawnPlayersServ : MonoBehaviour
{

    [SerializeField] private GameObject player;
    [SerializeField] private float minX, minY, maxX, maxY;
    [SerializeField] private Transform playerPos;

    [SerializeField] private GameObject[] players;

    private void Start() {
        //Vector2 randomPosition = new Vector2 (Random.Range(minX, minY), Random.Range(maxX, maxY));


        player = players[PlayerPrefs.GetInt("Player")];
        PhotonNetwork.Instantiate(player.name, playerPos.position, Quaternion.identity);
    }
}
