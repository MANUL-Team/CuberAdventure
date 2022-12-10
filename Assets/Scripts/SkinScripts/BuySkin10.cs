using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuySkin10 : MonoBehaviour
{
    [SerializeField] private GameObject Buy, Use;


    public void Pressed(){
        if(PlayerPrefs.GetInt("Coins", Money.Coin) >= 30){
            PlayerPrefs.SetInt("Coins", PlayerPrefs.GetInt("Coins", Money.Coin) - 30);
            Buy.SetActive(false);
            Use.SetActive(true);
            PlayerPrefs.SetInt("Skin10", 1);
        }
    }

    void Update()
    {
        if(PlayerPrefs.GetInt("Skin10") > 0){
            Buy.SetActive(false);
            Use.SetActive(true);
        }
    }
}
