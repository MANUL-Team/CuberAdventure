using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MoneyMenu : MonoBehaviour
{
    public static int Coin;
    Text text;
    void Start()
    {
        text = GetComponent<Text>();
        
    }
    void Update()
    {
        Coin = PlayerPrefs.GetInt("Coins", Coin);
        text.text = Coin.ToString();
    }
}
