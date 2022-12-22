using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WeaponInventory : MonoBehaviour
{
    [SerializeField] private GameObject[]weaponR, weaponL;

    [SerializeField] private float attackRange, damage, timeBtwAttack, startTimeBtwAttack;
    [SerializeField] private Transform attackPosR, attackPosL, bulletPosL, bulletPosR;
    [SerializeField] private LayerMask enemy;
    [SerializeField] private GameObject bullet, sound;
    private GameObject bulletInst;
    private void Update() {
        damage = PlayerPrefs.GetInt("PlayerDmg") + PlayerPrefs.GetInt("DmgBonus") + PlayerPrefs.GetInt("LaserDmg");
        if(PlayerPrefs.GetInt("ChangedWeapon") == 0){
            PlayerPrefs.SetInt("PlayerDmg", 1);
        }
        if(PlayerPrefs.GetInt("ChangedWeapon") == 1){
            PlayerPrefs.SetInt("PlayerDmg", 10);
        }
        if(PlayerPrefs.GetInt("ChangedWeapon") == 2){
            PlayerPrefs.SetInt("PlayerDmg", 15);
        }
        if(PlayerPrefs.GetInt("ChangedWeapon") == 3){
            PlayerPrefs.SetInt("PlayerDmg", 20);
        }
        if(PlayerPrefs.GetInt("ChangedWeapon") == 4){
            PlayerPrefs.SetInt("PlayerDmg", 25);
        }
        if(PlayerPrefs.GetInt("ChangedWeapon") == 5){
            PlayerPrefs.SetInt("PlayerDmg", 30);
        }
        if(PlayerPrefs.GetInt("ChangedWeapon") == 6){
            PlayerPrefs.SetInt("PlayerDmg", 42);
        }
        if(PlayerPrefs.GetInt("ChangedLaser") == 0){
            PlayerPrefs.SetInt("LaserDmg", 0);
        }
        if(PlayerPrefs.GetInt("ChangedLaser") == 1){
            PlayerPrefs.SetInt("LaserDmg", 20);
        }
        if(PlayerPrefs.GetInt("ChangedLaser") == 2){
            PlayerPrefs.SetInt("LaserDmg", 50);
        }
        if(timeBtwAttack > 0){
            timeBtwAttack -= Time.deltaTime;
        }
    }

    public void Skill1(){
        attackRange = 0.73f;
        int direction = PlayerPrefs.GetInt("PlayerRotation");
        if(timeBtwAttack <= 0){
            GameObject instSound = Instantiate(sound, transform.position, Quaternion.identity);
            Destroy(instSound, 2f);
        if(PlayerPrefs.GetInt("PlayerRotation") == 1){
            weaponR[0].SetActive(true);
            Collider2D[] enemies = Physics2D.OverlapCircleAll(attackPosR.position, attackRange, enemy);
            for(int i = 0; i < enemies.Length; i++){
                enemies[i].GetComponent<MobController>().Damage(damage);
                enemies[i].GetComponent<MobController>().PushAway(direction, 200f);
            }
            timeBtwAttack = startTimeBtwAttack;
        } else if(PlayerPrefs.GetInt("PlayerRotation") == -1){
            weaponL[0].SetActive(true);
            Collider2D[] enemies = Physics2D.OverlapCircleAll(attackPosL.position, attackRange, enemy);
            for(int i = 0; i < enemies.Length; i++){
                enemies[i].GetComponent<MobController>().Damage(damage);
                enemies[i].GetComponent<MobController>().PushAway(direction, 200f);
            }
            timeBtwAttack = startTimeBtwAttack;
        }
        }
    }
    public void Skill2(){
        attackRange = 0.73f;
        int direction = PlayerPrefs.GetInt("PlayerRotation");
        if(timeBtwAttack <= 0){
            GameObject instSound = Instantiate(sound, transform.position, Quaternion.identity);
            Destroy(instSound, 2f);
            weaponR[0].SetActive(true);
            Collider2D[] enemies = Physics2D.OverlapCircleAll(attackPosR.position, attackRange, enemy);
            for(int i = 0; i < enemies.Length; i++){
                enemies[i].GetComponent<MobController>().Damage(damage);
                enemies[i].GetComponent<MobController>().PushAway(direction, 200f);
            }
            weaponL[0].SetActive(true);
            Collider2D[] enemies1 = Physics2D.OverlapCircleAll(attackPosL.position, attackRange, enemy);
            for(int i = 0; i < enemies1.Length; i++){
                enemies1[i].GetComponent<MobController>().Damage(damage);
                enemies[i].GetComponent<MobController>().PushAway(direction, 200f);
            }
            timeBtwAttack = startTimeBtwAttack;
        }
    }
    public void Skill3(){
        attackRange = 1.15f;
        int direction = PlayerPrefs.GetInt("PlayerRotation");
        if(timeBtwAttack <= 0){
            GameObject instSound = Instantiate(sound, transform.position, Quaternion.identity);
            Destroy(instSound, 2f);
        if(PlayerPrefs.GetInt("PlayerRotation") == 1){
            weaponR[1].SetActive(true);
            weaponR[1].GetComponent<Animator>().SetBool("Skill3", true);
            Collider2D[] enemies = Physics2D.OverlapCircleAll(attackPosR.position, attackRange, enemy);
            for(int i = 0; i < enemies.Length; i++){
                enemies[i].GetComponent<MobController>().Damage(damage * 0.75f);
                enemies[i].GetComponent<MobController>().PushAway(direction, 200f);
            }
            timeBtwAttack = startTimeBtwAttack;
        } else if(PlayerPrefs.GetInt("PlayerRotation") == -1){
            weaponL[1].SetActive(true);
            weaponL[1].GetComponent<Animator>().SetBool("Skill3", true);
            Collider2D[] enemies = Physics2D.OverlapCircleAll(attackPosL.position, attackRange, enemy);
            for(int i = 0; i < enemies.Length; i++){
                enemies[i].GetComponent<MobController>().Damage(damage * 0.75f);
                enemies[i].GetComponent<MobController>().PushAway(direction, 50f);
            }
            timeBtwAttack = startTimeBtwAttack;
        }
        }
    }
    public void Skill4(){
        attackRange = 0.35f;
        int direction = PlayerPrefs.GetInt("PlayerRotation");
        if(timeBtwAttack <= 0){
            GameObject instSound = Instantiate(sound, transform.position, Quaternion.identity);
            Destroy(instSound, 2f);
        if(PlayerPrefs.GetInt("PlayerRotation") == 1){
            weaponR[2].SetActive(true);
            weaponR[2].GetComponent<Animator>().SetBool("Skill4", true);
            Collider2D[] enemies = Physics2D.OverlapCircleAll(attackPosR.position, attackRange, enemy);
            for(int i = 0; i < enemies.Length; i++){
                enemies[i].GetComponent<MobController>().Damage(damage * 1.25f);
                enemies[i].GetComponent<MobController>().PushAway(direction, 200f);
            }
            timeBtwAttack = startTimeBtwAttack;
        } else if(PlayerPrefs.GetInt("PlayerRotation") == -1){
            weaponL[2].SetActive(true);
            weaponL[2].GetComponent<Animator>().SetBool("Skill4", true);
            Collider2D[] enemies = Physics2D.OverlapCircleAll(attackPosL.position, attackRange, enemy);
            for(int i = 0; i < enemies.Length; i++){
                enemies[i].GetComponent<MobController>().Damage(damage * 1.25f);
                enemies[i].GetComponent<MobController>().PushAway(direction, 200f);
            }
            timeBtwAttack = startTimeBtwAttack;
        }
        }
    }
    public void Skill5(){
        if(timeBtwAttack <= 0){
            GameObject instSound = Instantiate(sound, transform.position, Quaternion.identity);
            Destroy(instSound, 2f);
        if(PlayerPrefs.GetInt("PlayerRotation") == 1){
            bulletInst = Instantiate(bullet, bulletPosR.position, Quaternion.identity);
            bulletInst.transform.Rotate(0, 0, -90);
            Destroy(bulletInst, 2f);
        } else if(PlayerPrefs.GetInt("PlayerRotation") == -1){
            bulletInst = Instantiate(bullet, bulletPosL.position, Quaternion.identity);
            bulletInst.transform.Rotate(0, 0, 90);
            Destroy(bulletInst, 2f);
        }
        timeBtwAttack = startTimeBtwAttack;
        }
    }
    public void WeaponUnActiveate(){
        gameObject.SetActive(false);
    }
    private void OnDrawGizmosSelected() {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackPosR.position, attackRange);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackPosL.position, attackRange);
    }
}
