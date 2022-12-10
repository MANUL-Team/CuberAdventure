using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PointOffline : MonoBehaviour
{
    private GameObject player;

    Animator animator;

    [SerializeField] private Transform groundCheck;
    [SerializeField] private float checkRadius;
    [SerializeField] private LayerMask whatIsGround;
    [SerializeField] private LayerMask whatIsNotGround;
    [SerializeField] private LayerMask EndLevel;
    [SerializeField] private bool isGrounded;
    [SerializeField] private bool isNotGrounded;
    [SerializeField] private bool levelEnded;



    private void Awake() {
        
    }
    void Start()
    {
        animator = GameObject.FindGameObjectWithTag("PlayerSkin").GetComponent<Animator>();
    }


    void Update()
    {
            if(isGrounded == false){
                animator.SetBool("Jump", true);
            }
            if(isGrounded){
                animator.SetBool("Jump", false);
            }
    }
    private void FixedUpdate() {
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, checkRadius, whatIsGround);
    }
}
