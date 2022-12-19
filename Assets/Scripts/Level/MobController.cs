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
    [SerializeField] private float height;
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
    private Animator anim;
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
    public string displayDieTime;
    [SerializeField] private bool isHasDrop;
    [SerializeField] private int itemId, count;
    
    public void Damage(float damage){
        if(maneken == false){
            hp = hp - damage;
            GameObject soundDmg = Instantiate(soundDmgObj, transform.position, Quaternion.identity);
            Destroy(soundDmg, 1f);
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

    private void OnCollisionStay2D(Collision2D collision){
        if(collision.gameObject.tag == "Player"){
            if(startTimeBtwAttack > 0){
            if(timeBtwAttack <= 0){
                anim.SetTrigger("Attack");
                Collider2D[] playerCol = Physics2D.OverlapCircleAll(attackPos.position, attackRange, playerLayer);
                for(int i = 0; i < playerCol.Length; i++){
                    playerCol[i].GetComponent<PlayerStats>().Damage(10f);
                    playerCol[i].GetComponent<PlayerController>().PushAway(direction, 5000f);
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
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();
        hp = maxHp;
        }
    }

    private void Update() {
        if(agressiveMob || pathFinding){
        if(ai.desiredVelocity.x <= -0.01f){
            anim.SetBool("Run", true);
            transform.rotation = new Quaternion(0, 0, 0, transform.rotation.w);
            direction = -1;
            
        } else if(ai.desiredVelocity.x >= 0.01f){
            anim.SetBool("Run", true);
            transform.rotation = new Quaternion(0, 180, 0, transform.rotation.w);
            direction = 1;
        }
        else{
            anim.SetBool("Run", false);
            transform.rotation = new Quaternion(0, 0, 0, transform.rotation.w);
        }
        }
        if(maneken == false){
        healthBar.fillAmount = hp/maxHp;
        displayDieTime = PlayerPrefs.GetString("MobTimeDie" + id + num);
        if(hp <= 0){
            PlayerPrefs.SetInt("Exp", PlayerPrefs.GetInt("Exp") + 20);
            PlayerPrefs.SetInt("SkillPoints", PlayerPrefs.GetInt("SkillPoints") + skillPoints);
            DateTime dieTime = DateTime.Now;
            PlayerPrefs.SetString("MobTimeDie" + id + num, dieTime.ToString());
            if(isHasDrop){
                PlayerPrefs.SetInt("Item" + "Mob" + itemId, PlayerPrefs.GetInt("Item" + "Mob" + itemId) + count);
            }
            displayDieTime = PlayerPrefs.GetString("MobTimeDie" + id + num);
            GameObject blood = Instantiate(bloodEffects, GetComponent<Transform>().position, Quaternion.identity);
            Destroy(blood, 1f);
            canvas.localScale = new Vector3(0, 0, 0);
            gameObject.SetActive(false);
        }
        canvas.position = new Vector2(transform.position.x, transform.position.y + height);
        if(timeBtwAttack > 0){
            timeBtwAttack -= Time.deltaTime;
        }
        distToPlayer = Vector2.Distance(transform.position, player.position);
        if(!confusion || pathFinding){
            if(distToPlayer < 8){
                StartHunter();
            }
            else{
                EndHunter();
            }
        }else if(pathFinding){
            EndHunter();
        }
        }
    }
    private void StartHunter(){
        if(startTimeBtwAttack > 0){
            anim.SetBool("Run", true);
        if(transform.position.x > player.position.x){
            speedx = -4f;
            transform.rotation = new Quaternion(0, 0, 0, transform.rotation.w);
            direction = -1;
        }
        else if(transform.position.x < player.position.x){
            speedx = 4f;
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
        if(!confusion){
            isGrounded = Physics2D.OverlapCircle(groundCheck.position, checkRadius, whatIsGround);
        }else{
            isGrounded = false;
        }
        if(agressiveMob){
            if(isGrounded){
                rb.velocity = new Vector2(speedx, rb.velocity.y);
            }
        }
    }
}
