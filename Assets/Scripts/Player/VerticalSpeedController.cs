using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VerticalSpeedController : MonoBehaviour
{
    [SerializeField] private float maxSpeed, currentSpeed;

    [SerializeField] private Rigidbody2D rb;
    private void Update(){
        if(rb.linearVelocity.y <= -maxSpeed){
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, -maxSpeed);
        }
        if(rb.linearVelocity.y >= maxSpeed){
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, maxSpeed);
        }
        currentSpeed = rb.linearVelocity.y;
    }
}
