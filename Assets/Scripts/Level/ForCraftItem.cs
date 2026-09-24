using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ForCraftItem : MonoBehaviour
{
    public Drop item;
    public int count;
    [SerializeField] private Text countText;
    void Awake()
    {
        ShowIcon();
    }

    private void Start() {
        ShowIcon();
    }

    void ShowIcon()
    {
        Image image = GetComponent<Image>();
        if (image == null)
            return;
        if (item != null)
            image.sprite = item.icon;
        TradeLook.FitIcon(image);
    }
    private void FixedUpdate(){
        if (countText == null || item == null)
            return;
        int have = PlayerPrefs.GetInt("Item " + item.id);
        countText.text = have + "/" + count;
        countText.color = have >= count
            ? new Color(0.55f, 0.86f, 0.62f, 1f)
            : new Color(0.93f, 0.45f, 0.32f, 1f);
    }
}
