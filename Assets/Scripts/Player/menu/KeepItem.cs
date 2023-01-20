using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class KeepItem : MonoBehaviour
{
    [Header("Item Properties")]
    public int id;
    [HideInInspector]
    public int num;
    [HideInInspector]
    public int loc;
    [Header("Buttons")]
    [SerializeField] private GameObject button;
    public int delay;
    public string DisplayTime;
    public DateTime collectTime;
    void Awake()
    {
        loc = SceneManager.GetActiveScene().buildIndex;
    }
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
        PlayerPrefs.SetInt("Item" + " " + id, PlayerPrefs.GetInt("Item" + " " + id) + 1);
        PlayerPrefs.SetString("CollectTimeItem" + " " + loc + " " + id + " " + num, collectTime.ToString());
        DisplayTime = PlayerPrefs.GetString("CollectTimeItem" + " " + loc + " " + id + " " + num);
        gameObject.SetActive(false);
    }
}
