using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NeedToCraft : MonoBehaviour
{
    public Drop[] needToCraft;
    public int[] count;
    public CraftedItem module;
    public Drop item;
    public bool stack;
    public Image icon;
    private void Start() {
        icon = transform.GetChild(0).GetComponent<Image>();
        if(!stack){
            icon.sprite = module.icon;
        }
        else{
            icon.sprite = item.icon;
        }
    }
}