using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WebStopObject : MonoBehaviour
{
    private Transform player;
    private PlayerController pc;
    [SerializeField] private GameObject sprite;
    [SerializeField] private GameObject web;
    private void Start() {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();
        pc = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
    }
    private void OnTriggerEnter2D(Collider2D other) {
        if(other.CompareTag("Player")){
            pc.Confuse();
            GameObject newWeb = Instantiate(web, player.position, Quaternion.identity);
            Destroy(newWeb, 3f);
            sprite.SetActive(false);
            GetComponent<BoxCollider2D>().enabled = false;
        }
    }
}
