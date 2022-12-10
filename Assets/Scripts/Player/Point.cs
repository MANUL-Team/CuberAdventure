using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Photon.Pun;

public class Point : MonoBehaviour
{


    [SerializeField] private GameObject player;

    Animator animator;

    private Transform groundCheck;
    [SerializeField] private float checkRadius;
    [SerializeField] private LayerMask whatIsGround;
    [SerializeField] private LayerMask whatIsNotGround;
    [SerializeField] private LayerMask EndLevel;
    [SerializeField] private bool isGrounded;
    [SerializeField] private bool isNotGrounded;
    [SerializeField] private bool levelEnded;

    PhotonView view;



    void Start()
    {
        animator = player.GetComponent<Animator>();
        groundCheck = GameObject.FindGameObjectWithTag("GroundCheck").GetComponent<Transform>();
        view = GetComponent<PhotonView>();
    }


    void Update()
    {
        if(view.IsMine){
            if(isGrounded == false){
                animator.SetBool("Jump", true);
            }
            if(isGrounded){
                animator.SetBool("Jump", false);
            }
        }
    }
    private void FixedUpdate() {
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, checkRadius, whatIsGround);
    }
}
