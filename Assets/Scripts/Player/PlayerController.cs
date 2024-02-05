using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

public class PlayerController : MonoBehaviour
{

    [SerializeField] private float speed = 5f;
    private float startSpeed = 10;
    [SerializeField] private float x;
    private Rigidbody2D rb;
    [SerializeField] private float JumpForce = 300f;
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
    [SerializeField] private float moveInput, moveInputY;
    [SerializeField] private int DoubleJumps, DoubleJumpsValue;
    [SerializeField] private Joystick joystick, uwJoystick;
    [SerializeField] private GameObject jumpPart, canvas, flyButton;
    [SerializeField] private ModulesController modules;
    private float speedMnojitel;
    private AudioSource source;
    private AudioClip fallSound;
    private bool confusion;
    private int timeLand;
    [SerializeField] private float falling;
    private PlayerStats ps;
    [SerializeField] private UnderWater uw;
    private float fallSpeed;
    private bool flying = false;
    private float flyTime = 0;
    private Camera mainCamera;
    private float standartCameraDist = 7.199291f;
    private float camDistIndex = 0f;

    public void Confuse(){
        confusion = true;
        rb.velocity = new Vector2(0, 0);
        Invoke("DeConfusion", 3f);
    }
    public void AnyJumpForce(float jf){
        JumpForce = jf;
    }
    private IEnumerator Falling(){
        while(true){
            if(!isGrounded){
                falling += 0.1f;
            }
            yield return new WaitForSeconds(0.1f);
        }
    }
    private void OnTriggerEnter2D(Collider2D collision){
        if(collision.CompareTag("SuperBatut")){
            rb.velocity = new Vector2(rb.velocity.x, 30);
        }
    }
    public void PushAway(int direction, float pushPower, float deConfusion)
    {
        if (rb == null || pushPower == 0)
        {
            return;
        }
        confusion = true;
        rb.AddForce(Vector2.up * pushPower*5);
        rb.AddForce(Vector2.right * direction * pushPower*3);
        Invoke("DeConfusion", deConfusion);
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
                falling = 0;
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
        JumpForce = modules._turbine.currentJumpHeight;
        DoubleJumpsValue = modules._turbine.jumpCount;
        if (modules._turbine.flyTime != 0)
        {
            flyButton.SetActive(true);
        }
        else
        {
            flyButton.SetActive(false);
        }
    }
    private void CheckUWS(){
        if(uw.loseAir){
            speed = modules._uws.currentSpeed;
        }
    }
    private void Start() {
        obj = GameObject.FindGameObjectWithTag("Player");
        mainCamera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
        rb = obj.GetComponent<Rigidbody2D>();
        playerTransform = obj.GetComponent<Transform>();
        source = obj.GetComponent<AudioSource>();
        fallSound = source.clip;
        ps = GetComponent<PlayerStats>();
        StartCoroutine(Falling());
    }

    private void Update() {
        if(!isGrounded){
            fallSpeed = rb.velocity.y;
        }
        if(falling != 0 && isGrounded){
            if(fallSpeed <= -20f && fallSpeed > -25f && !uw.loseAir){
                ps.Damage(10f);
                fallSpeed = 0;
                falling = 0;
            }
            else if(fallSpeed <= -25f && !uw.loseAir){
                ps.Damage(20f);
                fallSpeed = 0;
                falling = 0;
            }
            else{
                fallSpeed = 0;
                falling = 0;
            }
            if(falling >= 5f){
                ps.Damage(40f);
                fallSpeed = 0;
                falling = 0;
            }
        }
        if(joystick.gameObject.activeSelf == true)
        {
            if(canvas.activeSelf == true){
                moveInput = joystick.Horizontal;
            }
            else{
                joystick.input = Vector2.zero;
                moveInput = 0;
            }
            moveInputY = 0;
        }
        else{
            moveInput = uwJoystick.Horizontal;
            moveInputY = uwJoystick.Vertical;
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
        if (Input.GetKeyDown(KeyCode.W)){
            Jump();
        }
        
        if(moveInput > 0){
            PlayerPrefs.SetInt("PlayerRotation", 1);
        } else if(moveInput < 0){
            PlayerPrefs.SetInt("PlayerRotation", -1);
        }
    }

    public void FixedUpdate()
    {
        CheckTurbine();
        CheckUWS();
        float camDist = 0;
        if (camDistIndex < (Math.Abs(rb.velocity.x) / 25))
        {
            camDist = Mathf.Lerp(standartCameraDist, 10, camDistIndex);
        }
        else
        {
            for (int i = 0; i < 10; i++)
            {
                float curDist = Math.Clamp(camDistIndex - 0.1f, 0, 1);
                camDist = Mathf.Lerp(standartCameraDist, 10, curDist);
            }
        }
        //mainCamera.orthographicSize = Math.Clamp(camDist, standartCameraDist, 10);
        camDistIndex = (Math.Abs(rb.velocity.x) / 25);
        int clutchInt = Random.Range(0, 100);
        if (isGrounded)
        {
            flyTime = modules._turbine.flyTime;
        }
        if(joystick.gameObject.activeSelf)
        {
            if (!uw.loseAir && clutchInt > modules._tracks.clutch)
            {
                if(moveInput > 0){
                    speed = Math.Clamp(speed + (modules._tracks.acceleration), -modules._tracks.currentSpeed, modules._tracks.currentSpeed);
                }
                else if(moveInput < 0)
                {
                    speed = Math.Clamp(speed - (modules._tracks.acceleration), -modules._tracks.currentSpeed, modules._tracks.currentSpeed);
                }
                else
                {
                    if (speed > 0)
                    {
                        speed = Math.Clamp(speed - 0.5f, 0, modules._tracks.currentSpeed);
                    }
                    else
                    {
                        speed = Math.Clamp(speed + 0.5f, -modules._tracks.currentSpeed, 0);
                    }
                }
            }
            else if (clutchInt <= modules._tracks.clutch)
            {
                speed = speed * 0.95f;
            }
            if(!confusion){
                rb.velocity = new Vector2( speed, rb.velocity.y);
            }
        }
        else{
            if(!confusion && moveInputY == 0){
                rb.velocity = new Vector2(moveInput * speed, rb.velocity.y);
            }
            else if(!confusion && moveInputY != 0){
                rb.velocity = new Vector2(moveInput * speed, moveInputY * speed);
            }
        }

        if (flying)
        {
            flyTime -= 0.1f;
            float currentSpeedX = rb.velocity.x;
            if (currentSpeedX > 0)
            {
                currentSpeedX = Math.Clamp(currentSpeedX - 0.1f, 0, 50);
            }
            else if (currentSpeedX < 0)
            {
                currentSpeedX = Math.Clamp(currentSpeedX + 0.1f, -50, 0);
            }
            rb.velocity = new Vector2(currentSpeedX, modules._turbine.jumpHeight/3);
            if (flyTime <= 0)
            {
                flying = false;
            }
        }
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, checkRadius, whatIsGround);
        isNotGrounded = Physics2D.OverlapCircle(groundCheck.position, checkRadius, whatIsNotGround);
        levelEnded = Physics2D.OverlapCircle(groundCheck.position, checkRadius, EndLevel);
    }
    private void OnDrawGizmosSelected() {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(groundCheck.position, checkRadius);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(groundCheck.position, checkRadius);
    }
    public void ToFly()
    {
        if (flyTime > 0)
        {
            flying = true;
        }
    }

    public void FromFly()
    {
        flying = false;
    }
}


