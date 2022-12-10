using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpenItemShop : MonoBehaviour
{
    [SerializeField] private GameObject itemShop;
    public void OpenItemShopVoid(){
        itemShop.SetActive(true);
    }
    public void CloseItemShop(){
        itemShop.SetActive(false);
    }

}
