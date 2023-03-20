using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PointOffline : MonoBehaviour
{
    [SerializeField] Animator animator;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float checkRadius;
    [SerializeField] private LayerMask whatIsGround;
    [SerializeField] private bool isGrounded;
    private Rigidbody2D rb;
    private void Start() {
        rb = GetComponent<Rigidbody2D>();
    }
    void Update()
    {
        if(rb.velocity.y == 0){
            animator.SetBool("JumpDown", false);
            animator.SetBool("JumpUp", false);
        }
        else if(rb.velocity.y > 0){
            animator.SetBool("JumpUp", true);
            animator.SetBool("JumpDown", false);
        }
        else if(rb.velocity.y < 0){
            animator.SetBool("JumpUp", false);
            animator.SetBool("JumpDown", true);
        }
    }
    private void FixedUpdate() {
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, checkRadius, whatIsGround);
    }
}
