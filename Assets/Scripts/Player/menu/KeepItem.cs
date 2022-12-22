using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeepItem : MonoBehaviour
{
    [Header("Item Properties")]
    public string type;
    public int id, num;
    [Header("Buttons")]
    [SerializeField] private GameObject button;
    public int delay;
    public string DisplayTime;
    public DateTime collectTime;
    private void OnTriggerEnter2D(Collider2D other) {
        if(other.CompareTag("Player")){
            button.SetActive(true);
        }
    }
    private void OnTriggerExit2D(Collider2D other) {
        if(other.CompareTag("Player")){
            button.SetActive(false);
        }
    }
    public void KeepItemVoid(){
        collectTime = DateTime.Now;
        PlayerPrefs.SetInt("Item" + type + id, PlayerPrefs.GetInt("Item" + type + id) + 1);
        PlayerPrefs.SetString("CollectTimeItem"+ type + id + num, collectTime.ToString());
        DisplayTime = PlayerPrefs.GetString("CollectTimeItem"+ type + id + num);
        gameObject.SetActive(false);
    }
}
