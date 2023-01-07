using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwitchPoisionButtons : MonoBehaviour
{
    [SerializeField] private GameObject[] buttons;
    [SerializeField] private GameObject healEffect, efMenu;
    public DateTime collectTime;
    public void PoisionSwitch() {
        for(int i = 0; i < buttons.Length; i++){
            if(i == PlayerPrefs.GetInt("SelectedPoision") && PlayerPrefs.GetInt("ItemDefault" + buttons[i].GetComponent<Poision>().itemId) > 0){
                buttons[i].SetActive(true);
            }
            else{
                buttons[i].SetActive(false);
            }
        }
    }
    private void Start() {
        PoisionSwitch();
    }
    private void FixedUpdate() {
        PoisionSwitch();
        SwitchEffect();
    }
    public void SwitchEffect(){
        if(PlayerPrefs.GetInt("HealMnojitel") == 5){
            efMenu.SetActive(true);
            healEffect.SetActive(true);
        }
        else{
            efMenu.SetActive(false);
            healEffect.SetActive(false);
        }
    }
    public void RegenerationPoision(int itemId, int poisionId){
        collectTime = DateTime.Now;
        PlayerPrefs.SetString("CollectTimePoision" + poisionId, collectTime.ToString());
        PlayerPrefs.SetInt("HealMnojitel", 5);
        PlayerPrefs.SetInt("ItemDefault" + itemId, PlayerPrefs.GetInt("ItemDefault" + itemId) - 1);
    }
    private void RegenerationPoisionEnd(){
        PlayerPrefs.SetInt("HealMnojitel", 1);
    }
    public IEnumerator PoisionEnd(int poisionId, int delay){
        while(true){
            DateTime nowTime = DateTime.Now;
            if(PlayerPrefs.GetString("CollectTimePoision" + poisionId) != ""){
                if(nowTime > DateTime.Parse(PlayerPrefs.GetString("CollectTimePoision" + poisionId)).AddSeconds(delay)){
                    RegenerationPoisionEnd();
                }
            }
            else{
                RegenerationPoisionEnd();
            }
            yield return new WaitForSeconds(1f);
        }
    }
}
