using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Bullet : MonoBehaviour
{
    [SerializeField] private float speed, lifeTime, distance, damage;
    [SerializeField] private LayerMask waitIsSolid;
    [SerializeField] private GameObject particles;
    [SerializeField] private int direction;

    private void FixedUpdate() {
        RaycastHit2D hitInfo = Physics2D.Raycast(transform.position, transform.up, distance, waitIsSolid);
        if(hitInfo.collider != null){
            if(hitInfo.collider.CompareTag("Mob")){
                hitInfo.collider.GetComponent<MobController>().Damage(damage * 0.6f);
                hitInfo.collider.GetComponent<MobController>().PushAway(direction, 200f);
            }
            GameObject part = Instantiate(particles, transform.position, Quaternion.identity);
            Destroy(gameObject);
            Destroy(part, 2f);
        }
        transform.Translate(Vector2.up * speed * Time.deltaTime);
    }
    private void Start() {
        direction = PlayerPrefs.GetInt("PlayerRotation");
        damage = WeaponInventory.damage + PlayerPrefs.GetInt("DmgBonus") / 3 + PlayerPrefs.GetInt("LaserDmg");
    }

}
