using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DayNightSwitch : MonoBehaviour
{
    private Animator anim;

    private void Start() {
        anim = GetComponent<Animator>();
        if(PlayerPrefs.GetInt("Day") == 1){
            DayCycle();
        } else {
            ToDay();
        }
    }

    public void ToDay(){
        anim.SetBool("SunUp", true);
    }
    public void ToNight(){
        anim.SetBool("SunUp", false);
    }
    public void DayCycle(){
        anim.SetBool("DayCycle", true);
    }
    public void SetNight(){
        PlayerPrefs.SetInt("Day", 0);
    }
    public void SetDay(){
        PlayerPrefs.SetInt("Day", 1);
    }

}
