using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BuyItem : MonoBehaviour
{
    public Drop item;
    private Text textPrice;
    private Image icon;
    [SerializeField] private ShopMenuController shop;
    void Start(){
        textPrice = transform.GetChild(1).GetComponent<Text>();
        icon = transform.GetChild(0).GetComponent<Image>();
        icon.sprite = item.icon;
        if(!shop.sell){
            textPrice.text = item.price.ToString();
        }
        else{
            textPrice.text = (Mathf.RoundToInt(item.price*0.8f)).ToString();
        }
    }
    public void SelectItem(){
        shop.descriptionObj.SetActive(true);
        if(PlayerPrefs.GetInt("Language") == 0){
            shop.description.text = item.descriptionEng;
            shop.name.text = item.nameEng;
        }
        else if(PlayerPrefs.GetInt("Language") == 1){
            shop.description.text = item.descriptionRu;
            shop.name.text = item.nameRu;
        }
        shop.id = item.id;
        if(!shop.sell){
            shop.price = item.price;
        }
        else{
            shop.price = Mathf.RoundToInt(item.price*0.8f);
        }
        
    }
}
