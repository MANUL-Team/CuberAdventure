using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShopMenuController : MonoBehaviour
{
    [SerializeField] private List<BuyItem> items = new List<BuyItem>();
    public int price;
    public int id;
    [SerializeField] private Slider slider;
    [SerializeField] private Text textSlider;
    [SerializeField] private GameObject scale;
    [SerializeField] private Transform itemsFolder;
    [SerializeField] private GameObject successful, fail;
    public GameObject descriptionObj;
    public Text description, name;
    public bool sell;
    void Start(){
        for(int i = 0; i < itemsFolder.childCount; i++){
            items.Add(itemsFolder.GetChild(i).GetComponent<BuyItem>());
        }
    }
    public void ChangeCount(){
        if(PlayerPrefs.GetInt("Coins") >= price){
            scale.SetActive(true);
            slider.maxValue = Mathf.RoundToInt(PlayerPrefs.GetInt("Coins")/price);
        }
        else{
            fail.SetActive(true);
        }
    }
    public void ChangeCountForSell(){
        if(PlayerPrefs.GetInt("Item " + id) > 0){
            scale.SetActive(true);
            slider.maxValue = PlayerPrefs.GetInt("Item " + id);
        }
        else{
            fail.SetActive(true);
        }
    }
    public void BuyItem(){
        PlayerPrefs.SetInt("Coins", PlayerPrefs.GetInt("Coins") - (Mathf.RoundToInt(slider.value)*price));
        PlayerPrefs.SetInt("Item" + " " + id, PlayerPrefs.GetInt("Item" + " " + id) + Mathf.RoundToInt(slider.value));
        successful.SetActive(true);
        scale.SetActive(false);
    }
    public void SellItem(){
        PlayerPrefs.SetInt("Item " + id, PlayerPrefs.GetInt("Item" + " " + id) - Mathf.RoundToInt(slider.value));
        PlayerPrefs.SetInt("Coins", PlayerPrefs.GetInt("Coins") + (Mathf.RoundToInt(slider.value)*price));
        successful.SetActive(true);
        scale.SetActive(false);
    }
    void FixedUpdate(){
        textSlider.text = (Mathf.RoundToInt(slider.value) + "/" + slider.maxValue).ToString();
    }
}
