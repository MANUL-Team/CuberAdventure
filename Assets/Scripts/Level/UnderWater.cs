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
    [SerializeField] private float maxAir;
    [SerializeField] private Image scaleAir, scaleAirOutline;
    [SerializeField] private bool canAirLose;

    void OnTriggerEnter2D (Collider2D collision){
        if (collision.CompareTag("Water")){
            pc.AnyJumpForce(8);
            loseAir = true;
            StartCoroutine(AirLose());
            StopCoroutine(AirKeep());
        }
    }
    void OnTriggerExit2D (Collider2D collision){
        if (collision.CompareTag("Water")){
            pc.AnyJumpForce(17);
            loseAir = false;
            StopCoroutine(AirLose());
            StartCoroutine(AirKeep());
        }
    }
    private IEnumerator AirLose(){
        while(loseAir){
            if(canAirLose){
                if(air > 0){
                    air = air - 15f;
                }
                else{
                    ps.Damage(10f);
                } 
            }
            yield return new WaitForSeconds(1f);
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
        maxAir = PlayerPrefs.GetFloat("MaxAir");
        air = maxAir;
    }
    private void Update() {
        maxAir = PlayerPrefs.GetFloat("MaxAir");
        if(PlayerPrefs.GetInt("CanLoseAir") == 0){
            canAirLose = false;
        }
        else{
            canAirLose = true;
        }
        if(air < maxAir){
            scaleAirOutline.gameObject.SetActive(true);
        }
        else{
            scaleAirOutline.gameObject.SetActive(false);
        }
        scaleAir.fillAmount = air/maxAir;
    }
}
