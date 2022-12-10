using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Pun;
using UnityEngine.UI;

public class PlayerControlServ : MonoBehaviour
{


    [SerializeField] private float speed = 5f;
    [SerializeField] private float x;
    private Rigidbody2D rb;
    [SerializeField] private float JumpForce = 20f;

    private GameObject obj;
    private Transform playerTransform;
    [SerializeField] private float xc, yc;
    [SerializeField] private bool isGrounded;
    [SerializeField] private bool isNotGrounded;
    [SerializeField] private bool levelEnded;
    private Transform groundCheck;
    [SerializeField] private float checkRadius;
    [SerializeField] private LayerMask whatIsGround;
    [SerializeField] private LayerMask whatIsNotGround;
    [SerializeField] private LayerMask EndLevel;
    [SerializeField] private float moveInput;

    [SerializeField] private int DoubleJumps, DoubleJumpsValue;

    [SerializeField] private Joystick joystick;

    PhotonView view;
    [SerializeField] private GameObject cameraM;
    [SerializeField] private PlayerControlServ playercontrol;
    [SerializeField] private GameObject canvas;

    [SerializeField] private Text textName;

    private AudioSource source;
    private AudioClip fallSound;

    private int timeLand;



    public void Right(){
        moveInput = 1;
    }

    public void Left(){
        moveInput = -1;
    }

    public void Nothing(){
        moveInput = 0;
    }

    public void OnTriggerEnter2D(Collider2D collision){
        if(collision.CompareTag("Ground")){
            isGrounded = true;
        }
        else{
            isGrounded = false;
        }
    }

    public void Jump(){
        if(view.IsMine){
            if(DoubleJumps > 0 && isNotGrounded == false){
                rb.AddForce(Vector2.up * JumpForce);
                DoubleJumps--;
            }
        }
    }
    private void Start() {
        view = GetComponent<PhotonView>();
        rb = GetComponent<Rigidbody2D>();
        playerTransform = GetComponent<Transform>();
        groundCheck = GameObject.FindGameObjectWithTag("GroundCheck").GetComponent<Transform>();
        playercontrol = GetComponent<PlayerControlServ>();
        textName.text = view.Owner.NickName;
        source = GetComponent<AudioSource>();
        fallSound = source.clip;
    }

    private void Update() {
        if(!view.IsMine){
            cameraM.SetActive(false);
            playercontrol.enabled = false;
            canvas.SetActive(false);
        }



        if(view.IsMine){

        moveInput = joystick.Horizontal;


        if(isGrounded){
            DoubleJumps = DoubleJumpsValue;
            timeLand++;
            if(timeLand >= 2){
                timeLand = 2;
            }
            if(timeLand < 2){
                source.PlayOneShot(fallSound);
            }
        }
        if(!isGrounded){
            timeLand = 0;
        }

        if(levelEnded){
            PlayerPrefs.SetInt("Level", PlayerPrefs.GetInt("Level") + 1);
            if(PlayerPrefs.GetInt("Level") == 1){
                SceneManager.LoadScene("Game");
            }
            if(PlayerPrefs.GetInt("Level") == 2){
                SceneManager.LoadScene("Level2");
            }
            if(PlayerPrefs.GetInt("Level") > 2){
                SceneManager.LoadScene("Menu");
            }
        }

        if (xc > 0){
            Nothing();
        }
        //Управление с компьютера

        if (Input.GetKey(KeyCode.D) && xc <= 0){
            Right();
        }
        else if (Input.GetKey(KeyCode.A) && xc <= 0){
            Left();
        }
        if (Input.GetKeyUp(KeyCode.D) && xc <= 0){
            Nothing();
        }
        if (Input.GetKeyUp(KeyCode.A) && xc <= 0){
            Nothing();
        }
        if (Input.GetKeyDown(KeyCode.Space)){
            Jump();
        }
    }
        
        
    }
    private void FixedUpdate() {


        //moveInput = Input.GetAxisRaw("Horizontal");
        rb.velocity = new Vector2(moveInput * speed, rb.velocity.y);

        isGrounded = Physics2D.OverlapCircle(groundCheck.position, checkRadius, whatIsGround);
        isNotGrounded = Physics2D.OverlapCircle(groundCheck.position, checkRadius, whatIsNotGround);
        levelEnded = Physics2D.OverlapCircle(groundCheck.position, checkRadius, EndLevel);


//        if(playerTransform.position.x - camera.position.x > 18){
//            camera.Translate(new Vector2(0, 20));
  //      }

//        if(playerTransform.position.x - camera.position.x < -10f){
//            xc = 0;
//        }

//            -3                      1.2
//        if(playerTransform.position.y + camera.position.y < -1.8f && playerTransform.position.y + camera.position.y >= -3f){
 //           yc = -20;
  //      }            
   //     if(camera.position.y <= -5.5f){
     //       yc = 0;
       // }
    }
}
