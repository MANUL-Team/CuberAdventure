using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Pathfinding;

public class MobController : MonoBehaviour
{
    private Rigidbody2D rb;
    private Transform player;
    private float speedx, distToPlayer;
    public float hp;
    public float maxHp;
    [SerializeField] private Image healthBar;
    [SerializeField] private GameObject bloodEffects;
    [SerializeField] private LayerMask playerLayer;
    private float timeBtwAttack;
    [SerializeField] private float startTimeBtwAttack, attackRange;
    [SerializeField] private Transform attackPos;
    public RectTransform canvas;
    public GameObject bars;
    [SerializeField] private Animator anim;
    [SerializeField] private GameObject drop, soundDmgObj;
    [SerializeField] private int skillPoints;
    [SerializeField] private bool maneken, right, left, isGrounded, confusion, agressiveMob, pathFinding;
    [SerializeField] private Animator manekenAnim;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float checkRadius;
    [SerializeField] private LayerMask whatIsGround;
    [SerializeField] private AIPath ai;
    private int direction;
    public int id, num, delay;
    [SerializeField] private bool isHasDrop;
    [SerializeField] private Text dmgText;
    [SerializeField] private bool needGC, isTrigger, rotate;
    [SerializeField] private Transform canvasPosition, bloodPos;
    [SerializeField] private float damage;
    [SerializeField] private int exp;
    [SerializeField] private GameObject mobObj;
    private GiveDrop giveDrop;
    [SerializeField] private float distance, speed, pushPower, deConfusion;
    
    public void Damage(float damage){
        if(maneken == false){
            dmgText.gameObject.SetActive(true);
            dmgText.text = Mathf.RoundToInt(damage).ToString();
            GameObject soundDmg = Instantiate(soundDmgObj, transform.position, Quaternion.identity);
            Destroy(soundDmg, 1f);
            hp = hp - damage;
        }
        else{
            if(right){
                manekenAnim.SetBool("Right", true);
                manekenAnim.SetBool("Left", false);
            }
            else if(left){
                manekenAnim.SetBool("Right", false);
                manekenAnim.SetBool("Left", true);
            }
        }
        
    }

