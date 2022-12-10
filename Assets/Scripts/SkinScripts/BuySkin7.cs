using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuySkin7 : MonoBehaviour
{
    [SerializeField] private GameObject Buy, Use;


    public void Pressed(){
        if(PlayerPrefs.GetInt("Coins", Money.Coin) >= 30){
            PlayerPrefs.SetInt("Coins", PlayerPrefs.GetInt("Coins", Money.Coin) - 30);
            Buy.SetActive(false);
            Use.SetActive(true);
            PlayerPrefs.SetInt("Skin7", 1);
        }
    }

    void Start()
    {
        
    }
    private void Update() {
        if(PlayerPrefs.GetInt("Skin7") > 0){
            Buy.SetActive(false);
            Use.SetActive(true);
        }
    }
}
