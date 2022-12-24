using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnderWater : MonoBehaviour
{
    private PlayerController pc;
    void OnTriggerStay2D (Collider2D collision){
        if (collision.CompareTag("Water")){
            pc.JumpForce = 10;
        }
    }
    void OnTriggerExit2D (Collider2D collision){
        if (collision.CompareTag("Water")){
            pc.JumpForce = 17;
        }
    }
    void Start(){
        pc = GetComponent<PlayerController>();
    }
}
