using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Item : MonoBehaviour
{
    [Header("Item Properties")]
    public string type;
    public int id;

    [Header("Text")]
    [SerializeField] private Text count;
    public GameObject description;
    
    [Header("ItemControl")]
    [SerializeField] private ItemControl itemControl;

    private void Update() {
        count.text = PlayerPrefs.GetInt("Item" + type + id).ToString();
    }
    public void SelectItem(){
        itemControl.CloseDescriprions();
        description.SetActive(true);
    }
}
