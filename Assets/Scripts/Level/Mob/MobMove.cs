using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DefaultExecutionOrder(10000)]
public class MobMove : MonoBehaviour
{
    private int move;
    private Animator anim;
    private int speedx;
    private Rigidbody2D rb;
    private MobController facing;
    private bool faceRight;
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
            Turn(true);
        }
        else if(move == -1){
            speedx = -speed;
            anim.SetBool("Run", true);
            Turn(false);
        }
        StartCoroutine(RandomMove());
    }
    private void Start() {
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        facing = GetComponent<MobController>();
        StartCoroutine(RandomMove());
    }
    void Turn(bool right) {
        faceRight = right;
        if (facing != null)
            facing.FaceSide(right);
        else
            MobController.PresentFacing(transform, right);
    }

    private void FixedUpdate() {
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, checkRadius, whatIsGround);
        rb.linearVelocity = new Vector2(speedx, rb.linearVelocity.y);
    }

    private void LateUpdate() {
        if (facing != null)
            return;
        MobController.PresentFacing(transform, faceRight);
    }
}
