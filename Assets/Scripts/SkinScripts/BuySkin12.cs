using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuySkin12 : MonoBehaviour
{
    [SerializeField] private GameObject Buy, Use;


    public void Pressed(){
        if(PlayerPrefs.GetInt("Coins", Money.Coin) >= 40){
            PlayerPrefs.SetInt("Coins", PlayerPrefs.GetInt("Coins", Money.Coin) - 40);
            Buy.SetActive(false);
            Use.SetActive(true);
            PlayerPrefs.SetInt("Skin12", 1);
        }
    }

    void Update()
    {
        if(PlayerPrefs.GetInt("Skin12") > 0){
            Buy.SetActive(false);
            Use.SetActive(true);
        }
    }
}
