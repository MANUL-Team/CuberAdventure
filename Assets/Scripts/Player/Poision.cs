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

    public void UsePoision(){
        poision.RegenerationPoision(itemId, poisionId);
    }
    private void Start() {
        poision.StartPoisionEndCoroutine(poisionId, delay);
    }
}
