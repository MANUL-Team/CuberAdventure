using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpenFloor : MonoBehaviour
{
    [SerializeField] private Animator floor, button;

    private void OnTriggerEnter2D(Collider2D other){
        if(other.gameObject.CompareTag("Player")){
            floor.SetBool("Open", true);
            button.SetBool("Pressed", true);
        }
    }
}
