using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NeedToCraft : MonoBehaviour
{
    public Drop[] needToCraft;
    public int[] count;
    public CraftedItem module;
    public Drop item;
    public bool stack;
    public Image icon;

    void Awake()
    {
        ApplyIcon();
    }

    private void Start() {
        ApplyIcon();
    }

    void ApplyIcon()
    {
        if (transform.childCount == 0)
            return;
        icon = transform.GetChild(0).GetComponent<Image>();
        if (icon == null)
            return;
        if (!stack && module != null)
            icon.sprite = module.icon;
        else if (item != null)
            icon.sprite = item.icon;
        TradeLook.FitFrame(GetComponent<Image>());
        TradeLook.FitIcon(icon);
    }
}