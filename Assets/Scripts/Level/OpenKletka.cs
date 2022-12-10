using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpenKletka : MonoBehaviour
{
    [SerializeField] private Animator kletka;

    private void OnTriggerEnter2D(Collider2D other) {
        if(other.gameObject.CompareTag("Player")){
            kletka.SetBool("Open", true);
        }
    }
    private void OnTriggerExit2D(Collider2D other) {
        if(other.gameObject.CompareTag("Player")){
            kletka.SetBool("Open", false);
        }
    }
}
