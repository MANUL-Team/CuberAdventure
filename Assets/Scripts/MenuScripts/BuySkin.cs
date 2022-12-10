using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuySkin : MonoBehaviour
{
    [SerializeField] private Animator iconAnim;
    [SerializeField] private GameObject[] skins;
    [SerializeField] private GameObject panel, sorryMessage;
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
            if(PlayerPrefs.GetInt("Skin1") == 1 && PlayerPrefs.GetInt("Skin2") == 1 && PlayerPrefs.GetInt("Skin3") == 1 && PlayerPrefs.GetInt("Skin4") == 1 && PlayerPrefs.GetInt("Skin5") == 1 && PlayerPrefs.GetInt("Skin6") == 1 && PlayerPrefs.GetInt("Skin7") == 1 && PlayerPrefs.GetInt("Skin8") == 1 && PlayerPrefs.GetInt("Skin9") == 1 && PlayerPrefs.GetInt("Skin10") == 1 && PlayerPrefs.GetInt("Skin11") == 1 && PlayerPrefs.GetInt("Skin12") == 1 && PlayerPrefs.GetInt("Skin13") == 1 && PlayerPrefs.GetInt("Skin14") == 1 && PlayerPrefs.GetInt("Skin15") == 1 ){
                sorryMessage.SetActive(true);
            }
            else{
                buying = true;
                PlayerPrefs.SetInt("Diamonds", PlayerPrefs.GetInt("Diamonds") - 100);
                iconAnim.SetBool("Buying", true);
                BuyingSkin();
            }
        }
    }
    private void ViewSkin(){
        buying = false;
        panel.SetActive(true);
        skins[id].SetActive(true);
        iconAnim.SetBool("Buying", false);
    }
    private void UnviewSkin(){
        buying = false;
        sorryMessage.SetActive(false);
        panel.SetActive(false);
        skins[id].SetActive(false);
    }
}
