using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Random = System.Random;

public class TakePresent : MonoBehaviour
{
    [SerializeField] private GameObject have, havent, panel;
    [SerializeField] private Text presentText;
    public void Present(){
        PlayerPrefs.SetString("CollectTimePresent", DateTime.Now.ToString());
        have.SetActive(false);
        havent.SetActive(true);
        GetRandomPresent();
        panel.SetActive(true);
        panel.GetComponent<Animator>().SetBool("DropPresent", true);
    }
    public void Skins(){
        panel.SetActive(false);
    }

    private void GetRandomPresent()
    {
        int lang = PlayerPrefs.GetInt("Language");
        Random random = new Random();
        int randNum = random.Next(5);
        int diamonds = 0;
        switch (randNum)
        {
            case 0:
                diamonds = 50;
                break;
            case 1:
                diamonds = 50;
                break;
            case 2:
                diamonds = 50;
                break;
            case 3:
                diamonds = 100;
                break;
            case 4:
                diamonds = 100;
                break;
            case 5:
                diamonds = 200;
                break;
        }
        PlayerPrefs.SetInt("Diamonds", PlayerPrefs.GetInt("Diamonds") + diamonds);
        switch (lang)
        {
            case 0:
                presentText.text = $"+{diamonds} алмазов!";
                break;
            default:
                presentText.text = $"+{diamonds} diamonds!";
                break;
        }
    }
}
