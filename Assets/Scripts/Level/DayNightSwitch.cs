using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class DayNightSwitch : MonoBehaviour
{
    private Light2D light;
    private float intensity;
    private float floatTime;
    private bool toDay;
    private void Start() {
        toDay = IntToBool(PlayerPrefs.GetInt("ToDay"));
        light = GetComponent<Light2D>();
        StartCoroutine(TimeController());
    }
    private void Update() {
        light.intensity = intensity;
    }
    private int BoolToInt(bool variable){
        if(variable != false){
            return 1;
        }
        else{
            return 0;
        }
    }
    private bool IntToBool(int variable){
        if(variable != 0){
            return true;
        }
        else{
            return false;
        }
    }
    private IEnumerator TimeController(){
        while(true){
            if(PlayerPrefs.GetInt("Time") >= 1000){
                toDay = false;
            }
            else if(PlayerPrefs.GetInt("Time") <= 10){
                toDay = true;
            }
            if(toDay){
                PlayerPrefs.SetInt("Time", PlayerPrefs.GetInt("Time") + 1);
            }
            else if(!toDay){
                PlayerPrefs.SetInt("Time", PlayerPrefs.GetInt("Time") - 1);
            }
            floatTime = PlayerPrefs.GetInt("Time");
            intensity = floatTime/1000;
            PlayerPrefs.SetInt("ToDay", BoolToInt(toDay));
            yield return new WaitForSeconds(0.1f);
        }
    }
}
