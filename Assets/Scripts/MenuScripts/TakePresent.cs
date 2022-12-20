using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TakePresent : MonoBehaviour
{
    [SerializeField] private GameObject have, havent, panel;
    public void Present(){
        PlayerPrefs.SetString("CollectTimePresent", DateTime.Now.ToString());
        have.SetActive(false);
        havent.SetActive(true);
        PlayerPrefs.SetInt("Diamonds", PlayerPrefs.GetInt("Diamonds") + 50);
        panel.SetActive(true);
        panel.GetComponent<Animator>().SetBool("DropPresent", true);
    }
    public void Skins(){
        panel.SetActive(false);
    }
}
