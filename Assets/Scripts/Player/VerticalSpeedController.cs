using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VerticalSpeedController : MonoBehaviour
{
    [SerializeField] private float maxSpeed, currentSpeed;

    [SerializeField] private Rigidbody2D rb;
    private void Update(){
        if(rb.velocity.y <= -maxSpeed){
            rb.velocity = new Vector2(rb.velocity.x, -maxSpeed);
        }
        if(rb.velocity.y >= maxSpeed){
            rb.velocity = new Vector2(rb.velocity.x, maxSpeed);
        }
        currentSpeed = rb.velocity.y;
    }
}
