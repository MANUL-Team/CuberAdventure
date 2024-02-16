using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
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
    public void SelectItem()
    {
        int lang = PlayerPrefs.GetInt("Language");
        itemControl.ClosePotDesc();
        if(potion){
            itemControl.CloseDescriptions();
            description.SetActive(true);
        }
        else{
            itemControl.OpenDescription();
            switch (lang)
            {
                case 0:
                    itemControl.name.text = item.nameRu;
                    itemControl.description.text = item.descriptionRu;
                    break;
                default:
                    itemControl.name.text = item.nameEng;
                    itemControl.description.text = item.descriptionEng;
                    break;
            }
        }
    }
}
