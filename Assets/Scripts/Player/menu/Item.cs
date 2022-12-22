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
    IEnumerator CountUpdate(){
        while(true){
            count.text = PlayerPrefs.GetInt("Item" + type + id).ToString();
            yield return new WaitForSeconds(0.5f);
        }
    }
    private void OnEnable() {
        StartCoroutine(CountUpdate());
    }
    public void SelectItem(){
        itemControl.CloseDescriprions();
        description.SetActive(true);
    }
}
