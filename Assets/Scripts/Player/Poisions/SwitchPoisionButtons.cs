using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SwitchPoisionButtons : MonoBehaviour
{
    [SerializeField] private GameObject[] buttons;
    [SerializeField] private GameObject efMenu;
    [SerializeField] private GameObject[] effects;
    [SerializeField] private Text[] effectsTimeOuts;
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
            effects[0].SetActive(true);
        }
        else{
            effects[0].SetActive(false);
        }
        if(PlayerPrefs.GetInt("CanLoseAir") == 0){
            effects[1].SetActive(true);
        }
        else{
            effects[1].SetActive(false);
        }
        if(PlayerPrefs.GetInt("LightPoision") == 1){
            effects[2].SetActive(true);
        }
        else{
            effects[2].SetActive(false);
        }
    }
    public void RegenerationPoision(int itemId, int poisionId, int delay){
        collectTime = DateTime.Now;
        PlayerPrefs.SetString("CollectTimePoision" + poisionId, collectTime.ToString());
        PlayerPrefs.SetInt("HealMnojitel", 5);
        PlayerPrefs.SetInt("ItemDefault" + itemId, PlayerPrefs.GetInt("ItemDefault" + itemId) - 1);
        StartPoisionEndCoroutine(poisionId, delay, "Regeneration");
    }
    public void UnderWaterPoision(int itemId, int poisionId, int delay){
        collectTime = DateTime.Now;
        PlayerPrefs.SetString("CollectTimePoision" + poisionId, collectTime.ToString());
        PlayerPrefs.SetInt("CanLoseAir", 0);
        PlayerPrefs.SetInt("ItemDefault" + itemId, PlayerPrefs.GetInt("ItemDefault" + itemId) - 1);
        StartPoisionEndCoroutine(poisionId, delay, "UnderWater");
    }
    public void LightPoision(int itemId, int poisionId, int delay){
        collectTime = DateTime.Now;
        PlayerPrefs.SetString("CollectTimePoision" + poisionId, collectTime.ToString());
        PlayerPrefs.SetInt("LightPoision", 1);
        PlayerPrefs.SetInt("ItemDefault" + itemId, PlayerPrefs.GetInt("ItemDefault" + itemId) - 1);
        StartPoisionEndCoroutine(poisionId, delay, "Light");
    }
    public void PoisionEnd(string typeOfPoision){
        if(typeOfPoision == "Regeneration"){
            PlayerPrefs.SetInt("HealMnojitel", 1);
            StopCoroutine("PoisionEndCoroutine");
        }
        else if(typeOfPoision == "UnderWater"){
            PlayerPrefs.SetInt("CanLoseAir", 1);
            StopCoroutine("PoisionEndCoroutine");
        }
        else if(typeOfPoision == "Light"){
            PlayerPrefs.SetInt("LightPoision", 0);
            StopCoroutine("PoisionEndCoroutine");
        }
    }
    public void StartPoisionEndCoroutine(int poisionId, int delay, string typeOfPoision){
        StartCoroutine(PoisionEndCoroutine(poisionId, delay, typeOfPoision));
    }
    private IEnumerator PoisionEndCoroutine(int poisionId, int delay, string typeOfPoision){
        while(true){
            DateTime nowTime = DateTime.Now;
            if(typeOfPoision == "Regeneration"){
                effectsTimeOuts[0].text = (DateTime.Parse(PlayerPrefs.GetString("CollectTimePoision" + 1)).AddMinutes(3) - nowTime).Minutes.ToString() + ":" + (DateTime.Parse(PlayerPrefs.GetString("CollectTimePoision" + 1)).AddMinutes(3) - nowTime).Seconds.ToString();
            }
            else if(typeOfPoision == "UnderWater"){
                effectsTimeOuts[1].text = (DateTime.Parse(PlayerPrefs.GetString("CollectTimePoision" + 2)).AddMinutes(1) - nowTime).Minutes.ToString() + ":" + (DateTime.Parse(PlayerPrefs.GetString("CollectTimePoision" + 2)).AddMinutes(1) - nowTime).Seconds.ToString();
            }
            else if(typeOfPoision == "Light"){
                effectsTimeOuts[2].text = (DateTime.Parse(PlayerPrefs.GetString("CollectTimePoision" + 3)).AddMinutes(3) - nowTime).Minutes.ToString() + ":" + (DateTime.Parse(PlayerPrefs.GetString("CollectTimePoision" + 3)).AddMinutes(3) - nowTime).Seconds.ToString();
            }
            if(PlayerPrefs.GetString("CollectTimePoision" + poisionId) != ""){
                if(nowTime > DateTime.Parse(PlayerPrefs.GetString("CollectTimePoision" + poisionId)).AddMinutes(delay)){
                    PoisionEnd(typeOfPoision);
                    break;
                }
            }
            else{
                PoisionEnd(typeOfPoision);
                break;
            }
            Debug.Log("1sec");
            yield return new WaitForSeconds(1f);
        }
    }
}
