using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemControl : MonoBehaviour
{

    [SerializeField] private Transform itemsFolder;
    [SerializeField] private List<Item> itemsList = new List<Item>();
    private string nowSorting;
    [SerializeField] private GameObject mainDescription;
    public Text name, description;
    [SerializeField] private GameObject[] potionDesc;
    void Start()
    {
        for(int i = 0; i < itemsFolder.childCount; i++){
            itemsList.Add(itemsFolder.GetChild(i).GetComponent<Item>());
        }
        nowSorting = "All";
    }
    public void CloseDescriptions(){
        mainDescription.SetActive(false);
    }
    public void OpenDescription(){
        mainDescription.SetActive(true);
    }
    IEnumerator ItemsUpdate(){
        while(true){
            for(int i = 0; i < itemsList.Count; i++){
                if(nowSorting != "All"){
                    if(PlayerPrefs.GetInt("Item" + " " + itemsList[i].item.id) != 0 && itemsList[i].sorting == nowSorting){
                        itemsList[i].gameObject.SetActive(true);
                    }
                    else{
                        itemsList[i].gameObject.SetActive(false);
                    }
                }
                else{
                    if(PlayerPrefs.GetInt("Item" + " " + itemsList[i].item.id) != 0){
                        itemsList[i].gameObject.SetActive(true);
                    }
                    else{
                        itemsList[i].gameObject.SetActive(false);
                    }
                }
            }
            yield return new WaitForSeconds(0.1f);
        }
    }
    public void SortingItems(string sorting){
        nowSorting = sorting;
    }
    private void OnEnable() {
        StartCoroutine(ItemsUpdate());
    }
    public void ClosePotDesc(){
        for(int i = 0; i < potionDesc.Length; i++){
            potionDesc[i].SetActive(false);
        }
    }
}
