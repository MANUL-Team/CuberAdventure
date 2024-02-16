using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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
    [SerializeField] private Text name, description, stats, comment;
    [SerializeField] private GameObject descriptionObj;
    public void SelectCraftingItem(int id){
        stack = itemsToCraft[id].stack;
        if(stack){
            craftingItem = itemsToCraft[id].item;
        }
        else{
            craftingModule = itemsToCraft[id].module;
        }
        AddItems(id);
        SetDescription(id);
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
                descriptionObj.SetActive(false);
            }
            for(int i = 0; i < forCraftCounts.Count; i++){
                PlayerPrefs.SetInt("Item" + " " + forCraftItems[i].id.ToString(), PlayerPrefs.GetInt("Item" + " " + forCraftItems[i].id.ToString()) - forCraftCounts[i]);
            }
            successful.SetActive(true);
            ClearItems();
        }
        else{
            notEnough.SetActive(true);
        }
    }
    void FixedUpdate()
    {
        for(int i = 0; i < itemsToCraft.Length; i++){
            if(itemsToCraft[i].module != null){
                if(PlayerPrefs.GetInt(itemsToCraft[i].module.module + itemsToCraft[i].module.id.ToString()) != 0){
                    itemsToCraft[i].gameObject.SetActive(false);
                }
                else{
                    itemsToCraft[i].gameObject.SetActive(true);
                }
            }
        }
    }
    public void ClearItems(){
        itemsDisplay.SetActive(false);
        craftButton.SetActive(false);
        closeButton.SetActive(true);
        for(int i = 0; i < items.Length; i++){
            items[i].gameObject.SetActive(false);
        }
        descriptionObj.SetActive(false);
        forCraftCounts.Clear();
        forCraftItems.Clear();
    }
    public void AddItems(int id){
        ClearItems();
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
        itemsDisplay.SetActive(true);
        craftButton.SetActive(true);
        closeButton.SetActive(false);
    }
    public void SetDescription(int id){
        descriptionObj.SetActive(true);
        int lang = PlayerPrefs.GetInt("Language");
        if(!stack){
            switch (lang)
            {
                case 0:
                    name.text = itemsToCraft[id].module.nameRu;
                    description.text = itemsToCraft[id].module.descriptionRu;
                    comment.text = itemsToCraft[id].module.commentRu;
                    stats.text = itemsToCraft[id].module.statsRu;
                    break;
                default:
                    name.text = itemsToCraft[id].module.nameEng;
                    description.text = itemsToCraft[id].module.descriptionEng;
                    comment.text = itemsToCraft[id].module.commentEng;
                    stats.text = itemsToCraft[id].module.statsEng;
                    break;
            }
        }
        else{
            switch (lang)
            {
                case 0:
                    name.text = itemsToCraft[id].item.nameRu;
                    description.text = itemsToCraft[id].item.descriptionRu;
                    comment.text = itemsToCraft[id].item.commentRu;
                    stats.text = itemsToCraft[id].item.statsRu;
                    break;
                default:
                    name.text = itemsToCraft[id].item.nameEng;
                    description.text = itemsToCraft[id].item.descriptionEng;
                    comment.text = itemsToCraft[id].item.commentEng;
                    stats.text = itemsToCraft[id].item.statsEng;
                    break;
            }
        }
    }

}
