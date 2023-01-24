using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BuyItem : MonoBehaviour
{
    public int id;
    public int price;
    public GameObject description;
    [SerializeField] private Text textPrice;
    void Start(){
        textPrice.text = price.ToString();
    }
}
