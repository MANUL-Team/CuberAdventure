using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CraftngMenuController : MonoBehaviour
{
    [SerializeField] private GameObject[] descriptions;
    [SerializeField] private GameObject itemsDisplay, craftButton;
    [SerializeField] private NeedToCraft[] itemsToCraft;
    [SerializeField] private ForCraftItem[] items;
    [SerializeField] private List<Drop> forCraftItems = new List<Drop>();
    [SerializeField] private List<int> forCraftCounts = new List<int>();
    [SerializeField] private bool canCraft;
    [SerializeField] private GameObject successful, notEnough;
    private bool stack;
    private CraftedItem craftingItem;
    public void SelectCraftingItem(int id){
        craftingItem = itemsToCraft[id].item;
        stack = itemsToCraft[id].stack;
        for(int i = 0; i < descriptions.Length; i++){
            if(i != id){
                descriptions[i].SetActive(false);
            }
            else{
                descriptions[i].SetActive(true);
            }
        }
        itemsDisplay.SetActive(true);
        craftButton.SetActive(true);
        for(int i = 0; i < items.Length; i++){
            items[i].gameObject.SetActive(false);
        }
        for(int i = 0; i < forCraftItems.Count; i++){
            forCraftCounts.RemoveAt(0);
            forCraftItems.RemoveAt(0);
        }

        for(int b = 0; b < items.Length; b++){
            for(int i = 0; i < itemsToCraft[id].needToCraft.Length; i++){
                if(items[b].item.type == itemsToCraft[id].needToCraft[i].type && items[b].item.id == itemsToCraft[id].needToCraft[i].id){
                    items[b].gameObject.SetActive(true);
                    items[b].count = itemsToCraft[id].count[i];
                    forCraftItems.Add(items[b].item);
                    forCraftCounts.Add(items[b].count);
                }
            }
        }
    }
    public void Craft(){
        canCraft = true;
        for(int i = 0; i < forCraftItems.Count; i++){
            if(!(PlayerPrefs.GetInt("Item" + forCraftItems[i].type.ToString() + forCraftItems[i].id.ToString()) >= forCraftCounts[i])){
                canCraft = false;
            }
        }
        if(canCraft){
            if(stack){
                PlayerPrefs.SetInt(craftingItem.WhatIsIt + craftingItem.id.ToString(), PlayerPrefs.GetInt(craftingItem.WhatIsIt + craftingItem.id.ToString()) + 1);
            }
            else{
                PlayerPrefs.SetInt(craftingItem.WhatIsIt + craftingItem.id.ToString(), 1);
            }
            for(int i = 0; i < forCraftCounts.Count; i++){
                PlayerPrefs.SetInt("Item" + forCraftItems[i].type.ToString() + forCraftItems[i].id.ToString(), PlayerPrefs.GetInt("Item" + forCraftItems[i].type.ToString() + forCraftItems[i].id.ToString()) - forCraftCounts[i]);
            }
            successful.SetActive(true);
        }
        else{
            notEnough.SetActive(true);
        }
    }
    void FixedUpdate()
    {
        for(int i = 0; i < itemsToCraft.Length; i++){
            if(PlayerPrefs.GetInt(itemsToCraft[i].item.WhatIsIt + itemsToCraft[i].item.id.ToString()) != 0 && !itemsToCraft[i].stack){
                itemsToCraft[i].gameObject.SetActive(false);
            }
            else{
                itemsToCraft[i].gameObject.SetActive(true);
            }
        }
    }

}
