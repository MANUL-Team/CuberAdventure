using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Poision : MonoBehaviour
{
    public int itemId;
    public int poisionId;
    [SerializeField] private int delay;
    [SerializeField] private SwitchPoisionButtons poision;
    [SerializeField] private string typeOfPoision;

    public void UsePoision(){
        if(typeOfPoision == "Regeneration"){
            poision.RegenerationPoision(itemId, poisionId, delay);
        }
        else if(typeOfPoision == "UnderWater"){
            poision.UnderWaterPoision(itemId, poisionId, delay);
        }
        else if(typeOfPoision == "Light"){
            poision.LightPoision(itemId, poisionId, delay);
        }
    }
    private void Start() {
        DateTime nowTime = DateTime.Now;
        if(PlayerPrefs.GetString("CollectTimePoision" + poisionId) != ""){
            if(nowTime < DateTime.Parse(PlayerPrefs.GetString("CollectTimePoision" + poisionId)).AddMinutes(delay)){
                poision.StartPoisionEndCoroutine(poisionId, delay, typeOfPoision);
            }
            else{
                poision.PoisionEnd(typeOfPoision);
            }
        }
    }
}