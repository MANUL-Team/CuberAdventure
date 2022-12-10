using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoonMove : MonoBehaviour
{

    private void Start() {
        if(PlayerPrefs.GetInt("Day") == 0){
            transform.position = new Vector2(transform.position.x, 5);
        } else{
            transform.position = new Vector2(transform.position.x, -5);
        }
    }
    private void FixedUpdate() {
        if(PlayerPrefs.GetInt("Day") == 0){
            if(transform.position.y < 5){
                transform.position = new Vector2(transform.position.x, transform.position.y + 0.01f);
            }
        } else if(PlayerPrefs.GetInt("Day") == 1){
            if(transform.position.y > -5){
                transform.position = new Vector2(transform.position.x, transform.position.y - 0.01f);
            }
        }
    }

}
