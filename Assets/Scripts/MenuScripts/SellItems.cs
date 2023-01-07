using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SellItems : MonoBehaviour
{
    [SerializeField] private int id;
    [SerializeField] private string type;
    [SerializeField] private int price;
    [SerializeField] private GameObject notEnough, scaleCount;
    [SerializeField] private Slider slider;
    [SerializeField] private Text textSlider, textPrice;
    [SerializeField] private SellSomeItems button;
    public void Sell(){
        if(PlayerPrefs.GetInt("Item" + type + id) > 0){
            scaleCount.SetActive(true);
            slider.maxValue = PlayerPrefs.GetInt("Item" + type + id);
            button.price = price;
            button.id = id;
            button.type = type;
        }
        else{
            notEnough.SetActive(true);
        }
    }
    private void Update() {
        textSlider.text = Mathf.RoundToInt(slider.value).ToString() + "/" + slider.maxValue.ToString();
        textPrice.text = (Convert.ToInt32(Mathf.RoundToInt(slider.value)) * button.price).ToString();
    }
}
