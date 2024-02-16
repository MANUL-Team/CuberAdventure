using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PresentTimeReset : MonoBehaviour
{
    [SerializeField] private GameObject have, havent;
    [SerializeField] private int delay;
    void Start()
    {
        CheckPresent();
    }
    public void CheckPresent()
    {
        DateTime nowTime = DateTime.Now;
        if(PlayerPrefs.GetString("CollectTimePresent") != ""){
            if(nowTime > DateTime.Parse(PlayerPrefs.GetString("CollectTimePresent")).AddHours(delay)){
                have.SetActive(true);
                havent.SetActive(false);
            }
            else{
                have.SetActive(false);
                havent.SetActive(true);
            }
        }
        else{
            have.SetActive(true);
            havent.SetActive(false);
        }
    }
}
