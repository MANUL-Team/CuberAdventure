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
    [SerializeField] private JoystickManager jm;

    void OnTriggerEnter2D (Collider2D collision){
        if (collision.CompareTag("Water")){
            pc.AnyJumpForce(8);
            loseAir = true;
            if(PlayerPrefs.GetInt("ChangedUnderWaterSystem") != 0){
                jm.JoystickSwitch(loseAir);
            }
        }
    }
    void OnTriggerExit2D (Collider2D collision){
        if (collision.CompareTag("Water")){
            pc.AnyJumpForce(17);
            loseAir = false;
            if(PlayerPrefs.GetInt("ChangedUnderWaterSystem") != 0){
                jm.JoystickSwitch(loseAir);
            }
        }
    }
    private IEnumerator AirController(){
        while(true){
            if(loseAir){
                if(canAirLose){
                    if(air > 0){
                        air = Mathf.Clamp(air - 10, 0, maxAir);
                    }
                    else{
                        ps.Damage(10f);
                    } 
                }
            }
            else{
                air = Mathf.Clamp(air + 25, 0, maxAir);
            }
            yield return new WaitForSeconds(1f);
        }
    }
    void Start(){
        pc = GetComponent<PlayerController>();
        ps = GetComponent<PlayerStats>();
        maxAir = PlayerPrefs.GetFloat("MaxAir");
        air = maxAir;
        StartCoroutine(AirController());
        StartCoroutine(LowUpdate());
    }
    private IEnumerator LowUpdate(){
        while(true){
            if(PlayerPrefs.GetInt("ChangedUnderWaterSystem") != 0){
                jm.JoystickSwitch(loseAir);
            }
            else{
                jm.JoystickSwitch(false);
            }
            if(PlayerPrefs.GetInt("ChangedUnderWaterSystem") == 0){
                PlayerPrefs.SetFloat("MaxAir", 100);
            }
            else if(PlayerPrefs.GetInt("ChangedUnderWaterSystem") == 1){
                PlayerPrefs.SetFloat("MaxAir", 300);
            }
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
            yield return new WaitForSeconds(0.2f);
        }
    }
}
