using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SellSomeItems : MonoBehaviour
{
    public int id;
    public int price;
    public Slider slider;
    public GameObject scaleCount;

    public void Sell(){
        PlayerPrefs.SetInt("Item" + " " + id, PlayerPrefs.GetInt("Item" + " "+ id) - Mathf.RoundToInt(slider.value));
        PlayerPrefs.SetInt("Coins", PlayerPrefs.GetInt("Coins") + price * Mathf.RoundToInt(slider.value));
        scaleCount.SetActive(false);
    }
}
