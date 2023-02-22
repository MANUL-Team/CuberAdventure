using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ForCraftItem : MonoBehaviour
{
    public Drop item;
    public int count;
    [SerializeField] private Text countText;
    private void Start() {
        GetComponent<Image>().sprite = item.icon;
    }
    private void FixedUpdate(){
        countText.text = count.ToString();
    }
}
