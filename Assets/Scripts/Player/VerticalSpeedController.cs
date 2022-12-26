using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VerticalSpeedController : MonoBehaviour
{
    [SerializeField] private float maxSpeed, currentSpeed;

    private Rigidbody2D rb;

    private void Start(){
        rb = GetComponent<Rigidbody2D>();
    }
    private void Update(){
        if(rb.velocity.y <= -maxSpeed){
            rb.velocity = new Vector2(rb.velocity.x, -maxSpeed);
        }
        currentSpeed = rb.velocity.y;
    }
}
