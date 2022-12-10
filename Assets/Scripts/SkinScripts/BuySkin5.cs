using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuySkin5 : MonoBehaviour
{
    [SerializeField] private GameObject Buy, Use;


    public void Pressed(){
        if(PlayerPrefs.GetInt("Coins", Money.Coin) >= 20){
            PlayerPrefs.SetInt("Coins", PlayerPrefs.GetInt("Coins", Money.Coin) - 20);
            Buy.SetActive(false);
            Use.SetActive(true);
            PlayerPrefs.SetInt("Skin5", 1);
        }
    }

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(PlayerPrefs.GetInt("Skin5") > 0){
            Buy.SetActive(false);
            Use.SetActive(true);
        }
    }
}
