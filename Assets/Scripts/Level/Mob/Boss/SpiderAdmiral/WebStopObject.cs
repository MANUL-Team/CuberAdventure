using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WebStopObject : MonoBehaviour
{
    private Transform player;
    private PlayerController pc;
    [SerializeField] private SpriteRenderer sprite;
    [SerializeField] private GameObject web;
    [SerializeField] private float speed;
    private Vector3 playerLastPos, mainStartPos;
    private bool attack;
    private void Start() {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();
        pc = player.GetComponent<PlayerController>();
        playerLastPos = player.position;
        mainStartPos = transform.position;
        attack = true;
    }
    private void OnTriggerEnter2D(Collider2D other) {
        if(other.CompareTag("Player")){
            pc.Confuse();
            GameObject newWeb = Instantiate(web, player.position, Quaternion.identity);
            Destroy(newWeb, 3f);
            sprite.color = new Vector4(0, 0, 0, 0);
            GetComponent<BoxCollider2D>().enabled = false;
            attack = false;
        }
    }
    private void Update() {
        if(attack){
            transform.position += (playerLastPos - mainStartPos).normalized * speed * Time.deltaTime;
        }
    }
}
