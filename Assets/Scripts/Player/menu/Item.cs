using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Item : MonoBehaviour
{
    [Header("Item Properties")]
    public Drop item;
    public bool potion;

    [Header("Text")]
    [SerializeField] private Text count;
    public GameObject description;

    
    [Header("ItemControl")]
    [SerializeField] private ItemControl itemControl;
    public string sorting;
    IEnumerator CountUpdate(){
        while(true){
            count.text = PlayerPrefs.GetInt("Item" + " " + item.id).ToString();
            yield return new WaitForSeconds(0.5f);
        }
    }
    private void OnEnable() {
        StartCoroutine(CountUpdate());
    }
    private void Start() {
        transform.GetChild(0).GetComponent<Image>().sprite = item.icon;
    }
    public void SelectItem(){
        itemControl.ClosePotDesc();
        if(potion){
            itemControl.CloseDescriptions();
            description.SetActive(true);
        }
        else{
            itemControl.OpenDescription();
            if(PlayerPrefs.GetInt("Language") == 1){
                itemControl.name.text = item.nameRu;
                itemControl.description.text = item.descriptionRu;
            }
            else if(PlayerPrefs.GetInt("Language") == 0){
                itemControl.name.text = item.nameEng;
                itemControl.description.text = item.descriptionEng;
            }
            
        }
    }
}
