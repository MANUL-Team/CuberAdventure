using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{

    [SerializeField] private float speed = 5f;
    [SerializeField] private float x;
    private Rigidbody2D rb;
    [SerializeField] public float JumpForce = 300f;

    private GameObject obj;
    private Transform playerTransform;
    [SerializeField] private float xc, yc;
    [SerializeField] private bool isGrounded;
    [SerializeField] private bool isNotGrounded;
    [SerializeField] private bool levelEnded;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float checkRadius;
    [SerializeField] private LayerMask whatIsGround;
    [SerializeField] private LayerMask whatIsNotGround;
    [SerializeField] private LayerMask EndLevel;
    [SerializeField] private float moveInput;

    [SerializeField] private int DoubleJumps, DoubleJumpsValue;

    [SerializeField] private Joystick joystick;
    [SerializeField] private GameObject jumpPart, canvas;

    private AudioSource source;
    private AudioClip fallSound;
    private bool confusion;

    private int timeLand;

    private void OnTriggerEnter2D(Collider2D collision){
        if(collision.CompareTag("SuperBatut")){
            rb.velocity = new Vector2(rb.velocity.x, 30);
        }
    }
    public void PushAway(int direction, float pushPower)
    {
        if (rb == null || pushPower == 0)
        {
            return;
        }
        confusion = true;
        rb.AddForce(Vector2.up * pushPower*5);
        rb.AddForce(Vector2.right * direction * pushPower*3);
        Invoke("DeConfusion", 0.7f);
    }
    private void DeConfusion(){
        confusion = false;
    }


    public void menu(){
        SceneManager.LoadScene("Menu");
    }
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
        if(!isGrounded){
            if(DoubleJumps > 0 && Time.timeScale == 1){
                rb.velocity = new Vector2(rb.velocity.x, JumpForce);
                DoubleJumps--;
                GameObject particle = Instantiate(jumpPart, transform.position, Quaternion.identity);
                Destroy(particle, 0.5f);
            }
        }
        else{
            rb.velocity = new Vector2(rb.velocity.x, JumpForce);
            GameObject particle = Instantiate(jumpPart, transform.position, Quaternion.identity);
            Destroy(particle, 0.5f);
        }
    }
    private void CheckTurbine(){
        if(PlayerPrefs.GetInt("ChangedTurbine") == 0){
            DoubleJumpsValue = 1;
        }
        else if(PlayerPrefs.GetInt("ChangedTurbine") == 1){
            DoubleJumpsValue = 2;
        }
        else if(PlayerPrefs.GetInt("ChangedTurbine") == 2){
            DoubleJumpsValue = 3;
        }
    }
    private void CheckGusenici(){
        if(PlayerPrefs.GetInt("ChangedGusenici") == 0){
            speed = 10f;
        }
        else if(PlayerPrefs.GetInt("ChangedGusenici") == 1){
            speed = 12f;
        }
        else if(PlayerPrefs.GetInt("ChangedGusenici") == 2){
            speed = 14f;
        }
    }

    private void Awake() {
    }

    private void Start() {
        obj = GameObject.FindGameObjectWithTag("Player");
        rb = obj.GetComponent<Rigidbody2D>();
        playerTransform = obj.GetComponent<Transform>();
        source = obj.GetComponent<AudioSource>();
        fallSound = source.clip;
    }

    private void Update() {
        CheckTurbine();
        CheckGusenici();
        if(canvas.activeSelf == true){
            moveInput = joystick.Horizontal;
        }
        else{
            joystick.input = Vector2.zero;
            moveInput = 0;
        }
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
        if (xc > 0){
            Nothing();
        }

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
        
        if(moveInput > 0){
            PlayerPrefs.SetInt("PlayerRotation", 1);
        } else if(moveInput < 0){
            PlayerPrefs.SetInt("PlayerRotation", -1);
        }

        
    }
    private void FixedUpdate() {
        if(!confusion){
            rb.velocity = new Vector2(moveInput * speed, rb.velocity.y);
        }
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, checkRadius, whatIsGround);
        isNotGrounded = Physics2D.OverlapCircle(groundCheck.position, checkRadius, whatIsNotGround);
        levelEnded = Physics2D.OverlapCircle(groundCheck.position, checkRadius, EndLevel);
    }
}


