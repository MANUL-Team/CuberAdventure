using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuySkin : MonoBehaviour
{
    [SerializeField] private Animator iconAnim;
    [SerializeField] private GameObject[] skins;
    [SerializeField] private GameObject panel, sorryMessage, cantMove;
    [SerializeField] private int id;
    private bool buying;

    private void BuyingSkin(){
            id = Random.Range(0, 15);
            if(PlayerPrefs.GetInt("Skin" + (id+1).ToString()) == 1){
                BuyingSkin();
            }else{
                PlayerPrefs.SetInt("Skin" + (id+1).ToString(), 1);
                Invoke("ViewSkin", 4f);
            }
        
    }
    public void Buy(){
        if(PlayerPrefs.GetInt("Diamonds") >= 100 && buying == false){
            bool sorry = true;
            for(int i = 1; i < 15; i++){
                if(PlayerPrefs.GetInt("Skin" + i.ToString()) != 1){
                    sorry = false;
                }
            }
            if(sorry == true){
                sorryMessage.SetActive(true);
            }
            else{
                buying = true;
                PlayerPrefs.SetInt("Diamonds", PlayerPrefs.GetInt("Diamonds") - 100);
                iconAnim.SetBool("Buying", true);
                cantMove.SetActive(true);
                BuyingSkin();
            }
        }
    }
    private void ViewSkin(){
        buying = false;
        panel.SetActive(true);
        skins[id].SetActive(true);
        iconAnim.SetBool("Buying", false);
        cantMove.SetActive(false);
    }
    private void UnviewSkin(){
        buying = false;
        sorryMessage.SetActive(false);
        panel.SetActive(false);
        skins[id].SetActive(false);
    }
}
