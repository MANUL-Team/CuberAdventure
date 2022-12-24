using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UnderWater : MonoBehaviour
{
    private PlayerController pc;
    [SerializeField] private float air;
    private PlayerStats ps;
    public bool loseAir;
    [SerializeField] private Image scaleAir, scaleAirOutline;

    void OnTriggerEnter2D (Collider2D collision){
        if (collision.CompareTag("Water")){
            pc.JumpForce = 8;
            loseAir = true;
            StartCoroutine(AirLose());
            StopCoroutine(AirKeep());
        }
    }
    void OnTriggerExit2D (Collider2D collision){
        if (collision.CompareTag("Water")){
            pc.JumpForce = 17;
            loseAir = false;
            StopCoroutine(AirLose());
            StartCoroutine(AirKeep());
        }
    }
    private IEnumerator AirLose(){
        while(loseAir){
            if(air > 0){
                air = air - 3;
            }
            else{
                ps.Damage(5f);
            }
            yield return new WaitForSeconds(0.2f);
        }
    }
    private IEnumerator AirKeep(){
        while(!loseAir){
            air = Mathf.Clamp(air + 5, 0, 100);
            yield return new WaitForSeconds(0.2f);
        }
    }
    void Start(){
        pc = GetComponent<PlayerController>();
        ps = GetComponent<PlayerStats>();
        air = 100;
    }
    private void Update() {
        if(air < 100){
            scaleAirOutline.gameObject.SetActive(true);
        }
        else{
            scaleAirOutline.gameObject.SetActive(false);
        }
        scaleAir.fillAmount = air/100;
    }
}
