using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SellItems : MonoBehaviour
{
    [SerializeField] private int id;
    [SerializeField] private string type;
    [SerializeField] private int price;
    [SerializeField] private GameObject notEnough;
    public void Sell(){
        if(PlayerPrefs.GetInt("Item" + type + id) > 0){
            PlayerPrefs.SetInt("Item" + type + id, PlayerPrefs.GetInt("Item" + type + id) - 1);
            PlayerPrefs.SetInt("Coins", PlayerPrefs.GetInt("Coins") + price);
        } else{
            notEnough.SetActive(true);
        }
    }
}
