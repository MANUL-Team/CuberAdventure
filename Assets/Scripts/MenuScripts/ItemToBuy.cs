using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemToBuy : MonoBehaviour
{
    [SerializeField] private int id;
    [SerializeField] private int money;
    [SerializeField] private GameObject button, buyed, notEnough;
    [SerializeField] private string whatIsIt;

    public void Buy(){
        if(PlayerPrefs.GetInt("Coins") >= money){
            PlayerPrefs.SetInt(whatIsIt + id.ToString(), 1);
            PlayerPrefs.SetInt("Coins", PlayerPrefs.GetInt("Coins") - money);
        }
        else{
            notEnough.SetActive(true);
        }
    }
    private void Update() {
        if(PlayerPrefs.GetInt(whatIsIt + id.ToString()) == 1){
            button.SetActive(false);
            buyed.SetActive(true);
        }
    }
}
