using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MobMove : MonoBehaviour
{
    private int move;
    private Animator anim;
    private int speedx;
    private Rigidbody2D rb;
    private bool isGrounded;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float checkRadius;
    [SerializeField] private LayerMask whatIsGround;
    [SerializeField] private int speed;

    IEnumerator RandomMove(){
        yield return new WaitForSeconds(3f);
        move = Random.Range(-2, 2);
        if(move == 0){
            speedx = 0;
            anim.SetBool("Run", false);
        }
        else if(move == 1){
            speedx = speed;
            anim.SetBool("Run", true);
            transform.rotation = new Quaternion(0, 180, 0, transform.rotation.w);
        }
        else if(move == -1){
            speedx = -speed;
            anim.SetBool("Run", true);
            transform.rotation = new Quaternion(0, 0, 0, transform.rotation.w);
        }
        StartCoroutine(RandomMove());
    }
    private void Start() {
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        StartCoroutine(RandomMove());
    }
    private void FixedUpdate() {
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, checkRadius, whatIsGround);
        rb.velocity = new Vector2(speedx, rb.velocity.y);
    }
}
