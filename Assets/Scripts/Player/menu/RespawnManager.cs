using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RespawnManager : MonoBehaviour
{
    [SerializeField] private KeepItem[] items;
    [SerializeField] private MobController[] mobs;

    private void Start() {
        //StartCoroutine("CheckRespawns");
        CheckItems();
        CheckMobs();
    }
    private void CheckItems(){
        DateTime nowTime = DateTime.Now;
        for(int i = 0; i < items.Length; i++){
            if(PlayerPrefs.GetString("CollectTimeItem" + items[i].type + items[i].id + items[i].num) != ""){
                if(nowTime > DateTime.Parse(PlayerPrefs.GetString("CollectTimeItem" + items[i].type + items[i].id + items[i].num)).AddMinutes(items[i].delay)){
                    items[i].gameObject.SetActive(true);
                }
                else{
                    items[i].gameObject.SetActive(false);
                }
            }
            else{
                items[i].gameObject.SetActive(true);
            }
        }
    }
    private void CheckMobs(){
        DateTime nowTime = DateTime.Now;
        for(int i = 0; i < mobs.Length; i++){
            if(PlayerPrefs.GetString("MobTimeDie" + mobs[i].id + mobs[i].num) != ""){
                if(nowTime > DateTime.Parse(PlayerPrefs.GetString("MobTimeDie"  + mobs[i].id + mobs[i].num)).AddMinutes(mobs[i].delay) && mobs[i].gameObject.activeSelf == false){
                    mobs[i].hp = mobs[i].maxHp;
                    mobs[i].gameObject.SetActive(true);
                    mobs[i].bars.SetActive(true);
                }
            }
        }
    }
    private IEnumerator CheckRespawns() {
        while(true){
            CheckItems();
            CheckMobs();
            yield return new WaitForSeconds(10f);
        }
    }
}
