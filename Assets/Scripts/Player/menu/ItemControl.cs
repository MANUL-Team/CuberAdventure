using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemControl : MonoBehaviour
{
    [SerializeField] private Item[] items;

    public void CloseDescriprions(){
        for(int i = 0; i < items.Length; i++){
            items[i].description.SetActive(false);
        }
    }
    private void Update() {
        for(int i = 0; i < items.Length; i++){
            if(PlayerPrefs.GetInt("Item" + items[i].type + items[i].id) == 0){
                items[i].gameObject.SetActive(false);
            }
            else{
                items[i].gameObject.SetActive(true);
            }
        }
    }
}
