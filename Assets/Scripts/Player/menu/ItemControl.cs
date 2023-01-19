using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemControl : MonoBehaviour
{

    [SerializeField] private Transform itemsFolder;
    [SerializeField] private List<Item> itemsList = new List<Item>();
    void Start()
    {
        for(int i = 0; i < itemsFolder.childCount; i++){
            itemsList.Add(itemsFolder.GetChild(i).GetComponent<Item>());
        }
    }
    public void CloseDescriptions(){
        for(int i = 0; i < itemsList.Count; i++){
            itemsList[i].description.SetActive(false);
        }
    }
    IEnumerator ItemsUpdate(){
        while(true){
            for(int i = 0; i < itemsList.Count; i++){
                if(PlayerPrefs.GetInt("Item" + " " + itemsList[i].id) == 0){
                    itemsList[i].gameObject.SetActive(false);
                }
                else{
                    itemsList[i].gameObject.SetActive(true);
                }
            }
            yield return new WaitForSeconds(0.1f);
        }
    }
    private void OnEnable() {
        StartCoroutine(ItemsUpdate());
    }
}
