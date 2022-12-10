using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerContMenu : MonoBehaviour
{
    [SerializeField] private float speed = 5f;
    private Rigidbody2D rb;
    [SerializeField] private float JumpForce = 300f;
    private Transform playerTransform;

    [SerializeField] private bool isGrounded;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float checkRadius;
    [SerializeField] private LayerMask whatIsGround;
    [SerializeField] private float moveInput;

    [SerializeField] private int DoubleJumps, DoubleJumpsValue;




    public void Right(){
        moveInput = 1;
    }

    public void Left(){
        moveInput = -1;
    }

    public void Nothing(){
        moveInput = 0;
    }

    public void Jump(){
        rb.AddForce(Vector2.up * JumpForce);
        DoubleJumps--;
    }
    private void OnTriggerEnter2D(Collider2D collision) {
        if(collision.CompareTag("Jumper")){
            Jump();
        }
    }


    private void Awake() {
    }

    private void Start() {
        rb = GetComponent<Rigidbody2D>();
        playerTransform = GetComponent<Transform>();
    }

    private void Update() {
        Right();
        if(isGrounded){
            DoubleJumps = DoubleJumpsValue;
        }  
    }
    private void FixedUpdate() {
        rb.velocity = new Vector2(moveInput * speed, rb.velocity.y);

        isGrounded = Physics2D.OverlapCircle(groundCheck.position, checkRadius, whatIsGround);
    }
}
