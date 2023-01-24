using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogTrigger : MonoBehaviour
{
    [SerializeField] private GameObject button;
    void OnTriggerEnter2D(Collider2D collision){
        if(collision.CompareTag("Player")){
            button.SetActive(true);
        }
    }
    void OnTriggerExit2D(Collider2D collision){
        if(collision.CompareTag("Player")){
            button.SetActive(false);
        }
    }
}