    public void PushAway(int direction, float pushPower)
    {
        if (rb == null || pushPower == 0)
        {
            return;
        }
        confusion = true;
        rb.AddForce(Vector2.up * pushPower*50f);
        rb.AddForce(Vector2.right * direction * pushPower*100f);
        Invoke("DeConfusion", 0.5f);
    }
    private void DeConfusion(){
        confusion = false;
    }
    void OnCollisionStay2D(Collision2D collision) {
        if(collision.gameObject.tag == "Player"){
            if(startTimeBtwAttack > 0){
                if(timeBtwAttack <= 0){
                    anim.SetTrigger("Attack");
                    Collider2D[] playerCol = Physics2D.OverlapCircleAll(attackPos.position, attackRange, playerLayer);
                    for(int i = 0; i < playerCol.Length; i++){
                        playerCol[i].GetComponent<PlayerStats>().Damage(damage);
                        playerCol[i].GetComponent<PlayerController>().PushAway(direction, pushPower, deConfusion);
                    }
                    timeBtwAttack = startTimeBtwAttack;
                }
            }
        }
    }
    void OnTriggerStay2D(Collider2D collision) {
        if(collision.CompareTag("Player")){
            if(startTimeBtwAttack > 0){
                if(timeBtwAttack <= 0){
                    anim.SetTrigger("Attack");
                    Collider2D[] playerCol = Physics2D.OverlapCircleAll(attackPos.position, attackRange, playerLayer);
                    for(int i = 0; i < playerCol.Length; i++){
                        playerCol[i].GetComponent<PlayerStats>().Damage(damage);
                        playerCol[i].GetComponent<PlayerController>().PushAway(direction, pushPower, deConfusion);
                    }
                    timeBtwAttack = startTimeBtwAttack;
                }
            }
        }
    }
    private void OnDrawGizmosSelected() {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackPos.position, attackRange);
    }

    private void Start() {
        if(maneken == false){
            player = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();
            hp = maxHp;
            rb = GetComponent<Rigidbody2D>();
            giveDrop = GetComponent<GiveDrop>();
            StartCoroutine(Hunter());
        }
    }
    private void DeathCheck(){
        if(hp <= 0){
            if(ai != null){
                ai.canMove = false;
            }
            PlayerPrefs.SetInt("Exp", PlayerPrefs.GetInt("Exp") + exp);
            PlayerPrefs.SetInt("SkillPoints", PlayerPrefs.GetInt("SkillPoints") + skillPoints);
            DateTime dieTime = DateTime.Now;
            PlayerPrefs.SetString("MobTimeDie" + id + " " + num, dieTime.ToString());
            if(isHasDrop){
                giveDrop.Drop();
            }
            GameObject blood = Instantiate(bloodEffects, bloodPos.position, Quaternion.identity);
            Destroy(blood, 1f);
            bars.SetActive(false);
            gameObject.SetActive(false);
        }
    }
    private void OnEnable() {
        if(ai != null){
            ai.canMove = true;
        }
    }

    private void Update() {
        if(maneken == false){
            healthBar.fillAmount = hp/maxHp;
            DeathCheck();
            if(canvasPosition != null){
                canvas.position = new Vector2(canvasPosition.position.x, canvasPosition.position.y);
            }
            if(timeBtwAttack > 0){
                timeBtwAttack -= Time.deltaTime;
            }
            distToPlayer = Vector2.Distance(transform.position, player.position);
            if(distToPlayer >= 30){
                mobObj.SetActive(false);
            }
            else{
                mobObj.SetActive(true);
            }
        }
    }
    private IEnumerator Hunter(){
        while(true){
            if(!confusion && !pathFinding && agressiveMob){
                if(distToPlayer < distance){
                    StartHunter();
                }
                else{
                    EndHunter();
                }
            }else if(pathFinding){
                EndHunter();
            }
            if(rotate){
                Rotate();
            }
            yield return new WaitForSeconds(0.5f);
        }
    }
    private void Rotate(){
        if(transform.position.x > player.position.x){
            transform.rotation = new Quaternion(0, 0, 0, transform.rotation.w);
            direction = -1;
        }
        else if(transform.position.x < player.position.x){
            if(player.position.x - transform.position.x <= 5){
                transform.rotation = new Quaternion(0, 0, 0, transform.rotation.w);
                direction = -1;
            }
            else{
                transform.rotation = new Quaternion(0, 180, 0, transform.rotation.w);
                direction = 1;
            }
        }
    }
    private void StartHunter(){
        if(startTimeBtwAttack > 0){
            anim.SetBool("Run", true);
            if(transform.position.x > player.position.x){
                speedx = -speed;
                transform.rotation = new Quaternion(0, 0, 0, transform.rotation.w);
                direction = -1;
            }
            else if(transform.position.x < player.position.x){
                speedx = speed;
                transform.rotation = new Quaternion(0, 180, 0, transform.rotation.w);
                direction = 1;
            }
        }
    }
    private void EndHunter(){
        if(startTimeBtwAttack > 0){
            anim.SetBool("Run", false);
            speedx = 0;
        }
    }
    private void FixedUpdate() {
        if(!confusion && needGC){
            isGrounded = Physics2D.OverlapCircle(groundCheck.position, checkRadius, whatIsGround);
        }else{
            isGrounded = false;
        }
        if(agressiveMob && !pathFinding){
            if(isGrounded){
                rb.velocity = new Vector2(speedx, rb.velocity.y);
            }
            else if(!needGC){
                rb.velocity = new Vector2(speedx, rb.velocity.y);
            }
        }
    }
}
