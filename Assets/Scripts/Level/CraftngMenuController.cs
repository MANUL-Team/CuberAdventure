using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CraftngMenuController : MonoBehaviour
{
    [Header("Buttons")]
    [SerializeField] private GameObject itemsDisplay;
    [SerializeField] private GameObject craftButton;
    [SerializeField] private GameObject closeButton;
    [Header("Panels")]
    [SerializeField] private GameObject successful;
    [SerializeField] private GameObject notEnough;
    [Header("Arrays")]
    [SerializeField] private GameObject[] descriptions;
    [SerializeField] private NeedToCraft[] itemsToCraft;
    [SerializeField] private ForCraftItem[] items;
    [Header("Lists with about craft info")]
    [SerializeField] private List<Drop> forCraftItems = new List<Drop>();
    [SerializeField] private List<int> forCraftCounts = new List<int>();
    [SerializeField] private bool isItem;
    private bool canCraft;
    private bool stack;
    private CraftedItem craftingModule;
    private Drop craftingItem;
    public void SelectCraftingItem(int id){
        stack = itemsToCraft[id].stack;
        if(stack){
            craftingItem = itemsToCraft[id].item;
        }
        else{
            craftingModule = itemsToCraft[id].module;
        }
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
        closeButton.SetActive(false);
        for(int i = 0; i < items.Length; i++){
            items[i].gameObject.SetActive(false);
        }
        forCraftCounts.Clear();
        forCraftItems.Clear();

        for(int b = 0; b < items.Length; b++){
            for(int i = 0; i < itemsToCraft[id].needToCraft.Length; i++){
                if(items[b].item.id == itemsToCraft[id].needToCraft[i].id){
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
            if(!(PlayerPrefs.GetInt("Item" + " " + forCraftItems[i].id.ToString()) >= forCraftCounts[i])){
                canCraft = false;
            }
        }
        if(canCraft){
            if(stack){
                PlayerPrefs.SetInt("Item" + " " + craftingItem.id.ToString(), PlayerPrefs.GetInt("Item" + " " + craftingItem.id.ToString()) + 1);
            }
            else{
                PlayerPrefs.SetInt(craftingModule.module + craftingModule.id.ToString(), 1);
            }
            for(int i = 0; i < forCraftCounts.Count; i++){
                PlayerPrefs.SetInt("Item" + " " + forCraftItems[i].id.ToString(), PlayerPrefs.GetInt("Item" + " " + forCraftItems[i].id.ToString()) - forCraftCounts[i]);
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
            if(!itemsToCraft[i].module != null){
                if(PlayerPrefs.GetInt(itemsToCraft[i].module.module + itemsToCraft[i].module.id.ToString()) != 0){
                    itemsToCraft[i].gameObject.SetActive(false);
                }
                else{
                    itemsToCraft[i].gameObject.SetActive(true);
                }
            }
        }
    }

}
