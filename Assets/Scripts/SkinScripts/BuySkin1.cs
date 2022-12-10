using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuySkin1 : MonoBehaviour
{

    [SerializeField] private GameObject Buy, Use;


    public void Pressed(){
        if(PlayerPrefs.GetInt("Coins", Money.Coin) >= 10){
            PlayerPrefs.SetInt("Coins", PlayerPrefs.GetInt("Coins", Money.Coin) - 10);
            Buy.SetActive(false);
            Use.SetActive(true);
            PlayerPrefs.SetInt("Skin1", 1);
        }
    }

    void Start()
    {
        
    }


    void Update()
    {
        if(PlayerPrefs.GetInt("Skin1") > 0){
            Buy.SetActive(false);
            Use.SetActive(true);
        }
    }
}
